using System.Collections.Generic;
using System.ComponentModel;

namespace UminekoLauncher.Models
{
    /// <summary>
    /// 游戏的显示模式。
    /// </summary>
    internal enum DisplayMode
    { Window, FullScreen, Auto }

    /// <summary>
    /// 游戏的显示分辨率。
    /// </summary>
    internal enum DisplayResolution
    {
        [Description("1280x720")]
        x720,

        [Description("1366x768")]
        x768,

        [Description("1440x810")]
        x810,

        [Description("1600x900")]
        x900,

        [Description("1920x1080")]
        x1080,

        [Description("2560x1440")]
        x1440,

        [Description("Custom")]
        Custom
    }

    /// <summary>
    /// 表示游戏配置。
    /// </summary>
    internal class ConfigModel
    {
        /// <summary>
        /// 当 <see cref="DisplayResolution"/> 设置为 <see cref="DisplayResolution.Custom"/> 时，表示自定义分辨率的值。
        /// </summary>
        public string CustomDisplayResolution { get; set; }

        /// <summary>
        /// 获取或设置游戏显示模式。
        /// </summary>
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Auto;

        /// <summary>
        /// 获取或设置游戏分辨率
        /// </summary>
        public DisplayResolution DisplayResolution { get; set; } = DisplayResolution.x1080;

        /// <summary>
        /// 获取或设置当前游戏脚本。
        /// </summary>
        public string GameScript { get; set; } = "cn.file";

        /// <summary>
        /// 获取或设置经典OP配置。
        /// </summary>
        public bool LegacyOp { get; set; } = false;

        /// <summary>
        /// 获取或设置包含其他游戏配置的列表。
        /// </summary>
        public List<string> UnsupportedConfigs { get; set; }

        /// <summary>
        /// 获取或设置缩放全屏配置。
        /// </summary>
        public bool Scale { get; set; } = false;
    }
}