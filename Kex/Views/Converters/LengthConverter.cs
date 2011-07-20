﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Kex.Views.Converters
{
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (long) value;
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LengthConverterImpl : LengthConverter
    {
    }
}