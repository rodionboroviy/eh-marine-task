using MarineTask.ValidationApp.Extensions;
using MarineTask.ValidationApp.Processors.Result;
using System.Collections.Generic;

namespace MarineTask.ValidationApp.Processors
{
    internal class InventoryLineProcessor : IRecordLineProcessor<InventoryResult>
    {
        private string inventoryRecord; 
        private List<string> inventory;

        public InventoryLineProcessor()
        {
            this.inventory = new List<string>();
        }

        public void ProcessLine(string line)
        {
            if (line.IsInventoryId())
            {
                this.inventoryRecord = line.Replace("Inventory:", string.Empty).Trim();
            }

            if (!line.IsRecordId() && !line.IsInventoryId() && !line.IsSequenceLine())
            {
                this.inventory.Add(line);
            }
        }

        public ProcessResult<InventoryResult> GetResult()
        {
            return new ProcessResult<InventoryResult>
            {
                Result = new InventoryResult
                {
                    InventoryId = this.inventoryRecord,
                    Inventories = this.inventory
                }
            };
        }
    }
}
