using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using selective_archive_compressor.model;
using selective_archive_compressor.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public DirectoryNode? SelectedNode
        {
            get => m_SelectedNode;
            private set => SetProperty(ref m_SelectedNode, value);
        }

        public long SelectedNodeFileCount
        {
            get => m_SelectedNodeFileCount;
            private set => SetProperty(ref m_SelectedNodeFileCount, value);
        }

        public long SelectedNodeDirectoryCount
        {
            get => m_SelectedNodeDirectoryCount;
            private set => SetProperty(ref m_SelectedNodeDirectoryCount, value);
        }

        public long SelectedNodeTotalSize
        {
            get => m_SelectedNodeTotalSize;
            private set => SetProperty(ref m_SelectedNodeTotalSize, value);
        }

        public ObservableCollection<DirectoryNode> DirectoryTree => m_DirectoryTree;


        public IAsyncRelayCommand BrowseRootDirectoryCommand => m_BrowseRootDirectoryCommand;
        public IAsyncRelayCommand ScanCommand => m_ScanCommand;
        public IRelayCommand<DirectoryNode> ToggleCompressionCommand => m_ToggleCompressionCommand;
        public IRelayCommand<DirectoryNode> SelectItemCommand => m_SelectItemCommand;





        #endregion



        public MainViewModel(IFileService fileService, ILogService logService, IWindowService windowService)
        {
            m_FileService = fileService;
            m_LogService = logService;
            m_WindowService = windowService;

            m_DirectoryTree = new ObservableCollection<DirectoryNode>();

            m_ToggleCompressionCommand = new RelayCommand<DirectoryNode>(ToggleCompression);
            m_SelectItemCommand = new RelayCommand<DirectoryNode>(SelectItem);

            m_BrowseRootDirectoryCommand = new AsyncRelayCommand(BrowseRootDirectory, BrowseRootDirectoryCanExecute);
            m_ScanCommand = new AsyncRelayCommand(Scan, ScanCanExecute);

            m_RelayCommands = new List<IRelayCommand>();
            m_AsyncRelayCommands = new List<IAsyncRelayCommand> { m_BrowseRootDirectoryCommand, m_ScanCommand };
        }

        private void SelectItem(DirectoryNode? node)
        {
            SelectedNode = node;
        }

        private void ToggleCompression(DirectoryNode? node)
        {
            node?.MarkSelectedForCompression(!node.IsSelectedForCompression);
                
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

        private bool ScanCanExecute()
        {
            return !m_IsAnalyzing && !m_IsCompressing && !string.IsNullOrWhiteSpace(m_RootDirectoryPath);
        }

        private async Task Scan()
        {
            IsAnalyzing = true;
            DirectoryTree.Clear();
            DirectoryTree.Add(await m_FileService.CreateDirectoryTreeAsync(m_RootDirectoryPath));

            IsAnalyzing = false;
        }

        private static void PrintDirectoryTree(DirectoryNode node, int level = 0)
        {
            string indent = new(' ', level * 2);
            System.Diagnostics.Debug.WriteLine($"{indent}{node.Name}");
            foreach (var child in node.Children)
            {
                PrintDirectoryTree(child, level + 1);
            }
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
        DirectoryNode? m_SelectedNode;
        long m_SelectedNodeFileCount;
        long m_SelectedNodeDirectoryCount;
        long m_SelectedNodeTotalSize;

        readonly IFileService m_FileService;
        readonly ILogService m_LogService;
        readonly IWindowService m_WindowService;

        readonly ObservableCollection<FileItem> m_FileItems = new();
        readonly ObservableCollection<DirectoryNode> m_DirectoryTree;

        readonly IAsyncRelayCommand m_BrowseRootDirectoryCommand;
        readonly IAsyncRelayCommand m_ScanCommand;
        readonly IAsyncRelayCommand m_AnalyzeCommand;

        readonly IRelayCommand<DirectoryNode> m_ToggleCompressionCommand;
        readonly IRelayCommand<DirectoryNode> m_SelectItemCommand;

        readonly List<IRelayCommand> m_RelayCommands;
        readonly List<IAsyncRelayCommand> m_AsyncRelayCommands;


        #endregion
    }
}
