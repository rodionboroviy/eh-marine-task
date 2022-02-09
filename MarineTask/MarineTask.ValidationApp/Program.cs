using MarineTask.ValidationApp.Processors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarineTask.ValidationApp
{
    internal class Program
    {
        public static IConfiguration Configuration;

        static void Main(string[] args)
        {
            var sourceDirectory = "D:\\Test\\Records\\Source";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var files = Directory.GetFiles(sourceDirectory);

            foreach (var file in files)
            {
                var processor = new RecordProcessor();

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

        //private static IServiceCollection ConfigureServices()
        //{
        //    IServiceCollection services = new ServiceCollection();

        //    services.AddLogging(configure => configure.AddConsole());

        //    services.AddSingleton(
        //        new ConfigurationBuilder()
        //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //            .Build());

        //    services.AddTransient<ServiceBusConfigurator>();
        //    services.AddTransient<ArmProcessorFactory>();
        //    services.AddTransient<ArmObjectSubstituter>();
        //    services.AddTransient<ArmValueSubstituter>();
        //    services.AddTransient<ArmTimeSpanConverter>();
        //    services.AddTransient<ArmProcessor>();
        //    services.AddTransient<TokenGenerator>();
        //    services.AddTransient<QueueMapper>();
        //    services.AddTransient<QueueProcessor>();
        //    services.AddTransient<TopicMapper>();
        //    services.AddTransient<SubscriptionMapper>();
        //    services.AddTransient<RuleMapper>();
        //    services.AddTransient<TopicProcessor>();

        //    return services;
        //}
    }
}
