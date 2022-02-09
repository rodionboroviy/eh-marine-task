using MarineTask.Configuration;
using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure.CloudBlob;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarineTask.ValidationApp.Validation.ValidationContext
{
    internal class RecordValidationContextCreator : IRecordValidationContextCreator
    {
        private readonly ICloudBlobClientResolver cloudBlobClientResolver;
        private readonly IInventorySequenceGenerator inventorySequenceGenerator;
        private readonly IBlockIdConverter blockIdConverter;
        private readonly IFilePathProvider filePathProvider;
        private readonly AppConfiguration config;

        public RecordValidationContextCreator(
            ICloudBlobClientResolver cloudBlobClientResolver,
            IInventorySequenceGenerator inventorySequenceGenerator,
            IBlockIdConverter blockIdConverter,
            IFilePathProvider filePathProvider,
            IOptions<AppConfiguration> config)
        {
            this.cloudBlobClientResolver = cloudBlobClientResolver;
            this.inventorySequenceGenerator = inventorySequenceGenerator;
            this.blockIdConverter = blockIdConverter;
            this.filePathProvider = filePathProvider;
            this.config = config.Value;
        }

        public async Task<RecordValidationContext> CreateCommand(RecordValidationContextData data)
        {
            var filePath = this.filePathProvider.GetRecordFilePath($"{data.RecordId}.txt");
            var blobConnectionString = this.config.ConnectionStrings.FileStore.ConnectionString;
            var blobContainer = this.config.ConnectionStrings.FileStore.Container;

            var expectedSequence = inventorySequenceGenerator.GenerateSequence(data.Inventories);

            var blockClient = this.cloudBlobClientResolver.GetCloudBlockBlobClient(filePath, blobConnectionString);

            var actualSequence = new List<string>(0);
            var blockBlobExist = await blockClient.ExistsAsync();

            // Get committed blocks from block blob
            if (blockBlobExist)
            {
                var blockList = await blockClient.GetBlockListAsync();

                actualSequence = blockList.Value.CommittedBlocks
                    .Select(_ => this.blockIdConverter.GetStringFromBlockId(_.Name, 25))
                    .ToList();
            }

            return new RecordValidationContext 
            {
                RecordId = data.RecordId,
                ExpectedSequence = expectedSequence,
                ActualSequence = actualSequence
            };
        }
    }
}
