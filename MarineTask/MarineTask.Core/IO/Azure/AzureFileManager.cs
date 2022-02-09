using Azure.Storage.Blobs.Specialized;
using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure.CloudBlob;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineTask.Core.IO.Azure
{
    public class AzureFileManager : IFileReader, IFileWriter
    {
        private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stgmarinetask;AccountKey=IpmNwSBrsAC4XfrYlYtlC45SOL496/0zFE5SYfRbxbfhr/ZabKEYU35QWIi68BEsX6KCfEOaNE131HsM1l0EKg==;EndpointSuffix=core.windows.net";
        private readonly ICloudBlobClientResolver cloudBlobResolver;

        public AzureFileManager(ICloudBlobClientResolver cloudBlobResolver)
        {
            this.cloudBlobResolver = cloudBlobResolver;
        }

        public async Task<Stream> ReadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var blobClient = this.cloudBlobResolver.GetCloudBlockBlobClient(path, StorageConnectionString);

            return await blobClient.OpenReadAsync(); ;
        }

        public async Task<string> WriteFile(Stream contentToWrite, string path, int blockNumber, long blockCount)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            string blockId = $"{blockNumber:00}";
            string base64BlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
            var blockList = Enumerable.Range(1, (int)blockCount).Select(i => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{i:00}")));

            var blockBlobClient = this.cloudBlobResolver.GetCloudBlockBlobClient(path, StorageConnectionString);

            await blockBlobClient.StageBlockAsync(base64BlockId, contentToWrite);

            var list = await blockBlobClient.GetBlockListAsync();
            var uncommitedBlocks = list.Value.UncommittedBlocks.Select(_ => _.Name);

            if (uncommitedBlocks.Count() == blockList.Count() 
                && uncommitedBlocks.All(_ => blockList.Contains(_)))
            {
                await blockBlobClient.CommitBlockListAsync(blockList);
            }

            return blockBlobClient.Uri.ToString();
        }
    }
}
