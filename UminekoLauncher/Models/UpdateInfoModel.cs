using System.Collections.Generic;
using System.Xml.Serialization;

namespace UminekoLauncher.Models
{
    /// <summary>
    /// 获取 XML 校验码值所使用的类。
    /// </summary>
    public class Checksum
    {
        private string _value;

        /// <summary>
        /// 校验码算法。
        /// </summary>
        [XmlAttribute("Algorithm")]
        public string HashAlgorithm { get; set; }

        /// <summary>
        /// 文件校验码值。
        /// </summary>
        [XmlText]
        public string Value
        {
            get => _value;
            set => _value = value.ToUpperInvariant();
        }

        public static bool operator !=(Checksum a, Checksum b) => !a.Equals(b);

        public static bool operator ==(Checksum a, Checksum b) => a.Equals(b);

        public override bool Equals(object obj)
        {
            return obj is Checksum checksum &&
                   Value == checksum.Value &&
                   HashAlgorithm == checksum.HashAlgorithm;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }
    }

    /// <summary>
    /// 该类表示更新信息，应从服务器获取的 XML 实例化。
    /// </summary>
    [XmlRoot("UpdateInfo")]
    public class UpdateInfoModel
    {
        /// <summary>
        /// 更新日志的 URL。
        /// </summary>
        [XmlElement("Changelog")]
        public string ChangelogUrl { get; set; }

        /// <summary>
        /// 额外链接。
        /// </summary>
        [XmlElement("ExtraLink")]
        public string ExtraLink { get; set; }

        /// <summary>
        /// 启动器更新信息。
        /// </summary>
        [XmlElement("Launcher")]
        public Item LauncherInfo { get; set; }

        /// <summary>
        /// 资源更新信息。
        /// </summary>
        [XmlElement("Resource")]
        public Item ResourceInfo { get; set; }

        /// <summary>
        /// 脚本更新信息。
        /// </summary>
        [XmlElement("Script")]
        public Item ScriptInfo { get; set; }

        /// <summary>
        /// 待更新项目信息类。
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 更新文件的下载 URL。
            /// </summary>
            [XmlElement("URL")]
            public string DownloadUrl { get; set; }

            /// <summary>
            /// 最新版本文件的校验值。
            /// </summary>
            [XmlElement("Hash")]
            public Checksum FileHash { get; set; }

            /// <summary>
            /// 更新包的校验值。
            /// </summary>
            [XmlElement("Checksum")]
            public Checksum PackageHash { get; set; }

            /// <summary>
            /// 可用的最新版本。
            /// </summary>
            [XmlElement("Version")]
            public string Version { get; set; }
        }
    }
}