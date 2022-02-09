using System.Collections.Generic;

namespace MarineTask.ValidationApp.Validation
{
    public class ValidationResult
    {
        public ValidationResult(List<string> missedSequences)
        {
            MissedSequences = missedSequences;
        }

        public bool IsValid => MissedSequences.Count == 0;
        public List<string> MissedSequences { get; private set; }
    }
}
