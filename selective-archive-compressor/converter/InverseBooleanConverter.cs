using System;
using System.Windows.Data;

namespace selective_archive_compressor.converter
{
    internal class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("InverseBooleanConverter can only be used OneWay.");
        }
    }
}
