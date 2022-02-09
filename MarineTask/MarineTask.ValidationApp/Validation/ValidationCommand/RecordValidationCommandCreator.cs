using MarineTask.Configuration;
using MarineTask.Core.IO.Abstractions;
using MarineTask.Core.IO.Azure.CloudBlob;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace MarineTask.ValidationApp.Validation.ValidationCommand
{
    internal class RecordValidationCommandCreator : IRecordValidationCommandCreator
    {
        private readonly ICloudBlobClientResolver cloudBlobClientResolver;
        private readonly IInventorySequenceGenerator inventorySequenceGenerator;
        private readonly IBlockIdConverter blockIdConverter;
        private readonly IFilePathProvider filePathProvider;
        private readonly AppConfiguration config;

        public RecordValidationCommandCreator(
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

        public RecordValidationCommand CreateCommand(RecordValidationCommandData data)
        {
            var expectedSequence = inventorySequenceGenerator.GenerateSequence(data.Inventories);

            var blobConnectionString = this.config.ConnectionStrings.FileStore.ConnectionString;
            var blobContainer = this.config.ConnectionStrings.FileStore.Container;

            var filePath = this.filePathProvider.GetRecordFilePath($"{data.RecordId}.txt");

            var blockClient = this.cloudBlobClientResolver.GetCloudBlockBlobClient(filePath, blobConnectionString);

            // Get committed blocks from block blob
            var actualSequence = blockClient.Exists() ?
                blockClient.GetBlockList().Value.CommittedBlocks
                    .Select(_ => this.blockIdConverter.GetStringFromBlockId(_.Name, 25)).ToList()
                : new List<string>(0);

            return new RecordValidationCommand 
            {
                RecordId = data.RecordId,
                ExpectedSequence = expectedSequence,
                ActualSequence = actualSequence
            };
        }
    }
}
