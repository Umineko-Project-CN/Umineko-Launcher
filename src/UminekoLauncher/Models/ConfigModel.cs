using System.ComponentModel;

namespace UminekoLauncher.Models
{
    /// <summary>
    /// 游戏的显示模式。
    /// </summary>
    internal enum DisplayMode
    {
        Window,
        FullScreen,
        Auto
    }

    /// <summary>
    /// 游戏的脚本语言。
    /// </summary>
    internal enum Language
    {
        CHS,
        CHT,
        EN,
        Other
    }

    /// <summary>
    /// 游戏的显示分辨率。
    /// </summary>
    internal enum DisplayResolution
    {
        x720,
        x768,
        x810,
        x900,
        x1080,
        x1440,
        Custom
    }
}