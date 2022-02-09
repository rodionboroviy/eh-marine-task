using Azure.Storage.Blobs.Specialized;

namespace MarineTask.Core.IO.Azure.CloudBlob
{
    public interface ICloudBlobClientResolver
    {
        BlockBlobClient GetCloudBlockBlobClient(string path, string connectionString);
    }
}
