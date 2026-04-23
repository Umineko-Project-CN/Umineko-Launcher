using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Windows;
using System.Xml.Serialization;
using UminekoLauncher.Localization;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services;

/// <summary>
/// 表示更新服务状态。
/// </summary>
internal enum UpdateStatus
{
    NotStarted,
    ReadyToUpdate,
    NeedManualUpdate,
    UpToDate,
    Error,
}

/// <summary>
/// 更新服务。
/// </summary>
internal static class Updater
{
    private const string UpdateUrl = "https://down.snsteam.club/update.xml";
    private static readonly string s_installerPath = Path.Combine(
        Path.GetTempPath(),
        "ZipExtractor.exe"
    );
    private static readonly Queue<UpdateItem> s_updateItems = new();

    // TODO: 删除废弃用法。
    private static readonly WebClient s_webClient = new()
    {
        Encoding = System.Text.Encoding.UTF8,
        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
    };

    private static string s_changelog = Lang.Update_Not_Checked;
    private static string s_extraLink = string.Empty;
    private static bool s_needManualUpdate = false;

    static Updater()
    {
        s_webClient.DownloadProgressChanged += (a, b) => DownloadProgressChanged?.Invoke(a, b);
    }

    /// <summary>
    /// 单文件下载进度改变时将引发此事件。
    /// </summary>
    public static event DownloadProgressChangedEventHandler? DownloadProgressChanged;

    /// <summary>
    /// 更新状态改变时将引发此事件。
    /// </summary>
    public static event EventHandler<UpdateStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// 更新文件全部下载完成后将引发此事件。
    /// </summary>
    public static event EventHandler? UpdatesAllDownloaded;

    /// <summary>
    /// 更新日志文本。
    /// </summary>
    public static string Changelog => s_changelog;

    /// <summary>
    /// 额外链接。
    /// </summary>
    public static string ExtraLink => s_extraLink;

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
            if (s_updateItems.Count > 0)
            {
                Status = UpdateStatus.ReadyToUpdate;
            }
            else if (s_needManualUpdate)
            {
                Status = UpdateStatus.NeedManualUpdate;
            }
            else
            {
                Status = UpdateStatus.UpToDate;
            }
            var args = new UpdateStatusChangedEventArgs(Status);
            StatusChanged?.Invoke(null, args);
        }
        catch (Exception e)
        {
            if (e is WebException)
            {
                s_changelog = Lang.Error_Communication;
            }
            else
            {
                s_changelog = $"{Lang.Exception}{e}\n{e.Message}";
            }
            Status = UpdateStatus.Error;
            var args = new UpdateStatusChangedEventArgs(Status, e);
            StatusChanged?.Invoke(null, args);
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
            foreach (var item in s_updateItems)
            {
                item.FilePath = Path.GetTempFileName();
                s_webClient.DownloadFileAsync(item.DownloadUri, item.FilePath);
                while (s_webClient.IsBusy)
                {
                    await Task.Delay(500);
                }
            }
            UpdatesAllDownloaded?.Invoke(null, new EventArgs());
            File.WriteAllBytes(s_installerPath, Properties.Resources.ZipExtractor);
            while (s_updateItems.Count > 0)
            {
                UpdateItem item = s_updateItems.Dequeue();
                Install(item);
            }
            Status = s_needManualUpdate ? UpdateStatus.NeedManualUpdate : UpdateStatus.UpToDate;
            var args = new UpdateStatusChangedEventArgs(Status);
            StatusChanged?.Invoke(null, args);
        }
        catch (Exception e)
        {
            Status = UpdateStatus.Error;
            var args = new UpdateStatusChangedEventArgs(Status, e);
            StatusChanged?.Invoke(null, args);
        }
    }

    private static async Task CheckUpdateAsync()
    {
        string xml = await s_webClient.DownloadStringTaskAsync(UpdateUrl);
        var stringReader = new StringReader(xml);
        var xmlSerializer = new XmlSerializer(typeof(UpdateInfoModel));
        var updateInfo =
            xmlSerializer.Deserialize(stringReader) as UpdateInfoModel ?? throw new Exception();
        s_changelog = await s_webClient.DownloadStringTaskAsync(updateInfo.ChangelogUrl);
        s_extraLink = updateInfo.ExtraLink;
        string[] fields =
        [
            updateInfo.LauncherInfo.Version,
            updateInfo.ScriptInfo.FileHash.Value,
            updateInfo.ResourceInfo.Version,
            updateInfo.LauncherInfo.DownloadUrl,
            updateInfo.ScriptInfo.DownloadUrl,
            updateInfo.ResourceInfo.DownloadUrl,
        ];
        foreach (var field in fields)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new MissingFieldException(Lang.Missing_Field);
            }
        }
        Version? localLauncherVersion = Application.ResourceAssembly.GetName().Version;
        Checksum localScriptHash = Misc.GetHash(
            "cn.file",
            updateInfo.ScriptInfo.FileHash.HashAlgorithm
        );
        Version localResourceVersion = Misc.GetResourceVersion();
        Version remoteResourceVersion = new(updateInfo.ResourceInfo.Version);
        static void EnqueueUpdate(UpdateInfoModel.Item info, bool isRestartNeeded = false)
        {
            s_updateItems.Enqueue(
                new UpdateItem()
                {
                    IsRestartNeeded = isRestartNeeded,
                    DownloadUri = new Uri(info.DownloadUrl),
                    PackageHash = info.PackageHash,
                }
            );
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
                s_needManualUpdate = true;
            }
        }
        if (localLauncherVersion < new Version(updateInfo.LauncherInfo.Version))
        {
            EnqueueUpdate(updateInfo.LauncherInfo, true);
        }
    }

    private static void Install(UpdateItem item)
    {
        if (Misc.GetHash(item.FilePath!) != item.PackageHash)
        {
            throw new Exception(Lang.Failed_Integrity_Check);
        }
        string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;
        string? extractionPath = Path.GetDirectoryName(executablePath);
        string arguments = $"\"{item.FilePath}\" \"{extractionPath}\"";
        if (item.IsRestartNeeded)
        {
            arguments += $" \"{executablePath}\"";
        }
        var processStartInfo = new ProcessStartInfo
        {
            FileName = s_installerPath,
            UseShellExecute = true,
            Arguments = arguments.ToString(),
        };
        var installProgress = Process.Start(processStartInfo);
        if (item.IsRestartNeeded)
        {
            Application.Current.MainWindow.Close();
        }
        else
        {
            installProgress?.WaitForExit();
        }
    }
}

/// <summary>
/// 表示待更新项。
/// </summary>
internal class UpdateItem
{
    public required Uri DownloadUri { get; set; }
    public required bool IsRestartNeeded { get; set; }
    public required Checksum PackageHash { get; set; }
    public string? FilePath { get; set; }
}

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class UpdateStatusChangedEventArgs(UpdateStatus status, Exception? e = null) : EventArgs
{
    public Exception? Exception { get; } = e;
    public UpdateStatus UpdateStatus { get; } = status;
}
