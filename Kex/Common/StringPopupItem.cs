using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kex.Common
{
    public class StringPopupItem : IPopupItem
    {
        public StringPopupItem(string stringInternal)
        {
            StringInternal = stringInternal;
        }

        private readonly string StringInternal;

        public string DisplayName
        {
            get { return StringInternal; }
        }

        public string FilterString
        {
            get { return StringInternal; }
        }

        public BitmapSource Thumbnail { get; set; }

        public static IEnumerable<StringPopupItem> GetEnumerable(IEnumerable<string> items)
        {
            return items.Select(i => new StringPopupItem(i));
        }
    }
}
