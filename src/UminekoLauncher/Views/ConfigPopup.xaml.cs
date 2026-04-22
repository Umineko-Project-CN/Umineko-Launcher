using UminekoLauncher.ViewModels;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// ConfigPopup.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigPopup : AnimatedControl
    {
        public ConfigPopup()
        {
            InitializeComponent();
            DataContext = new ConfigViewModel();
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}