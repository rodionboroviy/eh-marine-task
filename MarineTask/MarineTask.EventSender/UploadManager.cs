using MarineTask.EventSender.Producer;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MarineTask.EventSender
{
    internal class UploadManager : IUploadManager
    {
        private readonly IEventProducer eventPublisher;

        public UploadManager(IEventProducer eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        // Upload stream in chyncks to simulate huge file upload with number of events to EventHub
        public async Task SendStreamInChuncks(Stream stream, string name, int pageSizeInBytes = 50_000)
        {
            // number of event to be sent
            int blocksCount = (int)(stream.Length / pageSizeInBytes + 1);

            // local variable to track the current number of bytes read into buffer
            int bytesRead;

            // track the current block number as the code iterates through the file
            int blockNumber = 0;

            do
            {
                // increment block number by 1 each iteration
                blockNumber++;

                // create buffer and retrieve chunk
                byte[] buffer = new byte[pageSizeInBytes];
                bytesRead = await stream.ReadAsync(buffer, 0, pageSizeInBytes);

                var publishDetails = new PublishDetails
                { 
                    FileName = name,
                    BlockNumber = blockNumber,
                    BlockCount = blocksCount,
                    Data = buffer
                };

                await this.eventPublisher.Publish(publishDetails);

                // While bytesRead == size it means there is more data left to read and process
            } while (bytesRead == pageSizeInBytes);

            await this.eventPublisher.DisposeAsync();
        }
    }
}
