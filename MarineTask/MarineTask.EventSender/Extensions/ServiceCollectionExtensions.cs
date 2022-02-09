using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarineTask.EventSender.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendServices(this IServiceCollection services)
        {
            services.AddScoped<IUploadManager, UploadManager>();

            return services;
        }
    }
}
