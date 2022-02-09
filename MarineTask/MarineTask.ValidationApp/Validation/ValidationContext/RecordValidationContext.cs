using System.Collections.Generic;

namespace MarineTask.ValidationApp.Validation.ValidationContext
{
    public class RecordValidationContext
    {
        public string RecordId { get; set; }

        public List<string> ExpectedSequence { get; set; }

        public List<string> ActualSequence { get; set; }
    }
}
