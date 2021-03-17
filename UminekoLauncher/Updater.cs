using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace UminekoLauncher
{
    public static class Updater
    {
        internal static Uri BaseUri;
        internal static bool Running;

        /// <summary>
        /// 包含最新版本信息的 XML 文件的 URL。
        /// </summary>
        public static string AppCastURL { get; set; }

        /// <summary>
        /// 若为真，则安装时请求提升权限。
        /// </summary>
        public static bool RunUpdateAsAdmin { get; set; } = false;

        /// <summary>
        /// 若为真，则以同步方式检查更新。
        /// </summary>
        public static bool Synchronous { get; set; } = false;

        /// <summary>
        /// 已安装的启动器版本。
        /// </summary>
        public static Version InstalledLauncherVersion { get; set; }

        /// <summary>
        /// 已安装的脚本校验值。
        /// </summary>
        public static Checksum InstalledScriptHash { get; set; }

        /// <summary>
        /// 已安装的资源版本。
        /// </summary>
        public static Version InstalledResourceVersion { get; set; }

        /// <summary>
        /// 用于挂接更新通知的委托类型。
        /// </summary>
        /// <param name="args">一个包含所有从 AppCast XML 文件接收的参数的对象。若在读取该 XML 文件时出错，则此对象为空。</param>
        public delegate void UpdateCheckedEventHandler(UpdateInfoEventArgs args);

        /// <summary>
        /// 检查更新后，客户端可以利用该事件来得到通知。
        /// </summary>
        public static event UpdateCheckedEventHandler UpdateCheckedEvent;

        /// <summary>
        /// 开始检查更新。
        /// </summary>
        /// <param name="appCast">包含最新版本信息的 XML 文件的 URL。</param>
        public static void Start(string appCast)
        {
            if (!Running)
            {
                Running = true;
                AppCastURL = appCast;
                if (Synchronous)
                {
                    try
                    {
                        CheckUpdate();
                        Running = false;
                    }
                    catch (Exception exception)
                    {
                        ShowError(exception);
                    }
                }
                else
                {
                    using (var backgroundWorker = new BackgroundWorker())
                    {
                        backgroundWorker.DoWork += (sender, e) =>
                        {
                            CheckUpdate();
                        };
                        backgroundWorker.RunWorkerCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                ShowError(e.Error);
                            }
                            Running = false;
                        };
                        backgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }
        private static void CheckUpdate()
        {
            BaseUri = new Uri(AppCastURL);
            UpdateInfoEventArgs args;
            using (UpdaterWebClient client = GetWebClient())
            {
                string xml = client.DownloadString(BaseUri);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
                XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xml)) { XmlResolver = null };
                args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);
            }
            if (string.IsNullOrEmpty(args.LauncherInfo.LatestVersion) || string.IsNullOrEmpty(args.LauncherInfo.DownloadURL) ||
                string.IsNullOrEmpty(args.ScriptInfo.LatestHash.Value) || string.IsNullOrEmpty(args.ScriptInfo.DownloadURL) ||
                string.IsNullOrEmpty(args.ResourceInfo.LatestVersion) || string.IsNullOrEmpty(args.ResourceInfo.DownloadURL))
            {
                throw new MissingFieldException();
            }
            args.LauncherInfo.InstalledVersion = InstalledLauncherVersion;
            args.ScriptInfo.InstalledHash = InstalledScriptHash ?? GameHash.GetHash("cn.file", args.ScriptInfo.LatestHash);
            args.ResourceInfo.InstalledVersion = InstalledResourceVersion;
            Item[] updateItems = { args.LauncherInfo, args.ScriptInfo, args.ResourceInfo };
            foreach (var item in updateItems)
            {
                item.IsUpdateAvailable = item.LatestHash == null
                    ? new Version(item.LatestVersion) > item.InstalledVersion
                    : !item.InstalledHash.Equals(item.LatestHash);
            }
            UpdateCheckedEvent?.Invoke(args);
        }
        private static void ShowError(Exception exception)
        {
            UpdateCheckedEvent?.Invoke(new UpdateInfoEventArgs { Error = exception });
        }

        /// <summary>
        /// 开始下载更新。
        /// </summary>
        public static bool DownloadUpdate(UpdateInfoEventArgs args, System.Windows.Window owner)
        {
            try
            {
                return (bool)new Dialogs.UpdateWindow(args, owner).ShowDialog();
            }
            catch (TargetInvocationException)
            {
            }
            return false;
        }
        internal static UpdaterWebClient GetWebClient()
        {
            UpdaterWebClient webClient = new UpdaterWebClient
            {
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
            };
            return webClient;
        }
    }
    /// <inheritdoc />
    public class UpdaterWebClient : WebClient
    {
        /// <summary>
        /// 在任意重定向后的 ResponseUri。
        /// </summary>
        public Uri ResponseUri;
        /// <inheritdoc />
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse webResponse = base.GetWebResponse(request, result);
            ResponseUri = webResponse.ResponseUri;
            return webResponse;
        }
    }
}
