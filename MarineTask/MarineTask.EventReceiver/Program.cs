using MarineTask.Configuration.Extensions;
using MarineTask.Core.Extensions;
using MarineTask.EventReceiver.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using MarineTask.EventReceiver.Consumer;

namespace MarineTask.EventReceiver
{
    internal class Program
    {
        static async Task Main()
        {
            var serviceProvider =
                ConfigureServices()
                    .BuildServiceProvider();

            var cancellationSource = new CancellationTokenSource();

            cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

            var consumer = serviceProvider.GetService<IEventConsumer>();

            // Start the processing
            await consumer.Start();

            try
            {
                // The processor performs its work in the background; block until cancellation
                // to allow processing to take place.
                await Task.Delay(Timeout.Infinite, cancellationSource.Token);
            }
            catch (TaskCanceledException)
            {
                // This is expected when the delay is canceled.
            }

            // Stop the processing
            await consumer.Stop();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddConfiguration();
            services.AddConsumeServices();
            services.AddAzureIO();

            return services;
        }
    }
}
