using System;
using System.Windows.Input;
using UminekoLauncher.Localization;
using UminekoLauncher.ViewModels;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : AnimatedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            _opacityFadeIn.Duration = TimeSpan.FromSeconds(0.4);
            _blurFadeIn.Duration = TimeSpan.FromSeconds(0.4);
            _opacityFadeOut.Duration = TimeSpan.FromSeconds(0.2);
            _blurFadeOut.Duration = TimeSpan.FromSeconds(0.2);
            imgGame.Source = Localized.GameLogoImage;
            imgTeam.Source = Localized.TeamLogoImage;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Language_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new LanguageWindow().ShowDialog();
        }
    }
}