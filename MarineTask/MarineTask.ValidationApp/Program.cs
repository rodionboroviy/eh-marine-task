using MarineTask.Core.Extensions;
using MarineTask.ValidationApp.Extensions;
using MarineTask.ValidationApp.Processors;
using MarineTask.ValidationApp.Processors.Result;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using MarineTask.Configuration;

namespace MarineTask.ValidationApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider =
                ConfigureServices()
                    .BuildServiceProvider();

            var sourceDirectory = "D:\\Test\\Records\\Source";

            var files = Directory.GetFiles(sourceDirectory);

            foreach (var file in files)
            {
                using(var scope = serviceProvider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetService<IRecordLineProcessor<RecordProcessResult>>();

                    using (StreamReader sr = new StreamReader(file))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var line = sr.ReadLine();
                            processor.ProcessLine(line);
                        }
                    }

                    var result = processor.GetResult();

                    Console.WriteLine(result.Result.RecordUrl);
                    Console.WriteLine($"Validated Record: {result.Result.ValidatedRecordId}");
                    Console.WriteLine($"Missed sequences: {string.Join(", ", result.Result.MissedSequences)}");
                    Console.WriteLine("----------------------------------------------");
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

            services.AddRecordProcessServices();
            services.AddAzureIO();

            return services;
        }
    }
}
