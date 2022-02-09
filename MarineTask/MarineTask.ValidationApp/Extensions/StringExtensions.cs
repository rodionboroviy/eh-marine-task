using System;
using System.Collections.Generic;
using System.Text;

namespace MarineTask.ValidationApp.Extensions
{
    public static class StringExtensions
    {
        public static bool IsRecordId(this string value)
        {
            return value.StartsWith("RecordID:");
        }

        public static bool IsSequenceLine(this string value)
        {
            return value.Contains("Sequence");
        }

        public static bool IsInventoryId(this string value)
        {
            return value.StartsWith("Inventory:");
        }
    }
}
