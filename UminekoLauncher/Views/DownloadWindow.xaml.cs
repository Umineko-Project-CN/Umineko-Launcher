using System.Windows;
using UminekoLauncher.ViewModels;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWindow : AnimatedWindow
    {
        public DownloadWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            DataContext = new DownloadViewModel();
        }

        private void downloadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (DownloadViewModel)DataContext;
            viewModel.LoadedCommand.Execute(null);
        }

        private void downloadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (DownloadViewModel)DataContext;
            viewModel.ClosingCommand.Execute(null);
        }
    }
}