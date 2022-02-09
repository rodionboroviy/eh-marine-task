using MarineTask.ValidationApp.Validation.ValidationCommand;

namespace MarineTask.ValidationApp.Validation
{
    public interface IRecordValidator
    {
        ValidationResult Validate(RecordValidationCommand command);
    }
}
