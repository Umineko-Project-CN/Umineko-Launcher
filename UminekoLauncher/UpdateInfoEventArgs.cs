using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace UminekoLauncher
{
    /// <summary>
    /// 该类表示更新信息，应从服务器获取的 XML 实例化。
    /// </summary>
    [XmlRoot("UpdateInfo")]
    public class UpdateInfoEventArgs : EventArgs
    {
        private string _changelogURL;
        private string _extraLink;

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
        /// 启动器更新信息。
        /// </summary>
        [XmlElement("Launcher")]
        public Item LauncherInfo { get; set; }

        /// <summary>
        /// 脚本更新信息。
        /// </summary>
        [XmlElement("Script")]
        public Item ScriptInfo { get; set; }

        /// <summary>
        /// 资源更新信息。
        /// </summary>
        [XmlElement("Resource")]
        public Item ResourceInfo { get; set; }

        /// <summary>
        /// 额外链接。
        /// </summary>
        [XmlElement("ExtraLink")]
        public string ExtraLink
        {
            get => GetURL(Updater.BaseUri, _extraLink);
            set => _extraLink = value;
        }

        /// <summary>
        /// 通过此方法来确保 URL 可用。
        /// </summary>
        /// <param name="baseUri">基础 Uri。</param>
        /// <param name="url">要处理的 URL。</param>
        /// <returns>经处理后的 URL。</returns>
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
        /// 已安装文件的校验值。
        /// </summary>
        public Checksum InstalledHash { get; set; }

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
        /// 最新版本文件的校验值。
        /// </summary>
        [XmlElement("Hash")]
        public Checksum LatestHash { get; set; }

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
        /// 更新包的校验值。
        /// </summary>
        [XmlElement("Checksum")]
        public Checksum Checksum { get; set; }
    }
    /// <summary>
    /// 获取 XML 校验码值所使用的类。
    /// </summary>
    public class Checksum
    {
        private string _value;
        /// <summary>
        /// 文件校验码值。
        /// </summary>
        [XmlText]
        public string Value
        {
            get => _value;
            set => _value = value.ToUpperInvariant();
        }

        /// <summary>
        /// 校验码算法。
        /// </summary>
        [XmlAttribute("Algorithm")]
        public string HashingAlgorithm { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Checksum checksum &&
                   Value == checksum.Value &&
                   HashingAlgorithm == checksum.HashingAlgorithm;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }
    }
}
