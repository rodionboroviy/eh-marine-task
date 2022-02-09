using MarineTask.EventSender.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.EventSender.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendServices(this IServiceCollection services)
        {
            services.AddScoped<IUploadManager, UploadManager>();
            services.AddScoped<IEventProducer, EventProducer>();

            return services;
        }
    }
}
