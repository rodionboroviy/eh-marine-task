using System.Collections.Generic;

namespace MarineTask.ValidationApp.Validation
{
    internal interface IInventorySequenceGenerator
    {
        List<string> GenerateSequence(List<string> inventories);
    }
}
