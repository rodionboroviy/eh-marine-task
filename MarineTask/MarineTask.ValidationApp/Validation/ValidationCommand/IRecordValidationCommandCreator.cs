namespace MarineTask.ValidationApp.Validation.ValidationCommand
{
    internal interface IRecordValidationCommandCreator
    {
        RecordValidationCommand CreateCommand(RecordValidationCommandData data);
    }
}
