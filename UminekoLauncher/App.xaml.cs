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
            Config config = Config.GetConfig();
            if (!config.FileExists())
            {
                MessageWindow.Show(Lang.Error_Broken);
                Current.Shutdown();
                return;
            }
            config.Load();
            string culture;
            switch (config.Language)
            {
                case Models.Language.CHS:
                    culture = "zh-CHS";
                    break;

                case Models.Language.CHT:
                    culture = "zh-CHT";
                    break;

                default:
                    culture = "en";
                    break;
            }
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Config config = Config.GetConfig();
            if (config.FileExists())
            {
                Config.GetConfig().Save();
            }
        }
    }
}