using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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
            SaveCommand = new RelayCommand<AnimatedWindow>(Save);
            _language = _config.Language;
        }

        public Language Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public ICommand SaveCommand { get; }

        private void Save(AnimatedWindow window)
        {
            if (Language != _config.Language)
            {
                if (Language == Language.Other)
                {
                    MessageWindow.Show(Lang.Language_Other);
                    Language = _config.Language;
                    return;
                }
                _config.Language = Language;
                if (Language != _config.Language)
                {
                    if (MessageWindow.Show(Lang.Require_CHT_Resource, true) != true)
                    {
                        Language = _config.Language;
                        return;
                    }
                    Process.Start("https://snsteam.club/downloads/");
                }
                else
                {
                    try
                    {
                        _config.Save();
                    }
                    catch (Exception e)
                    {
                        MessageWindow.Show($"{Lang.Exception}{e.Message}");
                    }
                    MessageWindow.Show(Lang.Language_Info);
                }
            }
            window.Close();
        }
    }
}