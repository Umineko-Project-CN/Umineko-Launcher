using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using UminekoLauncher.Localization;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

namespace UminekoLauncher.ViewModels
{
    internal class ConfigViewModel : ObservableObject
    {
        private Config _config = Config.GetConfig();

        public ConfigViewModel()
        {
            LoadCommand = new RelayCommand<bool>(Load);
            SaveCommand = new RelayCommand<AnimatedControl>(Save);
        }

        public Config Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private void Save(AnimatedControl control)
        {
            try
            {
                _config.Save();
            }
            catch (Exception e)
            {
                MessageWindow.Show($"{Lang.Exception}{e.Message}");
            }
            control.IsOpen = false;
        }

        private void Load(bool configViewOpen)
        {
            if (!configViewOpen)
            {
                return;
            }
            try
            {
                _config.Load();
                OnPropertyChanged(nameof(Config));
            }
            catch (Exception e)
            {
                MessageWindow.Show($"{Lang.Exception}{e.Message}");
            }
        }
    }
}