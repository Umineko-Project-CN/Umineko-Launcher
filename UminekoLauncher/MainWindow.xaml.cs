using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
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
            if (!File.Exists(GameConfig.ConfigPath) || !File.Exists(GameHash.HashPath) || !File.Exists("cn.file"))
            {
                new MessageWindow("出错了！未检测到相关重要文件，请检查游戏完整性。").ShowDialog();
                Application.Current.Shutdown();
            }
            else
            {
                // 载入配置，检查游戏更新
                configPopup.LoadConfig();
                // textVersion.Text = GameConfig.GameVersion.ToString();
                Updater.UpdateCheckedEvent += Check;
                Updater.InstalledLauncherVersion = Assembly.GetExecutingAssembly().GetName().Version;
                // Updater.InstalledScriptHash 在检测更新时给出
                Updater.InstalledResourceVersion = GameHash.ResourceVersion;
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
                if (args.LauncherInfo.IsUpdateAvailable || args.ScriptInfo.IsUpdateAvailable || args.ResourceInfo.IsUpdateAvailable)
                {
                    circle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                    textStatus.FontWeight = FontWeights.Bold;
                    textStatus.Foreground = Brushes.White;
                    btnAction.Icon = "";
                    btnAction.Content = "获 取 更 新";
                    btnAction.Click -= Start;
                    if (args.LauncherInfo.IsUpdateAvailable)
                    {
                        textStatus.Text = "启动器需要更新";
                        textInfo.Text = "点击按钮以自动更新";
                        btnAction.Click += Upgrade;
                    }
                    else if (args.ScriptInfo.IsUpdateAvailable)
                    {
                        textStatus.Text = "脚本文件需要更新";
                        textInfo.Text = "点击按钮以自动更新";
                        btnAction.Click += Upgrade;
                    }
                    else
                    {
                        if (new Version(args.ResourceInfo.LatestVersion).Major > args.ResourceInfo.InstalledVersion.Major)
                        {
                            textStatus.Text = "资源文件需要手动更新";
                            textInfo.Text = "为保证游玩体验 强烈建议下载";
                            btnAction.Icon = "";
                            btnAction.Content = "前 往 下 载";
                            btnAction.Click += (a, b) => Process.Start(args.ExtraLink);
                        }
                        else
                        {
                            textStatus.Text = "资源文件需要更新";
                            btnAction.Click += Upgrade;
                            textInfo.Text = "点击按钮以自动更新";
                        }
                    }
                    updateInfo = args;
                }
                else
                {
                    circle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4b9f2a"));
                    textStatus.Text = "最新版本";
                    textInfo.Text = "无可用更新";
                }
            }
            else
            {
                circle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                textStatus.Text = "检测失败";
                textInfo.Text = "请检查互联网连接并稍后重试";
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
            circle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#868686"));
            textStatus.Text = "正在更新";
            textInfo.Text = "请稍候";
            btnAction.IsEnabled = false;
            try
            {
                if (Updater.DownloadUpdate(updateInfo, this))
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    circle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32630"));
                    textStatus.Text = "更新失败";
                    textInfo.Text = "请稍候重试";
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
            (FindResource("LoadAnimation") as System.Windows.Media.Animation.Storyboard).Begin(configPopup);
        }
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            aboutPopup.Visibility = Visibility.Visible;
            (FindResource("LoadAnimation") as System.Windows.Media.Animation.Storyboard).Begin(aboutPopup);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var animation = FindResource("ExitAnimation") as System.Windows.Media.Animation.Storyboard;
            animation.Completed += (a, b) =>
            {
                Application.Current.Shutdown();
            };
            animation.Begin(this);
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
                GameConfig.SaveConfig();
        }
    }
}
