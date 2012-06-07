using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Kex.Controller;

namespace Kex.Views.Converters
{
    public class TabHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var header = value as string;
            var manager = ListerManager.Instance.ListerViewManager;
            var width = manager.CurrentListerView.View.ActualWidth;
            var count = manager.Listers.Count;
            if (count == 1)
                return header;
            var widthPerLister = width/count;
            var index = Math.Min(0, (int)(header.Length*8 - widthPerLister));
            return header.Substring(index);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
