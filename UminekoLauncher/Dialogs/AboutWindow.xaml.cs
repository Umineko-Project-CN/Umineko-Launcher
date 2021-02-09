using System.Diagnostics;
using System.Windows;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            textVersion.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
