using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using UminekoLauncher.Models;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

namespace UminekoLauncher.ViewModels
{
    class ConfigViewModel : ObservableObject
    {
        private ConfigModel _config;

        public ConfigModel Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public ICommand LoadCommand => new RelayCommand<bool>(Load);

        public ICommand DismissCommand => new RelayCommand<AnimatedControl>(Dismiss);

        private void Load(bool configViewOpen)
        {
            if (configViewOpen)
                Config = ConfigService.LoadConfig();
        }

        private void Dismiss(AnimatedControl control)
        {
            ConfigService.SaveConfig(Config);
            control.IsOpen = false;
        }
    }
}
