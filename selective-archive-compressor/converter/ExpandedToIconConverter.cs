using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows.Data;

namespace selective_archive_compressor.converter
{
    internal class ExpandedToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool expanded)
                return expanded ? PackIconMaterialKind.FolderOpen : PackIconMaterialKind.Folder;
            else return PackIconMaterialKind.ChatQuestion;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
