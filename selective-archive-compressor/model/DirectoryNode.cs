using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace selective_archive_compressor.model
{
    internal class DirectoryNode : ObservableObject
    {
        #region Properties
        public string Name
        {
            get => m_Name;
            private set => SetProperty(ref m_Name, value);
        }

        public bool IsSelected
        {
            get => m_IsSelected;
            set => SetProperty(ref m_IsSelected, value);
        }

        public bool IsExpanded
        {
            get => m_IsExpanded;
            set => SetProperty(ref m_IsExpanded, value);
        }

        public bool IsCompressed
        {
            get => m_IsCompressed;
            set => SetProperty(ref m_IsCompressed, value);
        }

        public bool IsCompressing
        {
            get => m_IsCompressing;
            set => SetProperty(ref m_IsCompressing, value);
        }

        public bool IsSelectedForCompression
        {
            get => m_IsSelectedForCompression;
            set => SetProperty(ref m_IsSelectedForCompression, value);
        }

        public long Size
        {
            get => m_Size;
            private set => SetProperty(ref m_Size, value);
        }

        public List<DirectoryNode> Children
        {
            get => m_Children;
            private set => SetProperty(ref m_Children, value);
        }
        #endregion

        public DirectoryNode(string name)
        {
            Name = name;
        }

        #region Methods
        public async Task<long> CalculateSize()
        {
            return await Task.Run(async () =>
            {
                Size = await CalculateFileSize();
                foreach (var child in Children)
                    Size += await child.CalculateSize();
                return Size;
            });
        }

        /// <summary>
        /// Calculates the total size of the files in the directory. It does not include the subdirectories.
        /// </summary>
        /// <returns></returns>
        static async Task<long> CalculateFileSize()
        {
            await Task.Delay(1000);
            return 42;

        }
        #endregion

        #region Fields
        string m_Name = string.Empty;
        bool m_IsSelected = false;
        bool m_IsExpanded = false;
        bool m_IsCompressed = false;
        bool m_IsCompressing = false;
        bool m_IsSelectedForCompression = false;
        long m_Size = 0;
        List<DirectoryNode> m_Children = new();
        #endregion
    }
}
