using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using MarineTask.Common;
using MarineTask.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MarineTask.EventSender.Producer
{
    internal class EventProducer : IEventProducer
    {
        private EventHubProducerClient producerClient;

        private readonly AppConfiguration config;

        public EventProducer(IOptions<AppConfiguration> config)
        {
            this.config = config.Value;

            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(
                this.config.ConnectionStrings.EventHub.ConnectionString,
                this.config.ConnectionStrings.EventHub.HubName);
        }

        public async Task Publish(PublishDetails details)
        {
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            var eventData = new EventData(BinaryData.FromBytes(details.Data));
            eventData.Properties.Add(Constants.EventDataPropFileName, details.FileName);
            eventData.Properties.Add(Constants.EventDataPropBlockNumber, details.BlockNumber);
            eventData.Properties.Add(Constants.EventDataPropBlockCount, details.BlockCount);

            if (!eventBatch.TryAdd(eventData))
            {
                // if it is too large for the batch
                throw new Exception($"Event is too large for the batch and cannot be sent.");
            }

            await producerClient.SendAsync(eventBatch);
        }

        #region IDisposable

        private bool disposed;

        public async ValueTask DisposeAsync()
        {
            await this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async ValueTask Dispose(bool disposing)
        {
            if (this.disposed)
            {
                await new ValueTask();
            }

            if (disposing)
            {
                if (producerClient != null)
                {
                    await producerClient.DisposeAsync();
                }

                producerClient = null;
            }

            this.disposed = true;
        }

        #endregion IDisposable
    }
}
