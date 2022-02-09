using MarineTask.Core.IO.Azure.CloudBlob;
using System.Collections.Generic;
using System.Linq;

namespace MarineTask.ValidationApp.Validation.ValidationCommand
{
    internal class RecordValidationCommandCreator : IRecordValidationCommandCreator
    {
        private readonly ICloudBlobClientResolver cloudBlobClientResolver;
        private readonly IInventorySequenceGenerator inventorySequenceGenerator;
        private readonly IBlockIdConverter blockIdConverter;

        public RecordValidationCommandCreator()
        {
            this.cloudBlobClientResolver = new CloudBlobClientResolver();
            this.inventorySequenceGenerator = new InventorySequenceGenerator();
            this.blockIdConverter = new BlockIdConverter();
        }

        public RecordValidationCommand CreateCommand(RecordValidationCommandData data)
        {
            var expectedSequence = inventorySequenceGenerator.GenerateSequence(data.Inventories);

            var blobConnectionString = Program.Configuration["ConnectionStrings:FileStore:ConnectionString"];
            var blobContainer = Program.Configuration["ConnectionStrings:FileStore:Container"];

            var filePath = $"{blobContainer}/{data.RecordId}.txt";

            var blockClient = this.cloudBlobClientResolver.GetCloudBlockBlobClient(filePath, blobConnectionString);

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
