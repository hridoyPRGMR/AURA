using System.Text;
using System.Text.Json;
using Core.IServices;
using RabbitMQ.Client;
using Shared.Dtos;

namespace Infrastructure.RabbitMq;

public sealed class RabbitMqPublisher(
    IConnection connection)
    : IMessagePublisher
{
    private readonly IConnection _connection = connection;

    public async Task PublishAsync(
        string queueName,
        TaskCreatedMessage message,
        CancellationToken cancellationToken = default)
    {
        await using var channel =
            await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken);
    }
}