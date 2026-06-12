using Core.IServices;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<ITaskService,TaskService>();

            return services;
        }
    }
}