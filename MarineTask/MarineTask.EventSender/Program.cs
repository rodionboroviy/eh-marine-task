using System;
using System.IO;
using System.Threading.Tasks;
using MarineTask.Configuration;
using MarineTask.Core.Extensions;
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

            using (FileStream fsSource = new FileStream(imageDirectory, FileMode.Open, FileAccess.Read))
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var uploadManager = scope.ServiceProvider.GetService<IUploadManager>();

                    await uploadManager.SendStreamInChuncks(fsSource, $"{Guid.NewGuid()}_vessel.jpg");
                }
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            services.AddSingleton<IConfiguration>(_ => config);

            services.Configure<AppConfiguration>(config);

            services.AddAzureIO();

            return services;
        }
    }
}
