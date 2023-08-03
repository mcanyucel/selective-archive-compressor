using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
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

        public bool IsSelectedForCompression
        {
            get => m_IsSelectedForCompression;
            private set => SetProperty(ref m_IsSelectedForCompression, value);
        }

        public long Size
        {
            get => m_Size;
            private set => SetProperty(ref m_Size, value);
        }

        public ObservableCollection<DirectoryNode> Children
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

        public void MarkSelectedForCompression(bool isSelectedForCompression)
        {
            IsSelectedForCompression = isSelectedForCompression;
            // we do not want to compress the subdirectories if the parent directory is compressed
            foreach (var child in Children)
                child.MarkSelectedForCompression(false);
            // this does not prevent selecting parent first and then child. but it is not a problem.
        }

        /// <summary>
        /// Calculates the total size of the files in the directory. It does not include the subdirectories.
        /// </summary>
        /// <returns></returns>
        public async Task<long> CalculateDirectorySizeAsync()
        {
            long size = 0;
            // current directory
            DirectoryInfo directoryInfo = new(Name);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var file in files)
                size += file.Length;
            //children
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            var tasks = new Task<long>[directories.Length];
            for (int i = 0; i < directories.Length; i++)
                tasks[i] = Task.Run(() => new DirectoryNode(directories[i].FullName).CalculateDirectorySizeAsync());

            await Task.WhenAll(tasks);
            foreach (var task in tasks)
                size += task.Result;

            return size;
        }
        #endregion

        #region Fields
        string m_Name = string.Empty;
        bool m_IsSelected = false;
        bool m_IsExpanded = false;
        bool m_IsSelectedForCompression = false;
        long m_Size = 0;
        ObservableCollection<DirectoryNode> m_Children = new();
        #endregion
    }
}
