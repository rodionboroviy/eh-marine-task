using MarineTask.Configuration;
using MarineTask.Core.IO.Abstractions;
using Microsoft.Extensions.Options;

namespace MarineTask.Core.IO.Azure
{
    public class FilePathProvider : IFilePathProvider
    {
        private readonly AppConfiguration config;

        public FilePathProvider(IOptions<AppConfiguration> config)
        {
            this.config = config.Value;
        }

        public string GetImageFilePath(string fileName)
        {
            return $"{this.config.ConnectionStrings.FileStore.Container}/{this.config.ConnectionStrings.EventHub.HubName}/images/{fileName}";
        }

        public string GetRecordFilePath(string fileName)
        {
            return $"{this.config.ConnectionStrings.FileStore.Container}/records/{fileName}";
        }
    }
}
