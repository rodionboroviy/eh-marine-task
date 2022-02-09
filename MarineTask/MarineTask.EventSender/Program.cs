using System;
using System.IO;
using System.Threading.Tasks;
using MarineTask.Configuration;
using MarineTask.Configuration.Extensions;
using MarineTask.Core.Extensions;
using MarineTask.EventSender.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarineTask.EventSender
{
    internal class Program
    {
        static async Task Main()
        {
            var serviceProvider =
                ConfigureServices()
                    .BuildServiceProvider();

            string imageDirectory = Path.Combine(Environment.CurrentDirectory, "Images/vessel.jpg");

            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                using (FileStream fsSource = new FileStream(imageDirectory, FileMode.Open, FileAccess.Read))
                {
                    var uploadManager = scope.ServiceProvider.GetService<IUploadManager>();

                    await uploadManager.SendStreamInChuncks(fsSource, $"{Guid.NewGuid()}_vessel.jpg");
                }
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddConfiguration();
            services.AddSendServices();
            services.AddAzureIO();

            return services;
        }
    }
}
