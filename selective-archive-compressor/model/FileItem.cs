using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace selective_archive_compressor.model
{
    internal class FileItem : ObservableObject
    {
        private string m_Name = string.Empty;
        private string m_Path = string.Empty;
        private bool m_IsDirectory;
        private bool m_IsSelected;
        private long m_Size;
        private IEnumerable<FileItem>? m_Children;

        public string Name
        {
            get => m_Name;
            set => SetProperty(ref m_Name, value);
        }

        public string Path
        {
            get => m_Path;
            set => SetProperty(ref m_Path, value);
        }

        public bool IsDirectory
        {
            get => m_IsDirectory;
            set => SetProperty(ref m_IsDirectory, value);
        }

        public bool IsSelected
        {
            get => m_IsSelected;
            set => SetProperty(ref m_IsSelected, value);
        }

        public long Size
        {
            get => m_Size;
            set => SetProperty(ref m_Size, value);
        }

        public IEnumerable<FileItem>? Children
        {
            get => m_Children;
            set => SetProperty(ref m_Children, value);
        }
    }
}
