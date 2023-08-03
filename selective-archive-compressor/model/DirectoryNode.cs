using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

        public string FullPath
        {
            get => m_FullPath;
            private set => SetProperty(ref m_FullPath, value);
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

        public DirectoryData? DirectoryData
        {
            get => m_DirectoryData;
            private set => SetProperty(ref m_DirectoryData, value);
        }



        public ObservableCollection<DirectoryNode> Children
        {
            get => m_Children;
            private set => SetProperty(ref m_Children, value);
        }
        #endregion

        public DirectoryNode(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
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
        public async Task<DirectoryData> CalculateDirectorySizeAsync()
        {
            if (DirectoryData != null)
                return DirectoryData;


            DirectoryData directoryData = new();
            try
            {
                // current directory
                DirectoryInfo directoryInfo = new(FullPath);

                FileInfo[] files = directoryInfo.GetFiles();
                foreach (var file in files)
                    directoryData.Size += file.Length;

                directoryData.FileCount += files.Length;

                //children
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                directoryData.DirectoryCount += directories.Length;
                var tasks = new List<Task<DirectoryData>>(directories.Length);
                foreach (var directory in directories)
                    tasks.Add(Task.Run(() => new DirectoryNode(directory.Name, directory.FullName).CalculateDirectorySizeAsync()));

                await Task.WhenAll(tasks);
                foreach (var task in tasks)
                {
                    directoryData.Size += task.Result.Size;
                    directoryData.FileCount += task.Result.FileCount;
                    directoryData.DirectoryCount += task.Result.DirectoryCount;
                }

                DirectoryData = directoryData;
            }
            catch (UnauthorizedAccessException)
            {
                //skip
            }
            return directoryData;
        }
        #endregion

        #region Fields
        string m_Name = string.Empty;
        bool m_IsSelected = false;
        bool m_IsExpanded = false;
        bool m_IsSelectedForCompression = false;
        DirectoryData? m_DirectoryData; // null if not calculated
        string m_FullPath = string.Empty;
        ObservableCollection<DirectoryNode> m_Children = new();
        #endregion
    }
}
