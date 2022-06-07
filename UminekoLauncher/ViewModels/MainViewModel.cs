﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using UminekoLauncher.Services;
using UminekoLauncher.Views;

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
        private string _info = "请稍候";
        private string _news = "正在加载……\n\n为保证体验，建议等待检测完成后再开始游戏。";

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

        public UpdateStatus UpdateStatus => UpdateService.Status;
        public ICommand VerifyCommand { get; }

        private void Exit(Window window) => window.Close();

        private void GoDownload() => Process.Start(UpdateService.ExtraLink);

        private void GoSurvey() => Process.Start(SurveyUrl);

        private void InitCheck()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            if (processes.Length > 1)
            {
                MessageWindow.Show("出错了！启动器已在运行。");
                Application.Current.Shutdown();
            }
            if (!ConfigService.FileExists())
            {
                MessageWindow.Show("出错了！未检测到相关重要文件，请检查游戏完整性。");
                Application.Current.Shutdown();
            }
            else
            {
                // 检查游戏更新。
                UpdateService.StatusChanged += UpdateService_StatusChanged;
                UpdateService.CheckAsync();
            }
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
                MessageWindow.Show("启动失败！请检查游戏完整性及相关设置。");
            }
        }

        private void Update()
        {
            Information = "请等待更新完成";
            CanAction = false;
            new DownloadWindow().Show();
            Application.Current.MainWindow.Activate();
        }

        private void UpdateService_StatusChanged(object sender, UpdateStatusChangedEventArgs e)
        {
            OnPropertyChanged(nameof(UpdateStatus));
            CanAction = true;
            News = UpdateService.Changelog;
            switch (e.UpdateStatus)
            {
                case UpdateStatus.ReadyToUpdate:
                    Information = "点击按钮以自动更新";
                    ActionCommand = _updateCommand;
                    break;

                case UpdateStatus.NeedManualUpdate:
                    Information = "点击按钮以前往下载";
                    ActionCommand = _goDownloadCommand;
                    break;

                case UpdateStatus.UpToDate:
                    Information = "无可用更新";
                    ActionCommand = _launchCommand;
                    break;

                case UpdateStatus.Error:
                    Information = e.Exception is WebException ? "请检查互联网连接并稍后重试" : "若重试更新无法解决还请反馈";
                    ActionCommand = _launchCommand;
                    break;

                default:
                    break;
            }
        }

        private void VerifyLaunch()
        {
            if (MessageWindow.Show("确定要检查游戏完整性吗？", true) == true)
            {
                Launch(true);
            }
        }
    }
}