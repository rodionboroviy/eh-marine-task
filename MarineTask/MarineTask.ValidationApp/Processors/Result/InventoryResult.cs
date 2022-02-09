using System.Collections.Generic;

namespace MarineTask.ValidationApp.Processors.Result
{
    public class InventoryResult
    {
        public string InventoryId { get; set; }

        public List<string> Inventories { get; set; }
    }
}
