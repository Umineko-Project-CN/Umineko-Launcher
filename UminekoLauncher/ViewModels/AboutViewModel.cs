using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using UminekoLauncher.Views;

namespace UminekoLauncher.ViewModels
{
    class AboutViewModel : ObservableObject
    {
        private const string _uminekoProjectUrl = "https://umineko-project.org/";
        private const string _snsteamUrl = "https://snsteam.club/";
        private const string _entergramUrl = "http://entergram.co.jp/umineko/";
        private const string _steamUrl = "https://store.steampowered.com/bundle/5465/Umineko_When_They_Cry_Complete_Collection/";
        private const string _playStationUrl = "https://store.playstation.com/ja-jp/product/JP0741-CUSA16973_00-UMINEKOSAKUZZZZZ";
        private const string _nintendoUrl = "https://store-jp.nintendo.com/list/software/70010000012343.html";

        public string TextVersion => $"LauncherVer. {Application.ResourceAssembly.GetName().Version}";
        public ICommand DismissCommand => new RelayCommand<AnimatedControl>(Dismiss);
        public ICommand OpenWebsiteCommand => new RelayCommand<string>(OpenWebsite);

        private void OpenWebsite(string str)
        {
            string url;
            switch (str)
            {
                case "up":
                    url = _uminekoProjectUrl;
                    break;
                case "sn":
                    url = _snsteamUrl;
                    break;
                case "eg":
                    url = _entergramUrl;
                    break;
                case "st":
                    url = _steamUrl;
                    break;
                case "ps":
                    url = _playStationUrl;
                    break;
                case "ns":
                    url = _nintendoUrl;
                    break;
                default:
                    throw new ArgumentException();
            }
            Process.Start(url);
        }

        private void Dismiss(AnimatedControl control)
        {
            control.IsOpen = false;
        }
    }
}
