namespace MarineTask.EventSender
{
    internal class PublishDetails
    {
        public string FileName { get; set; }

        public int BlockNumber { get; set; }

        public int BlockCount { get; set; }

        public byte[] Data { get; set; }
    }
}
