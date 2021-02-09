using AutoUpdaterDotNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UminekoLauncher.Dialogs;

namespace UminekoLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("ons.cfg"))
            {
                new MessageWindow("出错了!\n\n未检测到配置文件，请检查游戏完整性。\a").ShowDialog();
                Application.Current.Shutdown();
            }
            else
            {
                // 载入配置，检查更新
                GameConfig.LoadConfig("ons.cfg");
                AutoUpdater.CheckForUpdateEvent += CheckForUpdate;
                AutoUpdater.InstalledVersion = GameConfig.GameVersion;
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.Start("https://down.snsteam.club/update.xml");
                textVersion.Text += GameConfig.GameVersion.ToString();
            }
        }

        private void CheckForUpdate(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                textInfo.Text = new WebClient() { Encoding = Encoding.UTF8 }.DownloadString(new Uri(args.ChangelogURL));
                if (args.IsUpdateAvailable)
                {
                    textVersion.Text = "New: " + args.CurrentVersion;
                    textVersion.Foreground = Brushes.Yellow;
                    btnUpgrade.IsEnabled = true;
                    UpdateWindow.updateInfo = args;
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    textInfo.Text = "连接至更新服务器时出错。";
                }
                else
                {
                    textInfo.Text = args.Error.Message;
                }
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                // Arguments = "--env[legacy_op] true",
                FileName = "onscripter-ru.exe"
            };
            try
            {
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                new MessageWindow("启动失败!\n\n请检查游戏完整性及相关设置。") { Owner = this }.ShowDialog();
            }
        }
        private void btnUpgrade_Click(object sender, RoutedEventArgs e)
        {
            new UpdateWindow() { Owner = this }.ShowDialog();
        }
        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            new ConfigWindow { Owner = this }.ShowDialog();
        }
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow { Owner = this }.ShowDialog();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GameConfig.IsLoaded)
                GameConfig.SaveConfig("ons.cfg");
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            Keyboard.Focus(scrollViewer);
        }
    }
}
