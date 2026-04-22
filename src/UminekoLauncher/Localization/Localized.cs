using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace UminekoLauncher.Localization
{
    internal static class Localized
    {
        public static CultureInfo UICulture { get; set; }
        public static BitmapImage GameLogoImage { get; set; }
        public static BitmapImage TeamLogoImage { get; set; }

        private static readonly BitmapImage GameLogoCHS = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Splashes/chs.png"));
        private static readonly BitmapImage GameLogoCHT = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Splashes/cht.png"));
        private static readonly BitmapImage GameLogoEN = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Splashes/en.png"));
        private static readonly BitmapImage TeamLogoCHS = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Logos/chs.png"));
        private static readonly BitmapImage TeamLogoCHT = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Logos/cht.png"));

        static Localized()
        {
            UICulture = CultureInfo.CurrentUICulture;
            switch (UICulture.Name)
            {
                case "zh-CHS":
                    GameLogoImage = GameLogoCHS;
                    TeamLogoImage = TeamLogoCHS;
                    break;

                case "zh-CHT":
                    GameLogoImage = GameLogoCHT;
                    TeamLogoImage = TeamLogoCHT;
                    break;

                default:
                    GameLogoImage = GameLogoEN;
                    TeamLogoImage = TeamLogoCHS;
                    break;
            }
        }
    }
}