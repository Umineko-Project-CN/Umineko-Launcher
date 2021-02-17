using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// AboutPopup.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPopup : UserControl
    {
        public AboutPopup()
        {
            InitializeComponent();
            textVersion.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Visibility = Visibility.Collapsed;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void btnWebsite1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://umineko-project.org/");
        }

        private void btnWebsite2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://snsteam.club/");
        }
    }
}
