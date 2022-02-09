using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using MarineTask.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultipleWrites
{
    internal class Program
    {
        private const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stgmarinetask;AccountKey=IpmNwSBrsAC4XfrYlYtlC45SOL496/0zFE5SYfRbxbfhr/ZabKEYU35QWIi68BEsX6KCfEOaNE131HsM1l0EKg==;EndpointSuffix=core.windows.net";
        private const string blobContainerName = "ttt";

        static async Task Main(string[] args)
        {
            var ids = Enumerable.Range(0, 5).Select(x => $"{x:00000}".ToBase64());

            var tasks = ids.Select(x => Task.Run(() => NewMethod(x)));

            await Task.WhenAll(tasks);

            BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

            var blobPath = "test/fileName11.txt";

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(blobPath);

            await blockBlobClient.CommitBlockListAsync(ids);

            Console.WriteLine("Hello World!");
        }

        private static async Task NewMethod(string blockId)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}:{blockId}");

            string base64BlockId = blockId.ToBase64();

            BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

            var blobPath = "test/fileName11.txt";

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(blobPath);

            using (var stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.WriteLine("Test Text");
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    await blockBlobClient.StageBlockAsync(blockId, stream);
                }
            }
        }
    }
}
