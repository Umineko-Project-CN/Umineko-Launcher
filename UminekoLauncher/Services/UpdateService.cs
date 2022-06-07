using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services
{
    /// <summary>
    /// 表示更新服务状态。
    /// </summary>
    internal enum UpdateStatus
    {
        NotStarted,
        ReadyToUpdate,
        NeedManualUpdate,
        UpToDate,
        Error
    }

    /// <summary>
    /// 更新服务。
    /// </summary>
    internal static class UpdateService
    {
        private const string UpdateUrl = "https://down.snsteam.club/update.xml";
        private static readonly string _installerPath = Path.Combine(Path.GetTempPath(), "ZipExtractor.exe");
        private static readonly Queue<UpdateItem> _updateItems = new Queue<UpdateItem>();

        private static readonly WebClient _webClient = new WebClient
        {
            Encoding = System.Text.Encoding.UTF8,
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
        };

        private static string _changelog = "尚未检查更新。";
        private static string _extraLink = string.Empty;
        private static bool _needManualUpdate = false;

        static UpdateService()
        {
            _webClient.DownloadProgressChanged += (a, b) => DownloadProgressChanged?.Invoke(a, b);
        }

        /// <summary>
        /// 单文件下载进度改变时将引发此事件。
        /// </summary>
        public static event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// 更新状态改变时将引发此事件。
        /// </summary>
        public static event EventHandler<UpdateStatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// 更新文件全部下载完成后将引发此事件。
        /// </summary>
        public static event EventHandler UpdatesAllDownloaded;

        /// <summary>
        /// 更新日志文本。
        /// </summary>
        public static string Changelog => _changelog;

        /// <summary>
        /// 额外链接。
        /// </summary>
        public static string ExtraLink => _extraLink;

        /// <summary>
        /// 表示更新服务的当前状态。
        /// </summary>
        public static UpdateStatus Status { get; private set; } = UpdateStatus.NotStarted;

        /// <summary>
        /// 开始检查更新。
        /// </summary>
        public static async void CheckAsync()
        {
            if (Status != UpdateStatus.NotStarted)
            {
                return;
            }
            try
            {
                await CheckUpdateAsync();
                if (_updateItems.Count > 0)
                {
                    Status = UpdateStatus.ReadyToUpdate;
                }
                else if (_needManualUpdate)
                {
                    Status = UpdateStatus.NeedManualUpdate;
                }
                else
                {
                    Status = UpdateStatus.UpToDate;
                }
                var args = new UpdateStatusChangedEventArgs(Status);
                StatusChanged(null, args);
            }
            catch (Exception e)
            {
                if (e is WebException)
                {
                    _changelog = "与服务器通信时出错。";
                }
                else
                {
                    _changelog = $"异常：{e}\n{e.Message}";
                }
                Status = UpdateStatus.Error;
                var args = new UpdateStatusChangedEventArgs(Status, e);
                StatusChanged(null, args);
            }
        }

        /// <summary>
        /// 开始更新。
        /// </summary>
        public static async void Update()
        {
            if (Status != UpdateStatus.ReadyToUpdate)
            {
                return;
            }
            try
            {
                foreach (var item in _updateItems)
                {
                    item.FilePath = Path.GetTempFileName();
                    _webClient.DownloadFileAsync(item.DownloadUri, item.FilePath);
                    while (_webClient.IsBusy)
                    {
                        await Task.Delay(500);
                    }
                }
                UpdatesAllDownloaded(null, null);
                File.WriteAllBytes(_installerPath, Properties.Resources.ZipExtractor);
                while (_updateItems.Count > 0)
                {
                    UpdateItem item = _updateItems.Dequeue();
                    Install(item);
                }
                Status = _needManualUpdate ? UpdateStatus.NeedManualUpdate : UpdateStatus.UpToDate;
                var args = new UpdateStatusChangedEventArgs(Status);
                StatusChanged(null, args);
            }
            catch (Exception e)
            {
                Status = UpdateStatus.Error;
                var args = new UpdateStatusChangedEventArgs(Status, e);
                StatusChanged(null, args);
            }
        }

        private static async Task CheckUpdateAsync()
        {
            string xml = await _webClient.DownloadStringTaskAsync(UpdateUrl);
            var stringReader = new StringReader(xml);
            var xmlSerializer = new XmlSerializer(typeof(UpdateInfoModel));
            UpdateInfoModel updateInfo = (UpdateInfoModel)xmlSerializer.Deserialize(stringReader);
            _changelog = await _webClient.DownloadStringTaskAsync(updateInfo.ChangelogUrl);
            _extraLink = updateInfo.ExtraLink;
            string[] fields =
            {
                    updateInfo.LauncherInfo.Version,
                    updateInfo.ScriptInfo.FileHash.Value,
                    updateInfo.ResourceInfo.Version,
                    updateInfo.LauncherInfo.DownloadUrl,
                    updateInfo.ScriptInfo.DownloadUrl,
                    updateInfo.ResourceInfo.DownloadUrl
            };
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                {
                    throw new MissingFieldException("缺失必要的更新字段。");
                }
            }
            Version localLauncherVersion = Application.ResourceAssembly.GetName().Version;
            Checksum localScriptHash = Misc.GetHash("cn.file", updateInfo.ScriptInfo.FileHash.HashAlgorithm);
            Version localResourceVersion = Misc.GetResourceVersion();
            Version remoteResourceVersion = new Version(updateInfo.ResourceInfo.Version);
            void EnqueueUpdate(UpdateInfoModel.Item info, bool isRestartNeeded = false)
            {
                _updateItems.Enqueue(new UpdateItem()
                {
                    IsRestartNeeded = isRestartNeeded,
                    DownloadUri = new Uri(info.DownloadUrl),
                    PackageHash = info.PackageHash
                });
            }
            if (localScriptHash != updateInfo.ScriptInfo.FileHash)
            {
                EnqueueUpdate(updateInfo.ScriptInfo);
            }
            if (localResourceVersion < remoteResourceVersion)
            {
                if (localResourceVersion.Major == remoteResourceVersion.Major)
                {
                    EnqueueUpdate(updateInfo.ResourceInfo);
                }
                else
                {
                    _needManualUpdate = true;
                }
            }
            if (localLauncherVersion < new Version(updateInfo.LauncherInfo.Version))
            {
                EnqueueUpdate(updateInfo.LauncherInfo, true);
            }
        }

        private static void Install(UpdateItem item)
        {
            if (Misc.GetHash(item.FilePath) != item.PackageHash)
            {
                throw new Exception("文件完整性检查失败。");
            }
            string executablePath = Process.GetCurrentProcess().MainModule.FileName;
            string extractionPath = Path.GetDirectoryName(executablePath);
            string arguments = $"\"{item.FilePath}\" \"{extractionPath}\"";
            if (item.IsRestartNeeded)
            {
                arguments += $" \"{executablePath}\"";
            }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = _installerPath,
                UseShellExecute = true,
                Arguments = arguments.ToString()
            };
            var installProgress = Process.Start(processStartInfo);
            if (item.IsRestartNeeded)
            {
                Application.Current.MainWindow.Close();
            }
            else
            {
                installProgress.WaitForExit();
            }
        }
    }

    /// <summary>
    /// 表示待更新项。
    /// </summary>
    internal class UpdateItem
    {
        public Uri DownloadUri { get; set; }
        public string FilePath { get; set; }
        public bool IsRestartNeeded { get; set; }
        public Checksum PackageHash { get; set; }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class UpdateStatusChangedEventArgs : EventArgs
    {
        public UpdateStatusChangedEventArgs(UpdateStatus status, Exception e = null)
        {
            UpdateStatus = status;
            Exception = e;
        }

        public Exception Exception { get; }
        public UpdateStatus UpdateStatus { get; }
    }
}