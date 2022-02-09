﻿using MarineTask.ValidationApp.Processors.Result;
using MarineTask.ValidationApp.Validation;
using MarineTask.ValidationApp.Validation.ValidationCommand;

namespace MarineTask.ValidationApp.Processors
{
    internal class RecordProcessor : IRecordLineProcessor<RecordProcessResult>
    {
        private readonly IRecordLineProcessor<SequenceResult> sequenceProcessor;
        private readonly IRecordLineProcessor<InventoryResult> inventoryProcessor;
        private readonly IRecordValidationCommandCreator recordValidationCommandCreator;
        private readonly IRecordValidator recordValidator;

        public RecordProcessor(
            IRecordLineProcessor<SequenceResult> sequenceProcessor,
            IRecordLineProcessor<InventoryResult> inventoryProcessor,
            IRecordValidationCommandCreator recordValidationCommandCreator,
            IRecordValidator recordValidator)
        {
            this.sequenceProcessor = sequenceProcessor;
            this.inventoryProcessor = inventoryProcessor;
            this.recordValidationCommandCreator = recordValidationCommandCreator;
            this.recordValidator = recordValidator;
        }

        public void ProcessLine(string line)
        {
            this.sequenceProcessor.ProcessLine(line);
            this.inventoryProcessor.ProcessLine(line);
        }

        public ProcessResult<RecordProcessResult> GetResult()
        {
            var recordSequenceResult = this.sequenceProcessor.GetResult();
            var inventoryResult = this.inventoryProcessor.GetResult();

            var validationCommand = this.recordValidationCommandCreator.CreateCommand(
                new RecordValidationCommandData
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
