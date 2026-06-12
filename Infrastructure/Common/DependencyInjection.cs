using Core.IRepositories;
using Core.IServices;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            
            services.AddScoped<ITaskRepository,TaskRepository>();
            services.AddSingleton<IIdGenerator, SnowflakeIdGenerator>();

            return services;
        }
    }
}