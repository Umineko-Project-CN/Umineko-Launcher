using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using UminekoLauncher.Localization;
using UminekoLauncher.Models;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

namespace UminekoLauncher.ViewModels
{
    internal class LanguageViewModel : ObservableObject
    {
        private readonly Config _config = Config.GetConfig();
        private Language _language;

        public LanguageViewModel()
        {
            CheckCommand = new RelayCommand<Window>(Check);
            _language = _config.Language;
        }

        public Language Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public RelayCommand<Window> CheckCommand { get; }

        private void Check(Window window)
        {
            if (Language == _config.Language)
            {
                window.Close();
                return;
            }
            if (Language == Language.Other)
            {
                MessageWindow.Show(Lang.Language_Other);
                Language = _config.Language;
                return;
            }
            _config.Language = Language;
            if (Language == _config.Language)
            {
                MessageWindow.Show(Lang.Language_Info);
                window.Close();
                return;
            }
            if (MessageWindow.Show(Lang.Require_CHT_Resource, true) == true)
            {
                Process.Start("https://snsteam.club/downloads/");
                window.Close();
                return;
            }
            Language = _config.Language;
        }
    }
}