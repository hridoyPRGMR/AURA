using System.Text;
using System.Text.Json;
using Core.Common;
using Core.IRepositories;
using Core.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Dtos;

namespace Worker.Services
{
    public sealed class TaskConsumer(
        IServiceScopeFactory scopeFactory,
        IConnection connection,
        ILogger<TaskConsumer> logger): BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var queueName = QueueNames.TaskCreated;

            var channel =
                await connection.CreateChannelAsync(cancellationToken: stoppingToken);

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
                        logger.LogWarning("Received invalid TaskCreatedMessage");

                        await channel.BasicAckAsync(
                            ea.DeliveryTag,
                            false,
                            stoppingToken);

                        return;
                    }

                    using var scope = scopeFactory.CreateScope();
                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
                    await taskService.ProcessTaskAsync(message, stoppingToken);

                    await channel.BasicAckAsync(
                        ea.DeliveryTag,
                        false,
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing RabbitMQ message");

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

            logger.LogInformation("TaskConsumer started. Listening on queue {Queue}", queueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}

