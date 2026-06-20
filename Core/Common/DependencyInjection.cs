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
            services.AddScoped<ILLMService,LLMService>();
            services.AddSingleton(sp =>
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(300); // increased timeout for model calls
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });
            services.AddTransient<IAiAgentCallingService, AiAgentCallingService>();

            return services;
        }
    }
}