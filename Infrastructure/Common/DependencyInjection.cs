using Core.IRepositories;
using Core.IServices;
using Infrastructure.Persistence;
using Infrastructure.RabbitMq;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AuraDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Default"));
            });

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddSingleton<IIdGenerator, SnowflakeIdGenerator>();
            services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            services.AddSingleton<IConnection>(sp =>
            {
                var rabbitmqSettings = configuration.GetSection("RabbitMq");
                var factory = new ConnectionFactory
                {
                    HostName = rabbitmqSettings["Host"] ?? "localhost",
                    Port = int.TryParse(rabbitmqSettings["Port"], out var port) ? port : 5672,
                    UserName = rabbitmqSettings["Username"] ?? "guest",
                    Password = rabbitmqSettings["Password"] ?? "guest"
                };

                return factory.CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult();
            });

            return services;
        }
    }
}