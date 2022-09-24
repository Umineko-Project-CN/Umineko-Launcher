using System;
using System.Threading.Tasks;
using System.Windows;
using UminekoLauncher.Localization;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// SplashScreen.xaml 的交互逻辑
    /// </summary>
    public partial class SplashScreen : AnimatedWindow
    {
        public SplashScreen()
        {
            InitializeComponent();
            _opacityFadeIn.Duration = TimeSpan.FromSeconds(0.2);
            _blurFadeIn.Duration = TimeSpan.FromSeconds(0.2);
            _opacityFadeOut.Duration = TimeSpan.FromSeconds(0.2);
            _blurFadeOut.Duration = TimeSpan.FromSeconds(0.2);
            imgSplash.Source = Localized.GameLogoImage;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(300);
            Window mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        }
    }
}