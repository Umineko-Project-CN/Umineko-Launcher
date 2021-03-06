﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Windows;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Item _args;
        private string _tempFile;
        private UpdaterWebClient _webClient;
        private DateTime _startedAt;
        public UpdateWindow(UpdateInfoEventArgs args, Window owner)
        {
            InitializeComponent();

            Owner = owner;
            if (args.LauncherInfo.IsUpdateAvailable)
            {
                _args = args.LauncherInfo;
            }
            else if (args.ScriptInfo.IsUpdateAvailable)
            {
                _args = args.ScriptInfo;
            }
            else if (args.ResourceInfo.IsUpdateAvailable)
            {
                _args = args.ResourceInfo;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(_args.DownloadURL);
            _webClient = Updater.GetWebClient();
            _tempFile = Path.GetTempFileName();
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
            _webClient.DownloadFileAsync(uri, _tempFile);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_startedAt == default)
            {
                _startedAt = DateTime.Now;
            }
            else
            {
                var timeSpan = DateTime.Now - _startedAt;
                long totalSeconds = (long)timeSpan.TotalSeconds;
                if (totalSeconds > 0)
                {
                    var bytesPerSecond = e.BytesReceived / totalSeconds;
                    textInformation.Text =
                        string.Format("下载速度: {0}/s", BytesToString(bytesPerSecond));
                }
            }
            textSize.Text = $@"{BytesToString(e.BytesReceived)} / {BytesToString(e.TotalBytesToReceive)}";
            pbDownload.Value = e.ProgressPercentage;
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            if (asyncCompletedEventArgs.Cancelled)
            {
                return;
            }
            try
            {
                if (asyncCompletedEventArgs.Error != null)
                {
                    throw asyncCompletedEventArgs.Error;
                }
                if (_args.Checksum != null)
                {
                    CompareChecksum(_tempFile, _args.Checksum);
                }
                ContentDisposition contentDisposition = null;
                if (_webClient.ResponseHeaders["Content-Disposition"] != null)
                {
                    contentDisposition = new ContentDisposition(_webClient.ResponseHeaders["Content-Disposition"]);
                }
                var fileName = string.IsNullOrEmpty(contentDisposition?.FileName)
                    ? Path.GetFileName(_webClient.ResponseUri.LocalPath)
                    : contentDisposition.FileName;
                var tempPath = Path.Combine(Path.GetTempPath(), fileName);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                File.Move(_tempFile, tempPath);
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true,
                    //Arguments = installerArgs
                };
                var extension = Path.GetExtension(tempPath);
                if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    string installerPath = Path.Combine(Path.GetDirectoryName(tempPath), "ZipExtractor.exe");
                    File.WriteAllBytes(installerPath, Properties.Resources.ZipExtractor);

                    string executablePath = Process.GetCurrentProcess().MainModule.FileName;
                    string extractionPath = Path.GetDirectoryName(executablePath);

                    StringBuilder arguments =
                        new StringBuilder($"\"{tempPath}\" \"{extractionPath}\" \"{executablePath}\"");
                    string[] args = Environment.GetCommandLineArgs();
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (i.Equals(1))
                        {
                            arguments.Append(" \"");
                        }
                        arguments.Append(args[i]);
                        arguments.Append(i.Equals(args.Length - 1) ? "\"" : " ");
                    }

                    processStartInfo = new ProcessStartInfo
                    {
                        FileName = installerPath,
                        UseShellExecute = true,
                        Arguments = arguments.ToString()
                    };
                }
                if (Updater.RunUpdateAsAdmin)
                {
                    processStartInfo.Verb = "runas";
                }
                try
                {
                    Process.Start(processStartInfo);
                }
                catch (Win32Exception exception)
                {
                    if (exception.NativeErrorCode == 1223)
                    {
                        _webClient = null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                new MessageWindow(e.Message, Owner).ShowDialog();
                _webClient = null;
            }
            finally
            {
                DialogResult = _webClient != null;
                Closing -= Window_Closing;
                Close();
            }
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{(Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture)} {suf[place]}";
        }

        private static void CompareChecksum(string fileName, Checksum checksum)
        {
            if (GameHash.GetHash(fileName, checksum).Value == checksum.Value) return;
            throw new Exception("文件完整性检查失败。");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_webClient != null && _webClient.IsBusy)
            {
                _webClient.CancelAsync();
                DialogResult = false;
            }
        }
    }
}