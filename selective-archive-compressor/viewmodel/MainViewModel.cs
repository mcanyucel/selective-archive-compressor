using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using selective_archive_compressor.model;
using selective_archive_compressor.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace selective_archive_compressor.viewmodel
{
    internal class MainViewModel : ObservableObject
    {

        #region Properties
        public bool IsAnalyzing
        {
            get => m_IsAnalyzing;
            private set { SetProperty(ref m_IsAnalyzing, value); NotifyCommands(); }
        }

        public bool IsCompressing
        {
            get => m_IsCompressing;
            private set { SetProperty(ref m_IsCompressing, value); NotifyCommands(); }
        }

        public string RootDirectoryPath
        {
            get => m_RootDirectoryPath;
            set { SetProperty(ref m_RootDirectoryPath, value); NotifyCommands(); }
        }

        public string OutputDirectoryPath
        {
            get => m_OutputDirectoryPath;
            set { SetProperty(ref m_OutputDirectoryPath, value); NotifyCommands(); }
        }

        public long FileCount
        {
            get => m_FileCount;
            private set => SetProperty(ref m_FileCount, value);
        }

        public long DirectoryCount
        {
            get => m_DirectoryCount;
            private set => SetProperty(ref m_DirectoryCount, value);
        }

        public long TotalSize
        {
            get => m_TotalSize;
            private set => SetProperty(ref m_TotalSize, value);
        }

        public CompressionType CompressionType
        {
            get => m_CompressionType;
            set => SetProperty(ref m_CompressionType, value);
        }

        public int CompressionLevel
        {
            get => m_CompressionLevel;
            set => SetProperty(ref m_CompressionLevel, value);
        }

        public ObservableCollection<FileItem> FileItems => m_FileItems;

        public long TotalItemCount
        {
            get => m_TotalItemCount;
            private set => SetProperty(ref m_TotalItemCount, value);
        }

        public long ProcessedItemCount
        {
            get => m_ProcessedItemCount;
            private set => SetProperty(ref m_ProcessedItemCount, value);
        }

        public IAsyncRelayCommand BrowseRootDirectoryCommand => m_BrowseRootDirectoryCommand;
        public IAsyncRelayCommand AnalyzeCommand => m_AnalyzeCommand;



        #endregion



        public MainViewModel(IFileService fileService, ILogService logService, IWindowService windowService)
        {
            m_FileService = fileService;
            m_LogService = logService;
            m_WindowService = windowService;

            m_BrowseRootDirectoryCommand = new AsyncRelayCommand(BrowseRootDirectory, BrowseRootDirectoryCanExecute);
            m_AnalyzeCommand = new AsyncRelayCommand(Analyze, AnalyzeCanExecute);

            m_RelayCommands = new List<IRelayCommand>();
            m_AsyncRelayCommands = new List<IAsyncRelayCommand> { m_BrowseRootDirectoryCommand, m_AnalyzeCommand };
        }

        private bool BrowseRootDirectoryCanExecute()
        {
            return !m_IsAnalyzing && !m_IsCompressing;
        }

        private async Task BrowseRootDirectory()
        {
            var result = await m_WindowService.ShowFolderPickerAsync(m_RootDirectoryPath);
            if (result != null) RootDirectoryPath = result;
        }

        private bool AnalyzeCanExecute()
        {
            return !m_IsAnalyzing && !m_IsCompressing && !string.IsNullOrWhiteSpace(m_RootDirectoryPath);
        }

        private async Task Analyze()
        {
            IsAnalyzing = true;

            await Task.Delay(2000);

            IsAnalyzing = false;
        }




        void NotifyCommands()
        {
            foreach (var command in m_RelayCommands)
            {
                command.NotifyCanExecuteChanged();
            }

            foreach (var command in m_AsyncRelayCommands)
            {
                command.NotifyCanExecuteChanged();
            }
        }



        #region Fields

        bool m_IsAnalyzing;
        bool m_IsCompressing;
        string m_RootDirectoryPath = string.Empty;
        string m_OutputDirectoryPath = string.Empty;
        long m_FileCount;
        long m_DirectoryCount;
        long m_TotalItemCount;
        long m_ProcessedItemCount;
        long m_TotalSize; // MB
        CompressionType m_CompressionType = CompressionType.SevenZip;
        int m_CompressionLevel = 9;

        readonly IFileService m_FileService;
        readonly ILogService m_LogService;
        readonly IWindowService m_WindowService;

        readonly ObservableCollection<FileItem> m_FileItems = new();

        readonly IAsyncRelayCommand m_BrowseRootDirectoryCommand;
        readonly IAsyncRelayCommand m_AnalyzeCommand;

        readonly List<IRelayCommand> m_RelayCommands;
        readonly List<IAsyncRelayCommand> m_AsyncRelayCommands;


        #endregion
    }
}
