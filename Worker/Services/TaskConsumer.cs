using System.Text;
using System.Text.Json;
using Core.IRepositories;
using Core.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker.Services
{
    public sealed class TaskConsumer(
    IConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<TaskConsumer> logger)
    : BackgroundService
    {
        private readonly IConnection _connection = connection;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<TaskConsumer> _logger = logger;

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var queueName = "task-created";

            await using var channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var message = JsonSerializer.Deserialize<TaskCreatedMessage>(json);

                    if (message is null)
                    {
                        _logger.LogWarning("Received invalid TaskCreatedMessage");

                        await channel.BasicAckAsync(
                            ea.DeliveryTag,
                            false,
                            stoppingToken);

                        return;
                    }

                    await ProcessTaskAsync(message, stoppingToken);

                    await channel.BasicAckAsync(
                        ea.DeliveryTag,
                        false,
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RabbitMQ message");

                    await channel.BasicNackAsync(
                        ea.DeliveryTag,
                        false,
                        true,
                        stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("TaskConsumer started. Listening on queue {Queue}", queueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessTaskAsync(
            TaskCreatedMessage message,
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var taskRepository =
                scope.ServiceProvider
                    .GetRequiredService<ITaskRepository>();

            var llmService = scope.ServiceProvider
                    .GetRequiredService<ILLMService>();

            var task = await taskRepository.GetByIdAsync(message.TaskId)
                ?? throw new InvalidOperationException(
                    $"Task {message.TaskId} not found.");

            task.MarkRunning();
            await taskRepository.SaveChangesAsync(cancellationToken);

            try
            {
                var response =
                    await llmService.ExecuteTaskAsync(
                        task.UserPrompt);

                // TODO:
                // task.SetResult(response);

                task.MarkCompleted();
                await taskRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                task.MarkFailed(ex.Message);
                await taskRepository.SaveChangesAsync(cancellationToken);
                throw;
            }
        }
    }

    public sealed class TaskCreatedMessage
    {
        public long TaskId { get; set; }
    }
}

