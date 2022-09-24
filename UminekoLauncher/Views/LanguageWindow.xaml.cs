using System.Windows;
using UminekoLauncher.ViewModels;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// LanguageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LanguageWindow : AnimatedWindow
    {
        public LanguageWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            DataContext = new LanguageViewModel();
        }
    }
}