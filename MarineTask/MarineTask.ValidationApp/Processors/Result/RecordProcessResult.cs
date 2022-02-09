using System.Collections.Generic;

namespace MarineTask.ValidationApp.Processors.Result
{
    public class RecordProcessResult
    {
        public string RecordUrl { get; set; }

        public string ValidatedRecordId { get; set; }

        public List<string> MissedSequences { get; set; }
    }
}
