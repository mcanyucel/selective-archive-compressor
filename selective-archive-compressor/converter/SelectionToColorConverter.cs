using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace selective_archive_compressor.converter
{
    internal class SelectionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool selected)
                return selected ? Colors.Red : Colors.Black;
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
