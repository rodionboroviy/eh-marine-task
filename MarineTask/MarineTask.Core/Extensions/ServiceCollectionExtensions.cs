using MarineTask.Core.IO.Azure.CloudBlob;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureIO(this IServiceCollection services)
        {
            services.AddSingleton<ICloudBlobClientResolver, CloudBlobClientResolver>();
            services.AddSingleton<IBlockIdConverter, BlockIdConverter>();

            return services;
        }
    }
}
