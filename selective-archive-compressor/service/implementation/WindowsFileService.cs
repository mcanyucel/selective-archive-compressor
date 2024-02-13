using selective_archive_compressor.model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace selective_archive_compressor.service.implementation
{
    internal class WindowsFileService : IFileService
    {
        public async Task<IEnumerable<FileItem>> GetFilesAsync(string rootDirectoryPath)
        {
            if (string.IsNullOrEmpty(rootDirectoryPath))
                throw new ArgumentNullException(nameof(rootDirectoryPath));

            if (!Directory.Exists(rootDirectoryPath))
                throw new DirectoryNotFoundException(rootDirectoryPath);


            ConcurrentDictionary<string, DirectoryInfo> directories = new();
            await TraverseDirectoriesAsync(rootDirectoryPath, directories);

            foreach (var directory in directories)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"{directory.Key} - {directory.Value.FullName}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return new List<FileItem>();
        }

        static async Task TraverseDirectoriesAsync(string folderPath, ConcurrentDictionary<string, DirectoryInfo> result)
        {
            DirectoryInfo dirInfo = new(folderPath);
            result.TryAdd(dirInfo.FullName, dirInfo);

            DirectoryInfo[] subDirectories = dirInfo.GetDirectories();
            var tasks = new List<Task>();
            foreach (var subDirectory in subDirectories)
            {
                tasks.Add(Task.Run(() => TraverseDirectoriesAsync(subDirectory.FullName, result)));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<DirectoryNode> CreateDirectoryTreeAsync(string rootDirectoryPath)
        {
            List<Task<DirectoryNode>> tasks = new();
            DirectoryInfo directoryInfo = new(rootDirectoryPath);
            DirectoryNode node = new(directoryInfo.Name, rootDirectoryPath);
            try
            {


                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                foreach (var subDirectory in subDirectories)
                {
                    tasks.Add(Task.Run(() => CreateDirectoryTreeAsync(subDirectory.FullName)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                node.Children.Add(await task);
            }
            return node;
        }
    }
}
