using MarineTask.ValidationApp.Processors;
using MarineTask.ValidationApp.Processors.Result;
using MarineTask.ValidationApp.Validation;
using MarineTask.ValidationApp.Validation.ValidationCommand;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.ValidationApp.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRecordProcessServices(this IServiceCollection services)
        {
            services.AddScoped<IRecordLineProcessor<InventoryResult>, InventoryLineProcessor>();
            services.AddScoped<IRecordLineProcessor<SequenceResult>, SequenceLineProcessor>();
            services.AddScoped<IRecordLineProcessor<RecordProcessResult>, RecordProcessor>();

            services.AddTransient<IRecordValidator, RecordValidator>();
            services.AddTransient<IInventorySequenceGenerator, InventorySequenceGenerator>();
            services.AddTransient<IRecordValidationCommandCreator, RecordValidationCommandCreator>();

            return services;
        }
    }
}
