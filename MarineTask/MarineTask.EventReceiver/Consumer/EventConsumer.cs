using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using MarineTask.Configuration;
using MarineTask.EventReceiver.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MarineTask.EventReceiver.Consumer
{
    internal class EventConsumer : IEventConsumer
    {
        private readonly AppConfiguration config;
        private readonly IServiceProvider serviceProvider;

        private EventProcessorClient processor;

        // Read from the default consumer group: $Default
        private string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        // Create a blob container client that the event processor will use 
        private BlobContainerClient storageClient;

        public EventConsumer(
            IOptions<AppConfiguration> config,
            IServiceProvider serviceProvider)
        {
            this.config = config.Value;
            this.serviceProvider = serviceProvider;

            this.storageClient = new BlobContainerClient(
                this.config.ConnectionStrings.EventHub.BlobStorage.ConnectionString,
                this.config.ConnectionStrings.EventHub.BlobStorage.Container);
        }

        public async Task Start()
        {
            processor = new EventProcessorClient(
                storageClient,
                consumerGroup,
                this.config.ConnectionStrings.EventHub.ConnectionString,
                this.config.ConnectionStrings.EventHub.HubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            try
            {
                await processor.StopProcessingAsync();
            }
            finally
            {
                // To prevent leaks, the handlers should be removed when processing is complete.
                processor.ProcessEventAsync -= ProcessEventHandler;
                processor.ProcessErrorAsync -= ProcessErrorHandler;
            }
        }

        private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var consumer = scope.ServiceProvider.GetService<IEventDataHandler>();

                    await consumer.ConsumeEvent(eventArgs.Data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
                await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            }
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
