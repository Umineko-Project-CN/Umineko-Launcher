﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using UminekoLauncher.Services;

namespace UminekoLauncher.ViewModels
{
    internal class DownloadViewModel : ObservableObject
    {
        private Window _downloadWindow;
        private readonly Timer _timer = new Timer();
        private long _bytesReceived;
        private long _currentBytesReceived;

        private string _downloadSpeed = "正在下载……";

        public string DownloadSpeed
        {
            get => _downloadSpeed;
            set => SetProperty(ref _downloadSpeed, value);
        }

        private string _fileSize;

        public string FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        private int _downloadProgress;

        public int DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        public ICommand LoadedCommand => new RelayCommand<Window>(Loaded);

        public ICommand ClosingCommand => new RelayCommand(Closing);

        private void Loaded(Window window)
        {
            _downloadWindow = window;
            UpdateService.DownloadProgressChanged += UpdateService_DownloadProgressChanged;
            UpdateService.UpdatesAllDownloaded += UpdateService_UpdatesAllDownloaded;
            UpdateService.Update();
            _timer.Interval = 500;
            _timer.Elapsed += UpdateSpeedText;
            _timer.Start();
        }


        private void Closing()
        {
            _timer.Stop();
            UpdateService.DownloadProgressChanged -= UpdateService_DownloadProgressChanged;
            UpdateService.UpdatesAllDownloaded -= UpdateService_UpdatesAllDownloaded;
        }


        private void UpdateService_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _currentBytesReceived = e.BytesReceived;
            FileSize = $"{BytesToString(e.BytesReceived)}/{BytesToString(e.TotalBytesToReceive)}";
            DownloadProgress = e.ProgressPercentage;
        }

        private void UpdateService_UpdatesAllDownloaded(object sender, EventArgs e)
        {
            _downloadWindow.Close();
        }

        private void UpdateSpeedText(object sender, ElapsedEventArgs e)
        {
            long bytesPerSecond = _currentBytesReceived - _bytesReceived;
            DownloadSpeed = $"正在下载：{BytesToString(bytesPerSecond)}/s";
            _bytesReceived = _currentBytesReceived;
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{Math.Sign(byteCount) * num} {suf[place]}";
        }
    }
}