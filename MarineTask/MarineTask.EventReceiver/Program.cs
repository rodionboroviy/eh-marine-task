using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure;
using MarineTask.Core.IO.Azure.CloudBlob;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MarineTask.EventReceiver
{
    internal class Program
    {
        private const string ehubNamespaceConnectionString = "Endpoint=sb://marinetask.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xrlajIeVHdrer8RYXMe3ZPqIf6Jh/akqgE9VkJDuEb0=";
        private const string eventHubName = "IMO9648714_v1";

        private const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stgmarinetask;AccountKey=IpmNwSBrsAC4XfrYlYtlC45SOL496/0zFE5SYfRbxbfhr/ZabKEYU35QWIi68BEsX6KCfEOaNE131HsM1l0EKg==;EndpointSuffix=core.windows.net";
        private const string blobContainerName = "marineconsumerv1";

        static BlobContainerClient storageClient;

        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.        
        static EventProcessorClient processor;

        static async Task Main()
        {
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
            processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

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

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                IFileWriter fileWriter = new AzureFileManager(new CloudBlobClientResolver());

                var fileName = (string)eventArgs.Data.Properties["FileName"];
                var blockNumber = (int)eventArgs.Data.Properties["BlockNumber"];
                var blockCount = (long)eventArgs.Data.Properties["BlockCount"];

                var destinationPath = $"processedmarinefilesv1/{eventHubName}/{fileName}";

                // Write the body of the event to the blob
                var url = await fileWriter.WriteFile(
                    new MemoryStream(eventArgs.Data.Body.ToArray(), true),
                    destinationPath,
                    blockNumber,
                    blockCount);
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

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
