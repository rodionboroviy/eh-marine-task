using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MarineTask.EventSender
{
    internal class UploadManager : IUploadManager
    {
        // connection string to the Event Hubs namespace
        private const string connectionString = "Endpoint=sb://marinetask.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xrlajIeVHdrer8RYXMe3ZPqIf6Jh/akqgE9VkJDuEb0=";

        // name of the event hub
        private const string eventHubName = "IMO9648714";
        private const string eventHubName_v1 = "IMO9648714_v1";

        public UploadManager()
        {

        }

        // Upload stream in chyncks to simulate huge file upload with number of events to EventHub
        public async Task SendStreamInChuncks(Stream stream, string name, int pageSizeInBytes = 50_000)
        {
            // number of event to be sent
            long blocksCount = stream.Length / pageSizeInBytes + 1;

            // local variable to track the current number of bytes read into buffer
            int bytesRead;

            // track the current block number as the code iterates through the file
            int blockNumber = 0;

            var publishDetails = new List<PublishDetails>();

            do
            {
                // increment block number by 1 each iteration
                blockNumber++;

                // create buffer and retrieve chunk
                byte[] buffer = new byte[pageSizeInBytes];
                bytesRead = await stream.ReadAsync(buffer, 0, pageSizeInBytes);

                publishDetails.Add(new PublishDetails
                { 
                    FileName = name,
                    BlockNumber = blockNumber,
                    BlockCount = blocksCount,
                    Data = buffer
                });

                // While bytesRead == size it means there is more data left to read and process
            } while (bytesRead == pageSizeInBytes);

            await using (var publisher = new EventPublisher(connectionString, eventHubName_v1))
            {
                await publisher.Publish(publishDetails);
            }
        }
    }
}
