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
            var animation = FindResource("ExitAnimation") as System.Windows.Media.Animation.Storyboard;
            animation.Completed += (a, b) =>
            {
                Visibility = Visibility.Collapsed;
            };
            animation.Begin(this);
        }

        private void btnWebsite1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://umineko-project.org/");
        }

        private void btnWebsite2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://snsteam.club/");
        }
        private void btnWebsite3_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://entergram.co.jp/umineko/");
        }

        private void btnHome1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store.steampowered.com/bundle/5465/Umineko_When_They_Cry_Complete_Collection/");
        }

        private void btnHome2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store.playstation.com/ja-jp/product/JP0741-CUSA16973_00-UMINEKOSAKUZZZZZ");
        }

        private void btnHome3_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store-jp.nintendo.com/list/software/70010000012343.html");
        }
    }
}
