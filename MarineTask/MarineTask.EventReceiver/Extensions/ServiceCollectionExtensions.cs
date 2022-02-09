using MarineTask.EventReceiver.Consumer;
using MarineTask.EventReceiver.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.EventReceiver.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsumeServices(this IServiceCollection services)
        {
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddScoped<IEventDataHandler, EventDataHandler>();

            return services;
        }
    }
}
