using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace Infrastructure.Common
{
    public interface IConnectionProvider
    {
        IConnection GetConnection();
    }

    public sealed class RabbitMqConnectionProvider : IConnectionProvider, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqConnectionProvider>? _logger;
        private readonly int _maxAttempts;
        private readonly TimeSpan _initialDelay;
        private IConnection? _connection;
        private bool _disposed;

        public RabbitMqConnectionProvider(IConfiguration configuration, ILogger<RabbitMqConnectionProvider>? logger)
        {
            _logger = logger;
            var rabbitmqSettings = configuration.GetSection("RabbitMq");
            _factory = new ConnectionFactory
            {
                HostName = rabbitmqSettings["Host"] ?? "localhost",
                Port = int.TryParse(rabbitmqSettings["Port"], out var port) ? port : 5672,
                UserName = rabbitmqSettings["Username"] ?? "guest",
                Password = rabbitmqSettings["Password"] ?? "guest"
            };

            _maxAttempts = 5;
            _initialDelay = TimeSpan.FromSeconds(2);
        }

        public IConnection GetConnection()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMqConnectionProvider));
            }

            if (_connection is not null && _connection.IsOpen)
            {
                return _connection;
            }

            var delay = _initialDelay;
            for (var attempt = 1; attempt <= _maxAttempts; attempt++)
            {
                try
                {
                    _connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
                    _logger?.LogInformation("Connected to RabbitMQ on attempt {Attempt}", attempt);
                    return _connection;
                }
                catch (Exception ex) when (attempt < _maxAttempts)
                {
                    _logger?.LogWarning(ex, "RabbitMQ not reachable (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}s...", attempt, _maxAttempts, delay.TotalSeconds);
                    Thread.Sleep(delay);
                    delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 30));
                }
            }

            _connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
            return _connection;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _connection?.Dispose();
        }
    }
}
