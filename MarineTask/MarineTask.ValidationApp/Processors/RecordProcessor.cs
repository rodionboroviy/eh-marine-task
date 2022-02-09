using MarineTask.ValidationApp.Processors.Result;
using MarineTask.ValidationApp.Validation;
using MarineTask.ValidationApp.Validation.ValidationContext;
using System.Threading.Tasks;

namespace MarineTask.ValidationApp.Processors
{
    internal class RecordProcessor : IRecordLineProcessor<RecordProcessResult>
    {
        private readonly IRecordLineProcessor<SequenceResult> sequenceProcessor;
        private readonly IRecordLineProcessor<InventoryResult> inventoryProcessor;
        private readonly IRecordValidationContextCreator recordValidationCommandCreator;
        private readonly IRecordValidator recordValidator;

        public RecordProcessor(
            IRecordLineProcessor<SequenceResult> sequenceProcessor,
            IRecordLineProcessor<InventoryResult> inventoryProcessor,
            IRecordValidationContextCreator recordValidationCommandCreator,
            IRecordValidator recordValidator)
        {
            this.sequenceProcessor = sequenceProcessor;
            this.inventoryProcessor = inventoryProcessor;
            this.recordValidationCommandCreator = recordValidationCommandCreator;
            this.recordValidator = recordValidator;
        }

        public async Task ProcessLine(string line)
        {
            await this.sequenceProcessor.ProcessLine(line);
            await this.inventoryProcessor.ProcessLine(line);
        }

        public async Task<ProcessResult<RecordProcessResult>> GetResult()
        {
            var recordSequenceResult = await this.sequenceProcessor.GetResult();
            var inventoryResult = await this.inventoryProcessor.GetResult();

            var validationCommand = await this.recordValidationCommandCreator.CreateCommand(
                new RecordValidationContextData
                {
                    RecordId = inventoryResult.Result.InventoryId,
                    Inventories = inventoryResult.Result.Inventories,
                }
            );

            var result = this.recordValidator.Validate(validationCommand);

            return new ProcessResult<RecordProcessResult>
            {
                Result = new RecordProcessResult
                {
                    RecordUrl = recordSequenceResult.Result.FileUrl,
                    ValidatedRecordId = inventoryResult.Result.InventoryId,
                    MissedSequences = result.MissedSequences
                }
            };
        }
    }
}
