using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UminekoLauncher.ViewModels;

internal class AboutViewModel : ObservableObject
{
    private const string EntergramUrl = "http://entergram.co.jp/umineko/";
    private const string NintendoUrl =
        "https://store-jp.nintendo.com/list/software/70010000012343.html";
    private const string PlayStationUrl =
        "https://store.playstation.com/ja-jp/product/JP0741-CUSA16973_00-UMINEKOSAKUZZZZZ";
    private const string SnsteamUrl = "https://snsteam.club/";
    private const string SteamUrl =
        "https://store.steampowered.com/bundle/5465/Umineko_When_They_Cry_Complete_Collection/";
    private const string UminekoProjectUrl = "https://umineko-project.org/";

    public AboutViewModel()
    {
        OpenWebsiteCommand = new RelayCommand<string>(OpenWebsite);
    }

    public RelayCommand<string> OpenWebsiteCommand { get; }
    public string TextVersion { get; } =
        $"LauncherVer. {Application.ResourceAssembly.GetName().Version}";

    private void OpenWebsite(string? str)
    {
        string url = str switch
        {
            "up" => UminekoProjectUrl,
            "sn" => SnsteamUrl,
            "eg" => EntergramUrl,
            "st" => SteamUrl,
            "ps" => PlayStationUrl,
            "ns" => NintendoUrl,
            _ => throw new ArgumentException($"Unknown website identifier: {str}"),
        };
        Process.Start(url);
    }
}
