using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using UminekoLauncher.Services;
using UminekoLauncher.Views;
using UminekoLauncher.Localization;

namespace UminekoLauncher.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private const string SurveyUrl = "https://wj.qq.com/s2/8553984/974a/";
        private readonly ICommand _goDownloadCommand;
        private readonly ICommand _launchCommand;
        private readonly ICommand _updateCommand;
        private ICommand _actionCommand;
        private bool _canAction = false;
        private string _info = Lang.Wait;
        private string _news = Lang.Loading;

        public MainViewModel()
        {
            ActionCommand = _launchCommand;
            GoSurveyCommand = new RelayCommand(GoSurvey);
            VerifyCommand = new RelayCommand(VerifyLaunch);
            ExitCommand = new RelayCommand<Window>(Exit);
            _launchCommand = new RelayCommand(Launch);
            _updateCommand = new RelayCommand(Update);
            _goDownloadCommand = new RelayCommand(GoDownload);
            InitCheck();
        }

        public ICommand ActionCommand
        {
            get => _actionCommand;
            set => SetProperty(ref _actionCommand, value);
        }

        public bool CanAction
        {
            get => _canAction;
            set => SetProperty(ref _canAction, value);
        }

        public ICommand ExitCommand { get; }

        public ICommand GoSurveyCommand { get; }

        public string Information
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }

        public string News
        {
            get => _news;
            set => SetProperty(ref _news, value);
        }

        public UpdateStatus UpdateStatus => Updater.Status;
        public ICommand VerifyCommand { get; }

        private void Exit(Window window) => window.Close();

        private void GoDownload() => Process.Start(Updater.ExtraLink);

        private void GoSurvey() => Process.Start(SurveyUrl);

        private void InitCheck()
        {
            // 检查游戏更新。
            Updater.StatusChanged += UpdateService_StatusChanged;
            Updater.CheckAsync();
        }

        private void Launch() => Launch(false);

        private void Launch(bool verify)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "onscripter-ru.exe"
            };
            if (verify)
            {
                startInfo.Arguments = "--env[verify] full";
            }
            try
            {
                Process.Start(startInfo);
                Application.Current.MainWindow.Close();
            }
            catch (Exception)
            {
                MessageWindow.Show(Lang.Launch_Failed);
            }
        }

        private void Update()
        {
            Information = Lang.Wait_Updating;
            CanAction = false;
            new DownloadWindow().Show();
            Application.Current.MainWindow.Activate();
        }

        private void UpdateService_StatusChanged(object sender, UpdateStatusChangedEventArgs e)
        {
            OnPropertyChanged(nameof(UpdateStatus));
            CanAction = true;
            News = Updater.Changelog;
            switch (e.UpdateStatus)
            {
                case UpdateStatus.ReadyToUpdate:
                    Information = Lang.Click_Update;
                    ActionCommand = _updateCommand;
                    break;

                case UpdateStatus.NeedManualUpdate:
                    Information = Lang.Click_Download;
                    ActionCommand = _goDownloadCommand;
                    break;

                case UpdateStatus.UpToDate:
                    Information = Lang.No_Update;
                    ActionCommand = _launchCommand;
                    break;

                case UpdateStatus.Error:
                    Information = e.Exception is WebException ? Lang.Check_Internet : Lang.Retry;
                    ActionCommand = _launchCommand;
                    break;

                default:
                    break;
            }
        }

        private void VerifyLaunch()
        {
            if (MessageWindow.Show(Lang.Confirm_Check, true) == true)
            {
                Launch(true);
            }
        }
    }
}