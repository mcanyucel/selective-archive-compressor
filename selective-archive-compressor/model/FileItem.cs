using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace selective_archive_compressor.model
{
    public partial class FileItem : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string path = string.Empty;

        [ObservableProperty]
        private bool isDirectory;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private long size;

        [ObservableProperty]
        private IEnumerable<FileItem>? children;
    }
}
