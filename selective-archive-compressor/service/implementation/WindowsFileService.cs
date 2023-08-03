using selective_archive_compressor.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace selective_archive_compressor.service.implementation
{
    internal class WindowsFileService : IFileService
    {
        public async Task<CompressionResult> CompressDirectoryTreeAsync(DirectoryNode directoryNode, string outputDirectoryPath, CompressionType compressionType, int compressionLevel)
        {
            CompressionResult result = new();
            

            try
            {
                Directory.CreateDirectory(outputDirectoryPath);

                if (directoryNode.IsSelectedForCompression)
                {
                    string ext = compressionType switch
                    {
                        CompressionType.Zip => "zip",
                        CompressionType.SevenZip => "7z",
                        _ => throw new NotImplementedException()
                    };

                    string outputFilePath = Path.Combine(outputDirectoryPath, $"{directoryNode.Name}.{ext}");
                    if (File.Exists(outputFilePath))
                        File.Delete(outputFilePath);

                    switch (compressionType)
                    {
                        case CompressionType.Zip:
                            CompressionLevel level = CompressionLevel.SmallestSize;
                            await Task.Run(() => ZipFile.CreateFromDirectory(directoryNode.FullPath, outputFilePath, level, false));
                            break;
                        case CompressionType.SevenZip:
                            SharpCompress.
                            SevenZip.CompressionLevel level7z = SevenZip.CompressionLevel.Ultra;
                            SevenZip.SevenZipCompressor compressor = new()
                            {
                                CompressionLevel = level7z,
                                CompressionMethod = SevenZip.CompressionMethod.Lzma2,
                                CompressionMode = SevenZip.CompressionMode.Create,
                                ArchiveFormat = SevenZip.OutArchiveFormat.SevenZip
                            };
                            await compressor.CompressDirectoryAsync(directoryNode.FullPath, outputFilePath);
                            break;
                    }
                }
                else
                {

                }
                result.ResultStatus = true;
            }
            catch (Exception ex)
            {
                result.ResultStatus = false;
                result.ErrorList = new List<string>() { ex.Message };
            }

            return result;
        }

        public async Task<DirectoryNode> CreateDirectoryTreeAsync(string rootDirectoryPath)
        {
            DirectoryInfo directoryInfo = new(rootDirectoryPath);
            DirectoryNode node = new(directoryInfo.Name, rootDirectoryPath);

            List<Task<DirectoryNode>> tasks = new();
            try
            {
                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                foreach (var subDirectory in subDirectories)
                {
                    tasks.Add(Task.Run(() => CreateDirectoryTreeAsync(subDirectory.FullName)));
                }

                await Task.WhenAll(tasks);
                foreach (var task in tasks)
                {
                    node.Children.Add(await task);
                }
            }
            catch (UnauthorizedAccessException)
            {
                //skip
            }
            return node;
        }
    }
}
