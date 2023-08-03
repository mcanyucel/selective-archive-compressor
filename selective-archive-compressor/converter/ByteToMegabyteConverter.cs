using System;
using System.Globalization;
using System.Windows.Data;

namespace selective_archive_compressor.converter
{
    internal class ByteToMegabyteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                var mb = bytes >> 20;
                if (mb > 1000)
                    return $"{mb >> 10} GB";
                else if (mb > 0)
                    return $"{mb} MB";
                else if (mb == 0)
                    return $"{bytes >> 10} KB";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
