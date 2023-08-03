using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using selective_archive_compressor.model;
using selective_archive_compressor.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public DirectoryNode? SelectedNode
        {
            get => m_SelectedNode;
            private set { SetProperty(ref m_SelectedNode, value); NotifyCommands(); }
        }

        public string StatusText
        {
            get => m_StatusText;
            private set => SetProperty(ref m_StatusText, value);
        }

        public static List<CompressionType> CompressionTypes => Enum.GetValues(typeof(CompressionType)).Cast<CompressionType>().ToList();
        public static List<int> CompressionLevels => Enumerable.Range(0, 10).ToList();
        public ObservableCollection<DirectoryNode> DirectoryTree => m_DirectoryTree;
        public IAsyncRelayCommand BrowseRootDirectoryCommand => m_BrowseRootDirectoryCommand;
        public IAsyncRelayCommand BrowseOutputDirectoryCommand => m_BrowseOutputDirectoryCommand;
        public IAsyncRelayCommand CompressCommand => m_CompressCommand;
        public IAsyncRelayCommand ScanCommand => m_ScanCommand;
        public IAsyncRelayCommand AnalyzeCommand => m_AnalyzeCommand;
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
            m_BrowseOutputDirectoryCommand = new AsyncRelayCommand(BrowseOutputDirectory, BrowseOutputDirectoryCanExecute);
            m_ScanCommand = new AsyncRelayCommand(Scan, ScanCanExecute);
            m_AnalyzeCommand = new AsyncRelayCommand(Analyze, AnalyzeCanExecute);
            m_CompressCommand = new AsyncRelayCommand(Compress, CompressCanExecute);

            m_RelayCommands = new List<IRelayCommand>();
            m_AsyncRelayCommands = new List<IAsyncRelayCommand> { m_BrowseRootDirectoryCommand, m_ScanCommand, m_AnalyzeCommand, m_BrowseOutputDirectoryCommand, m_CompressCommand };
        }

        private bool CompressCanExecute() => !m_IsAnalyzing && !m_IsCompressing;

        private async Task Compress()
        {
            IsCompressing = true;
            SetStatusText("Compressing...");

            await m_FileService.CompressDirectoryTreeAsync(DirectoryTree.First(), OutputDirectoryPath, CompressionType, CompressionLevel);

            IsCompressing = false;
            SetStatusText();
        }

        private bool BrowseOutputDirectoryCanExecute() => !m_IsAnalyzing && !m_IsCompressing;

        private async Task BrowseOutputDirectory()
        {
            var result = await m_WindowService.ShowFolderPickerAsync(m_OutputDirectoryPath);
            if (result != null) OutputDirectoryPath = $"{result}/archive";
        }

        private bool AnalyzeCanExecute() => !m_IsAnalyzing && !m_IsCompressing && m_SelectedNode != null;

        private async Task Analyze()
        {
            SetStatusText($"Analyzing {SelectedNode!.Name}...");
            IsAnalyzing = true;
            _ = await SelectedNode!.CalculateDirectorySizeAsync();
            IsAnalyzing = false;
            SetStatusText();
        }

        private void SelectItem(DirectoryNode? node) => SelectedNode = node;

        private void ToggleCompression(DirectoryNode? node) => node?.MarkSelectedForCompression(!node.IsSelectedForCompression);

        private bool BrowseRootDirectoryCanExecute() => !m_IsAnalyzing && !m_IsCompressing;

        private async Task BrowseRootDirectory()
        {
            var result = await m_WindowService.ShowFolderPickerAsync(m_RootDirectoryPath);
            if (result != null) RootDirectoryPath = result;
        }

        private bool ScanCanExecute()
        => !m_IsAnalyzing && !m_IsCompressing && !string.IsNullOrWhiteSpace(m_RootDirectoryPath);

        private async Task Scan()
        {
            SetStatusText("Scanning the directory tree...");
            IsAnalyzing = true;
            DirectoryTree.Clear();
            DirectoryTree.Add(await m_FileService.CreateDirectoryTreeAsync(m_RootDirectoryPath));
            IsAnalyzing = false;
            SetStatusText();
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

        void SetStatusText(string? message = null) => StatusText = message ?? "Idle";

        #region Fields
        bool m_IsAnalyzing;
        bool m_IsCompressing;
        string m_RootDirectoryPath = string.Empty;
        string m_OutputDirectoryPath = string.Empty;
        CompressionType m_CompressionType = CompressionType.SevenZip;
        int m_CompressionLevel = 9;
        DirectoryNode? m_SelectedNode;
        string m_StatusText = "Idle";

        readonly IFileService m_FileService;
        readonly ILogService m_LogService;
        readonly IWindowService m_WindowService;

        readonly ObservableCollection<FileItem> m_FileItems = new();
        readonly ObservableCollection<DirectoryNode> m_DirectoryTree;

        readonly IAsyncRelayCommand m_BrowseRootDirectoryCommand;
        readonly IAsyncRelayCommand m_BrowseOutputDirectoryCommand;
        readonly IAsyncRelayCommand m_ScanCommand;
        readonly IAsyncRelayCommand m_AnalyzeCommand;
        readonly IAsyncRelayCommand m_CompressCommand;

        readonly IRelayCommand<DirectoryNode> m_ToggleCompressionCommand;
        readonly IRelayCommand<DirectoryNode> m_SelectItemCommand;

        readonly List<IRelayCommand> m_RelayCommands;
        readonly List<IAsyncRelayCommand> m_AsyncRelayCommands;

        #endregion
    }
}
