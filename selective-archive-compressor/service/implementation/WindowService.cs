using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selective_archive_compressor.service.implementation
{
    internal class WindowService : IWindowService
    {
        public Task<string?> ShowFilePickerAsync(string? initialDirectoryPath = null)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ShowFolderPickerAsync(string? initialDirectoryPath = null)
        {
            return await Task.Run(() =>
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
                {
                    SelectedPath = initialDirectoryPath
                };
                if (dialog.ShowDialog() == true)
                    return dialog.SelectedPath;
                else
                    return null;
            });
        }

        public Task<string?> ShowSaveFileDialogAsync(string? initialDirectoryPath = null)
        {
            throw new NotImplementedException();
        }
    }
}
