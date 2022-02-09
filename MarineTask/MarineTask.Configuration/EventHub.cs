namespace MarineTask.Configuration
{
    public class EventHub
    {
        public string ConnectionString { get; set; }

        public string HubName { get; set; }

        public FileStore BlobStorage { get; set; }
    }
}
