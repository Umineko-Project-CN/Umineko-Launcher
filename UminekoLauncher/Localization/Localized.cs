using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace UminekoLauncher.Localization
{
    internal static class Localized
    {
        public static CultureInfo UICulture { get; set; }
        public static string LogoPath { get; set; }
        public static BitmapImage LogoImage { get; set; }

        static Localized()
        {
            UICulture = CultureInfo.CurrentUICulture;
            switch (UICulture.Parent.Name)
            {
                case "zh-CHS":
                    LogoPath = "pack://application:,,,/Resources/Images/Splashes/chs.png";
                    break;
                case "zh-CHT":
                    LogoPath = "pack://application:,,,/Resources/Images/Splashes/cht.png";
                    break;
                default:
                    LogoPath = "pack://application:,,,/Resources/Images/Splashes/en.png";
                    break;
            }
            LogoImage = new BitmapImage(new Uri(LogoPath));
        }
    }
}
