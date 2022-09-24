using UminekoLauncher.ViewModels;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// AboutPopup.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPopup : AnimatedControl
    {
        public AboutPopup()
        {
            InitializeComponent();
            DataContext = new AboutViewModel();
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}