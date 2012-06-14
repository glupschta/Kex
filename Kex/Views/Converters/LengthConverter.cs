using System;
using System.Globalization;
using System.Windows.Data;

namespace Kex.Views.Converters
{
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (long) value;
            return (length / 1000) + " KB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LengthConverterImpl : LengthConverter
    {
    }
}
