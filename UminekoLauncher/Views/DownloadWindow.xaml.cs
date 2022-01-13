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
    }
}