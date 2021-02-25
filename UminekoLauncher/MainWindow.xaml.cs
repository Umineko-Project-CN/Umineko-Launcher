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
        private static System.Threading.Mutex mutex;
        private UpdateInfoEventArgs updateInfo;
        private const string configPath = "ons.cfg";
        private bool verify = false;
        public MainWindow()
        {
            InitializeComponent();
            mutex = new System.Threading.Mutex(true, "UminekoLauncher");
            if (!mutex.WaitOne(0, false))
            {
                new MessageWindow("出错了！启动器已在运行。").ShowDialog();
                Application.Current.Shutdown();
            }
            if (!File.Exists(configPath))
            {
                new MessageWindow("出错了！未检测到配置文件，请检查游戏完整性。").ShowDialog();
                Application.Current.Shutdown();
            }
            else
            {
                // 载入配置，检查游戏更新
                configPopup.LoadConfig(configPath);
                textVersion.Text = GameConfig.GameVersion.ToString();
                Updater.UpdateCheckedEvent += Check;
                Updater.InstalledGameVersion = GameConfig.GameVersion;
                Updater.Start("https://down.snsteam.club/update.xml");
            }
        }
        private void Check(UpdateInfoEventArgs args)
        {
            Dispatcher.Invoke(new Action<UpdateInfoEventArgs>(CheckForUpdate), args);
        }
        private void CheckForUpdate(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                textNews.Text = new WebClient() { Encoding = Encoding.UTF8 }.DownloadString(new Uri(args.ChangelogURL));
                if (args.GameInfo.IsUpdateAvailable || args.LauncherInfo.IsUpdateAvailable)
                {
                    if (args.GameInfo.IsUpdateAvailable)
                    {
                        textUpdate.Text = args.GameInfo.LatestVersion;
                    }
                    else
                    {
                        textItem.Text = "LauncherVer.";
                        textVersion.Text = Updater.InstalledLauncherVersion.ToString();
                        textUpdate.Text = args.LauncherInfo.LatestVersion;
                    }
                    bdInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                    textInfo.Text = "需要更新";
                    textArrow.Visibility = Visibility.Visible;
                    textUpdate.Visibility = Visibility.Visible;
                    btnAction.Icon = "";
                    btnAction.Content = "获 取 更 新";
                    btnAction.Click -= Start;
                    btnAction.Click += Upgrade;
                    updateInfo = args;
                }
                else
                {
                    bdInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4b9f2a"));
                    textInfo.Text = "最新版本";
                }
            }
            else
            {
                bdInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                textInfo.Text = "检测失败";
                if (args.Error is WebException)
                {
                    textNews.Text = "连接至更新服务器时出错。";
                }
                else
                {
                    textNews.Text = args.Error.Message;
                }
            }
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "onscripter-ru.exe"
            };
            if (verify)
            {
                startInfo.Arguments = "--env[verify] full";
            }
            try
            {
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                new MessageWindow("启动失败！请检查游戏完整性及相关设置。", this).ShowDialog();
            }
        }
        private void VerifyStart(object sender, MouseButtonEventArgs e)
        {
            if (new MessageWindow("确定要检查游戏完整性吗？", this, true).ShowDialog() == true)
            {
                verify = true;
                Start(sender, e);
            }
        }
        private void Upgrade(object sender, RoutedEventArgs e)
        {
            bdInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#868686"));
            textInfo.Text = "正在更新";
            btnAction.IsEnabled = false;
            if (updateInfo.GameInfo.IsUpdateAvailable)
            {
                GameConfig.GameVersion = new Version(updateInfo.GameInfo.LatestVersion);
            }
            try
            {
                if (Updater.DownloadUpdate(updateInfo, this))
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    bdInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                    textInfo.Text = "更新失败";
                    btnAction.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                textNews.Text = exception.Message;
            }
        }
        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            configPopup.Visibility = Visibility.Visible;
        }
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            aboutPopup.Visibility = Visibility.Visible;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void configPopup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            spButtons.IsEnabled = (configPopup.Visibility == Visibility.Collapsed);
        }

        private void aboutPopup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            spButtons.IsEnabled = (aboutPopup.Visibility == Visibility.Collapsed);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GameConfig.IsLoaded)
                GameConfig.SaveConfig("ons.cfg");
        }
    }
}
