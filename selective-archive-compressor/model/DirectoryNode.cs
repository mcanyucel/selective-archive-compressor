using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace selective_archive_compressor.model
{
    public partial class DirectoryNode : ObservableObject
    {
        #region Properties

        [ObservableProperty]
        string name = string.Empty;

        [ObservableProperty]
        bool isSelected = false;

        [ObservableProperty]
        bool isExpanded = false;

        [ObservableProperty]
        bool isSelectedForCompression = false;

        [ObservableProperty]
        DirectoryData? directoryData; // null if not calculated

        [ObservableProperty]
        string fullPath = string.Empty;

        [ObservableProperty]
        ObservableCollection<DirectoryNode> children = [];
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
            // we do not want to compress the subdirectories if the parent directory is already compressed
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            // convert to MB
            DirectoryData = directoryData;
            return directoryData;
        }
        #endregion

        #region Fields

        #endregion
    }
}
