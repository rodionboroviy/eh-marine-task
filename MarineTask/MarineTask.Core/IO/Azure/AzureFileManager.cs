using Azure.Storage.Blobs.Specialized;
using MarineTask.Configuration;
using MarineTask.Core.Extensions;
using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure.CloudBlob;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarineTask.Core.IO.Azure
{
    public class AzureFileManager : IFileWriter
    {
        private readonly AppConfiguration config;
        private readonly ICloudBlobClientResolver cloudBlobResolver;

        public AzureFileManager(
            IOptions<AppConfiguration> config,
            ICloudBlobClientResolver cloudBlobResolver)
        {
            this.config = config.Value;
            this.cloudBlobResolver = cloudBlobResolver;
        }

        public async Task<string> WriteFile(Stream contentToWrite, string path, int blockNumber, int blockCount)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            string blockId = $"{blockNumber:00000}";
            string base64BlockId = blockId.ToBase64();

            var blockBlobClient = this.cloudBlobResolver.GetCloudBlockBlobClient(
                path,
                this.config.ConnectionStrings.FileStore.ConnectionString);

            await blockBlobClient.StageBlockAsync(base64BlockId, contentToWrite);

            await TryToCommitFile(blockBlobClient, blockCount);

            return blockBlobClient.Uri.ToString();
        }

        private async Task TryToCommitFile(BlockBlobClient blockBlobClient, int blockCount)
        {
            var blockList = Enumerable.Range(1, (int)blockCount).Select(i => $"{i:00000}".ToBase64());

            var list = await blockBlobClient.GetBlockListAsync();
            var uncommitedBlocks = list.Value.UncommittedBlocks.Select(_ => _.Name);

            if (uncommitedBlocks.Count() == blockList.Count()
                && uncommitedBlocks.All(_ => blockList.Contains(_)))
            {
                await blockBlobClient.CommitBlockListAsync(blockList);
            }
        }
    }
}
