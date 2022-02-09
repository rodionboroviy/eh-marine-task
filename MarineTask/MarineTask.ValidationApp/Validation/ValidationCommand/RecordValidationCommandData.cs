using System.Collections.Generic;

namespace MarineTask.ValidationApp.Validation.ValidationCommand
{
    internal class RecordValidationCommandData
    {
        public string RecordId { get; set; }

        public List<string> Inventories { get; set; }
    }
}
