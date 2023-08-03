using selective_archive_compressor.model;
using System.Threading.Tasks;

namespace selective_archive_compressor.service
{
    internal interface IFileService
    {
        Task<DirectoryNode> CreateDirectoryTreeAsync(string rootDirectoryPath);
        Task<CompressionResult> CompressDirectoryTreeAsync(DirectoryNode directoryNode, string outputDirectoryPath, CompressionType compressionType, int compressionLevel);
    }
}
