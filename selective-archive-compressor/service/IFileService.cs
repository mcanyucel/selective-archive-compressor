using selective_archive_compressor.model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace selective_archive_compressor.service
{
    internal interface IFileService
    {
        Task<IEnumerable<FileItem>> GetFilesAsync(string rootDirectoryPath);

        Task<DirectoryNode> CreateDirectoryTreeAsync(string rootDirectoryPath);
    }
}
