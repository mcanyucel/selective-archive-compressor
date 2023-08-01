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
        public async Task<IEnumerable<FileItem>> GetFilesAsync(string rootDirectoryPath, ref ConcurrentBag<FileItem> fileItems)
        {
            if (string.IsNullOrEmpty(rootDirectoryPath))
                throw new ArgumentNullException(nameof(rootDirectoryPath));

            if (!Directory.Exists(rootDirectoryPath))
                throw new DirectoryNotFoundException(rootDirectoryPath);



                                   
        }

        private async Task<>
    }
}
