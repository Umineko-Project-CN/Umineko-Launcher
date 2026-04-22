using CommunityToolkit.Mvvm.ComponentModel;
using UminekoLauncher.Services;

namespace UminekoLauncher.ViewModels
{
    internal class ConfigViewModel : ObservableObject
    {
        private Config _config = Config.GetConfig();

        public Config Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }
    }
}