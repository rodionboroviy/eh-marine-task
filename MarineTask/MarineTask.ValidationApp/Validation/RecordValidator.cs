using MarineTask.ValidationApp.Validation.ValidationContext;
using System.Collections.Generic;
using System.Linq;

namespace MarineTask.ValidationApp.Validation
{
    public class RecordValidator : IRecordValidator
    {
        public ValidationResult Validate(RecordValidationContext command)
        {
            if (command.ExpectedSequence.Count == 0)
            {
                return new ValidationResult(new List<string>(0));
            }

            var missedSequences = command.ExpectedSequence.Except(command.ActualSequence).ToList();

            return new ValidationResult(missedSequences);
        }
    }
}
