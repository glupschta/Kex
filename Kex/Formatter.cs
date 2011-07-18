using System;

namespace Kex
{
    public class Formatter
    {
        public static string FormatLength(long length)
        {
            if (length > 1000000)
            {
                return (length / 1000000) + " MB";
            }
            if (length > 1000)
            {
                return (length / 1000) + " KB";
            }
            return length.ToString();
        }

        public static string FormatDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToShortDateString()+" "+date.Value.ToLongTimeString() : "";
        }
    }
}
