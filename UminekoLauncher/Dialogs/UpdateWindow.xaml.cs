using System;
using System.Windows;
using AutoUpdaterDotNET;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public static UpdateInfoEventArgs updateInfo;
        public UpdateWindow()
        {
            InitializeComponent();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            GameConfig.GameVersion = new Version(updateInfo.CurrentVersion);
            try
            {
                if (AutoUpdater.DownloadUpdate(updateInfo))
                {
                    Application.Current.Shutdown();
                }
            }
            catch (Exception exception)
            {
                textMessage.Text = exception.Message;
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
