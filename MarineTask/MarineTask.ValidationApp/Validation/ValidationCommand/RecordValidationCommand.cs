using System.Collections.Generic;

namespace MarineTask.ValidationApp.Validation.ValidationCommand
{
    public class RecordValidationCommand
    {
        public string RecordId { get; set; }

        public List<string> ExpectedSequence { get; set; }

        public List<string> ActualSequence { get; set; }
    }
}
