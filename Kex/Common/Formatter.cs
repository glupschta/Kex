using System;

namespace Kex.Common
{
    public class Formatter
    {
        public static string FormatLength(long length)
        {
            return (length / 1000) + " KB";
        }

        public static string FormatDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToShortDateString()+" "+date.Value.ToLongTimeString() : "";
        }
    }
}
