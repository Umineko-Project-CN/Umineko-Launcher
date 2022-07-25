using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using UminekoLauncher.Localization;
using UminekoLauncher.Models;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

namespace UminekoLauncher.ViewModels
{
    internal class ConfigViewModel : ObservableObject
    {
        private ConfigModel _config;

        public ConfigViewModel()
        {
            LoadCommand = new RelayCommand<bool>(Load);
            DismissCommand = new RelayCommand<AnimatedControl>(Dismiss);
        }

        public ConfigModel Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public ICommand DismissCommand { get; }
        public ICommand LoadCommand { get; }

        private void Dismiss(AnimatedControl control)
        {
            try
            {
                ConfigService.SaveConfig(Config);
            }
            catch (Exception e)
            {
                MessageWindow.Show($"{Lang.Exception}{e.Message}");
            }
            control.IsOpen = false;
        }

        private void Load(bool configViewOpen)
        {
            if (configViewOpen)
            {
                try
                {
                    Config = ConfigService.LoadConfig();
                }
                catch (Exception e)
                {
                    MessageWindow.Show($"{Lang.Exception}{e.Message}");
                }
            }
        }
    }
}