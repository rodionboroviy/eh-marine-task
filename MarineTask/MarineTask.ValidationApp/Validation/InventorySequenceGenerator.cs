using System.Collections.Generic;
using System.Linq;

namespace MarineTask.ValidationApp.Validation
{
    internal class InventorySequenceGenerator : IInventorySequenceGenerator
    {
        public List<string> GenerateSequence(List<string> inventories)
        {
            var result = new List<string>(0);

            foreach (string inventoryItem in inventories)
            {
                var parts = inventoryItem.Split(':');

                var sequenceType = parts[0].Trim();
                var highestSequence = int.Parse(parts[1]);

                result.AddRange(
                    Enumerable.Range(0, highestSequence + 1).Select(i => $"{sequenceType} – Sequence {i}").ToList()
                );
            }

            return result;
        }
    }
}
