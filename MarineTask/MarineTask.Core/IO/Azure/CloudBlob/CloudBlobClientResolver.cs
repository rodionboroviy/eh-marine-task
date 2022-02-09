using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.IO;
using System.Linq;

namespace MarineTask.Core.IO.Azure.CloudBlob
{
    public class CloudBlobClientResolver : ICloudBlobClientResolver
    {
        public BlockBlobClient GetCloudBlockBlobClient(string path, string connectionString)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            path = path.Replace(blobServiceClient.Uri.ToString(), string.Empty);

            var pathParts = path
                .Split(new char[] { Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (!pathParts.Any())
            {
                throw new ArgumentException("At least one path part must be given. This will be mapped to the container name.", nameof(path));
            }

            var containerName = pathParts[0];
            pathParts.RemoveAt(0);

            var blobPath = Path.Combine(pathParts.ToArray())
                .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(blobPath);

            return blockBlobClient;
        }
    }
}
