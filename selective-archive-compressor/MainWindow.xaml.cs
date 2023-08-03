using selective_archive_compressor.viewmodel;

namespace selective_archive_compressor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = App.Current.ServiceProvider.GetService(typeof(MainViewModel));
            InitializeComponent();
        }
    }
}
