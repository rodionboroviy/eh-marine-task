using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure;
using MarineTask.Core.IO.Azure.CloudBlob;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureIO(this IServiceCollection services)
        {
            services.AddSingleton<ICloudBlobClientResolver, CloudBlobClientResolver>();
            services.AddSingleton<IFileWriter, AzureFileManager>();
            services.AddSingleton<IBlockIdConverter, BlockIdConverter>();
            services.AddSingleton<IFilePathProvider, FilePathProvider>();

            return services;
        }
    }
}
