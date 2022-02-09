using Azure.Messaging.EventHubs;
using MarineTask.Common;
using MarineTask.Core.IO.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace MarineTask.EventReceiver.Handler
{
    internal class EventDataHandler : IEventDataHandler
    {
        private readonly IFileWriter fileWriter;
        private readonly IFilePathProvider filePathProvider;

        public EventDataHandler(
            IFileWriter fileWriter,
            IFilePathProvider filePathProvider)
        {
            this.fileWriter = fileWriter;
            this.filePathProvider = filePathProvider;
        }

        public async Task ConsumeEvent(EventData eventData)
        {
            var fileName = (string)eventData.Properties[Constants.EventDataPropFileName];
            var blockNumber = (int)eventData.Properties[Constants.EventDataPropBlockNumber];
            var blockCount = (int)eventData.Properties[Constants.EventDataPropBlockCount];

            var destinationPath = this.filePathProvider.GetImageFilePath(fileName);

            // Write the body of the event to the blob
            using (var stream = new MemoryStream(eventData.Body.ToArray(), true))
            {
                var url = await fileWriter.WriteFile(
                    stream,
                    destinationPath,
                    blockNumber,
                    blockCount);
            }
        }
    }
}
