using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Windows;
using System.Collections.Generic;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        /// <summary>
        /// 为true时_launcherItem有意义；为false时_otherItems有意义。
        /// </summary>
        private readonly bool _isLauncherUpdate;
        private readonly Item _launcherItem;
        private readonly Stack<Item> _otherItems;

        private string _tempFile;
        private string _installerPath;
        private UpdaterWebClient _webClient;
        private DateTime _startedAt;
        public UpdateWindow(UpdateInfoEventArgs args, Window owner)
        {
            InitializeComponent();

            Owner = owner;
            if (args.LauncherInfo.IsUpdateAvailable)
            {
                _isLauncherUpdate = true;
                _launcherItem = args.LauncherInfo;
            }
            else
            {
                _isLauncherUpdate = false;
                _otherItems = new Stack<Item>();

                // 倒序压栈，有点蠢
                // 说到底我也不知道我为什么会用栈，可能我就是想用
                if (args.ResourceInfo.IsUpdateAvailable)
                {
                    _otherItems.Push(args.ResourceInfo);
                }
                if (args.ScriptInfo.IsUpdateAvailable)
                {
                    _otherItems.Push(args.ScriptInfo);
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri;

            if (_isLauncherUpdate)
            {
                uri = new Uri(_launcherItem.DownloadURL);
            }
            else if (_otherItems.Count != 0)
            {
                uri = new Uri(_otherItems.Peek().DownloadURL);
            }
            else
            {
                return;
            }

            // 在这个时候就基本已经决定要用到 ZipExtractor 了，可能还要用两次，所以提前准备好
            _installerPath = Path.Combine(Path.GetTempPath(), "ZipExtractor.exe");
            File.WriteAllBytes(_installerPath, Properties.Resources.ZipExtractor);

            // setup（中文怎么说？）_webClient
            _webClient = Updater.GetWebClient();
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            _webClient.DownloadFileCompleted += OnDownloadLauncherCompleted;

            downloadToTemp(uri);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var timeSpan = DateTime.Now - _startedAt;
            long totalSeconds = (long)timeSpan.TotalSeconds;
            if (totalSeconds > 0)
            {
                var bytesPerSecond = e.BytesReceived / totalSeconds;
                textInformation.Text =
                    string.Format("下载速度: {0}/s", BytesToString(bytesPerSecond));
            }
            textSize.Text = $@"{BytesToString(e.BytesReceived)} / {BytesToString(e.TotalBytesToReceive)}";
            pbDownload.Value = e.ProgressPercentage;
        }

        private void OnDownloadLauncherCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
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

                // 根据 _isLauncherUpdate 有不同的行为
                Item item = _isLauncherUpdate ? _launcherItem : _otherItems.Pop();

                #region Reaname file
                if (item.Checksum != null)
                {
                    CompareChecksum(_tempFile, item.Checksum);
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
                #endregion

                #region Unzip file
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true,
                    //Arguments = installerArgs
                };
                var extension = Path.GetExtension(tempPath);
                if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    string executablePath = Process.GetCurrentProcess().MainModule.FileName;
                    string extractionPath = Path.GetDirectoryName(executablePath);

                    StringBuilder arguments =
                        new StringBuilder($"\"{tempPath}\" \"{extractionPath}\"");

                    // 只有在更新启动器时才重启它
                    if (_isLauncherUpdate)
                    {
                        arguments.Append(" \"{executablePath}\"");
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
                    }

                    processStartInfo = new ProcessStartInfo
                    {
                        FileName = _installerPath,
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
                    // 1223的意思是用户取消了这个操作
                    if (exception.NativeErrorCode == 1223)
                    {
                        _webClient = null;
                    }
                    else
                    {
                        throw;
                    }
                }
                #endregion 
            }
            catch (Exception e)
            {
                new MessageWindow(e.Message, Owner).ShowDialog();
                _webClient = null; // 意思就是给 DialogResult 赋 null
            }
            // 其实这个 finally 写不写应该是一样的
            finally
            {
                if (_isLauncherUpdate || _otherItems.Count == 0 || _webClient == null)
                {
                    // 原先我在 Window_Loaded 里给 DialogResult 赋值，这似乎导致了 ShowDialog 提前返回。
                    // 所以直到将要关闭窗口时才给它赋值。
                    // null 表示出错，true/false 表示是/否要重启启动器
                    DialogResult = _webClient != null ? _isLauncherUpdate : (bool?)null;
                    Closing -= Window_Closing;
                    Close();
                }
                else
                {
                    // 迭代 _otherItems
                    Uri uri = new Uri(_otherItems.Peek().DownloadURL);
                    downloadToTemp(uri);
                }
            }
        }

        private void downloadToTemp(Uri uri)
        {
            _tempFile = Path.GetTempFileName();
            _startedAt = DateTime.Now;
            _webClient.DownloadFileAsync(uri, _tempFile);
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