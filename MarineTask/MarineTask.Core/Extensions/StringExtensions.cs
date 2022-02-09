using System;
using System.Text;

namespace MarineTask.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(valueBytes);
        }

        public static string FromBase64(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
