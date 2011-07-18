using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Converters
{
    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shell = (ShellObject) value;
            try
            {
                return shell.Thumbnail.MediumBitmapSource;
            } catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
