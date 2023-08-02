using System.Threading.Tasks;

namespace selective_archive_compressor.service
{
    internal interface IWindowService
    {
        Task<string?> ShowFolderPickerAsync(string? initialDirectoryPath = null);
        Task<string?> ShowFilePickerAsync(string? initialDirectoryPath = null);
        Task<string?> ShowSaveFileDialogAsync(string? initialDirectoryPath = null);
    }
}
