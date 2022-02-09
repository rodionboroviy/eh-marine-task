using System.Threading.Tasks;

namespace MarineTask.ValidationApp.Validation.ValidationContext
{
    internal interface IRecordValidationContextCreator
    {
        Task<RecordValidationContext> CreateCommand(RecordValidationContextData data);
    }
}
