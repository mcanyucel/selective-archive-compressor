using Microsoft.Extensions.DependencyInjection;
using selective_archive_compressor.service;
using selective_archive_compressor.service.implementation;
using selective_archive_compressor.viewmodel;
using System;
using System.Windows;

namespace selective_archive_compressor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider ServiceProvider { get; }

        public App()
        {
            ServiceProvider = ConfigureServices();
            this.InitializeComponent();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFileService, WindowsFileService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddTransient<MainViewModel>();
            return services.BuildServiceProvider();
        }
    }
}
