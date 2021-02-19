using System;
using System.Xml.Serialization;

namespace UminekoLauncher
{
    [XmlRoot("UpdateInfo")]
    public class UpdateInfoEventArgs : EventArgs
    {
        private string _changelogURL;
        /// <summary>
        /// 若检查更新时出错，则此项属性不为空。
        /// </summary>
        [XmlIgnore]
        public Exception Error { get; set; }
        /// <summary>
        /// 更新日志的 URL。
        /// </summary>
        [XmlElement("Changelog")]
        public string ChangelogURL
        {
            get => GetURL(Updater.BaseUri, _changelogURL);
            set => _changelogURL = value;
        }
        /// <summary>
        /// 游戏更新信息。
        /// </summary>
        [XmlElement("Game")]
        public Item GameInfo { get; set; }
        /// <summary>
        /// 启动器更新信息。
        /// </summary>
        [XmlElement("Launcher")]
        public Item LauncherInfo { get; set; }
        public static string GetURL(Uri baseUri, string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                Uri uri = new Uri(baseUri, url);
                if (uri.IsAbsoluteUri)
                {
                    url = uri.AbsoluteUri;
                }
            }
            return url;
        }
    }
    /// <summary>
    /// 待更新项目类。
    /// </summary>
    public class Item
    {
        private string _downloadURL;
        /// <summary>
        /// 已安装的版本。
        /// </summary>
        public Version InstalledVersion { get; set; }
        /// <summary>
        /// 若更新可用则为真。
        /// </summary>
        public bool IsUpdateAvailable { get; set; }
        /// <summary>
        /// 可用的最新版本。
        /// </summary>
        [XmlElement("Version")]
        public string LatestVersion { get; set; }
        /// <summary>
        /// 更新文件的下载 URL。
        /// </summary>
        [XmlElement("URL")]
        public string DownloadURL
        {
            get => UpdateInfoEventArgs.GetURL(Updater.BaseUri, _downloadURL);
            set => _downloadURL = value;
        }
        /// <summary>
        /// 更新文件的校验码。
        /// </summary>
        [XmlElement("Checksum")]
        public Checksum Checksum { get; set; }
    }
    /// <summary>
    /// 获取 XML 校验码值所使用的类。
    /// </summary>
    public class Checksum
    {
        /// <summary>
        /// 文件校验码值。
        /// </summary>
        [XmlText]
        public string Value { get; set; }
        /// <summary>
        /// 校验码算法。
        /// </summary>
        [XmlAttribute("Algorithm")]
        public string HashingAlgorithm { get; set; }
    }
}
