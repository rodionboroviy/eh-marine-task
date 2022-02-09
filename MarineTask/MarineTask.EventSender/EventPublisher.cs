using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarineTask.EventSender
{
    internal class EventPublisher : IAsyncDisposable
    {
        private readonly string connectionString;
        private readonly string hubName;

        private EventHubProducerClient producerClient;

        public EventPublisher(string connectionString, string hubName)
        {
            this.connectionString = connectionString;
            this.hubName = hubName;
        }

        public ValueTask DisposeAsync()
        {
            return producerClient.DisposeAsync();
        }

        public async Task Publish(List<PublishDetails> details)
        {
            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(this.connectionString, this.hubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            foreach (PublishDetails publishDetails in details)
            {
                var eventData = new EventData(BinaryData.FromBytes(publishDetails.Data));
                eventData.Properties.Add(nameof(PublishDetails.FileName), publishDetails.FileName);
                eventData.Properties.Add(nameof(PublishDetails.BlockNumber), publishDetails.BlockNumber);
                eventData.Properties.Add(nameof(PublishDetails.BlockCount), publishDetails.BlockCount);

                if (!eventBatch.TryAdd(eventData))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
        }
    }
}
