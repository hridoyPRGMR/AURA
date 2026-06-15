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
            services.AddScoped<IMessagePublisher,RabbitMqPublisher>();
            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                return factory.CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult();
            });

            return services;
        }
    }
}