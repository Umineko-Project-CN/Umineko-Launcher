using System.Diagnostics;
using System.Globalization;
using System.Windows;
using UminekoLauncher.Localization;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

namespace UminekoLauncher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            if (processes.Length > 1)
            {
                MessageWindow.Show(Lang.Error_Running);
                Current.Shutdown();
            }
            if (!ConfigService.FileExists())
            {
                MessageWindow.Show(Lang.Error_Broken);
                Current.Shutdown();
            }
            //CultureInfo.CurrentUICulture = new CultureInfo("en");
        }
    }
}
