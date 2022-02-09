using MarineTask.ValidationApp.Validation.ValidationContext;

namespace MarineTask.ValidationApp.Validation
{
    public interface IRecordValidator
    {
        ValidationResult Validate(RecordValidationContext command);
    }
}
