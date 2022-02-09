using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace MarineTask.EventSender
{
    internal class Program
    {
        // connection string to the Event Hubs namespace
        private const string connectionString = "Endpoint=sb://marinetask.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xrlajIeVHdrer8RYXMe3ZPqIf6Jh/akqgE9VkJDuEb0=";

        // name of the event hub
        private const string eventHubName = "IMO9648714";
        private const string eventHubName_v1 = "IMO9648714_v1";

        static EventHubProducerClient producerClient;

        static async Task Main()
        {
            string imageDirectory = Path.Combine(Environment.CurrentDirectory, "Images/vessel.jpg");

            using (FileStream fsSource = new FileStream(imageDirectory, FileMode.Open, FileAccess.Read))
            {
                UploadManager uploadManager = new UploadManager();

                await uploadManager.UploadStreamAsync(fsSource, $"{Guid.NewGuid()}_vessel.jpg");
            }
        }
    }
}
