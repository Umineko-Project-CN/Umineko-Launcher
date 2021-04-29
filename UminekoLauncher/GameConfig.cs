using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace UminekoLauncher
{
    /// <summary>
    /// 游戏的显示模式。
    /// </summary>
    public enum DisplayMode { Window, FullScreen, Auto }
    /// <summary>
    /// 游戏的显示分辨率。
    /// </summary>
    public enum DisplayResolution
    {
        [Description("1280x720")]
        x1280,
        [Description("1366x768")]
        x1366,
        [Description("1440x810")]
        x1440,
        [Description("1600x900")]
        x1600,
        [Description("1920x1080")]
        x1920,
        [Description("2560x1440")]
        x2560,
        [Description("Custom")]
        Custom
    }
    /// <summary>
    /// 该静态类用于执行游戏配置的相关操作。
    /// </summary>
    static class GameConfig
    {
        private static List<string> config;

        /// <summary>
        /// 获取游戏配置文件（ons.cfg）所在路径。
        /// </summary>
        public static string ConfigPath { get; } = "ons.cfg";
        /// <summary>
        /// 若当前已读取配置，则该值为真。
        /// </summary>
        public static bool IsLoaded { get; private set; } = false;

        /// <summary>
        /// 获取或设置当前所使用的游戏脚本。
        /// </summary>
        public static string GameScript { get; set; } = "cn.file";

        /// <summary>
        /// 获取或设置是否启用经典开场。
        /// </summary>
        public static bool IsLegacyOpEnabled { get; set; } = false;

        /// <summary>
        /// 获取或设置游戏分辨率。
        /// </summary>
        public static DisplayResolution DisplayResolution { get; set; } = DisplayResolution.x1920;

        /// <summary>
        /// 获取或设置自定义游戏分辨率。此属性默认为空。
        /// </summary>
        public static string CustomDisplayResolution { get; set; } = null;

        /// <summary>
        /// 获取或设置游戏显示模式。
        /// </summary>
        public static DisplayMode DisplayMode { get; set; } = DisplayMode.Auto;

        /// <summary>
        /// 获取或设置是否缩放全屏。
        /// </summary>
        public static bool IsScaleEnabled { get; set; } = false;

        /// <summary>
        /// 从配置文件（ons.cfg）中载入游戏配置。
        /// </summary>
        public static void LoadConfig()
        {
            config = File.ReadAllLines(ConfigPath).ToList();
            for (int i = 0; i < config.Count; i++)
            {
                string line = config[i].Trim();
                config[i] = null;

                #region 游戏脚本
                if (line.StartsWith("game-script"))
                {
                    GameScript = line.Split('=')[1];
                    continue;
                }
                #endregion

                #region 旧版OP
                if (line.StartsWith("env[legacy_op]"))
                {
                    IsLegacyOpEnabled = Convert.ToBoolean(line.Split('=')[1]);
                    continue;
                }
                #endregion

                #region 分辨率
                if (line.StartsWith("window-width"))
                {
                    string strValue = line.Split('=')[1];
                    switch (strValue)
                    {
                        case "1280":
                            DisplayResolution = DisplayResolution.x1280;
                            break;
                        case "1366":
                            DisplayResolution = DisplayResolution.x1366;
                            break;
                        case "1440":
                            DisplayResolution = DisplayResolution.x1440;
                            break;
                        case "1600":
                            DisplayResolution = DisplayResolution.x1600;
                            break;
                        case "1920":
                            DisplayResolution = DisplayResolution.x1920;
                            break;
                        case "2560":
                            DisplayResolution = DisplayResolution.x2560;
                            break;
                        default:
                            DisplayResolution = DisplayResolution.Custom;
                            CustomDisplayResolution = strValue;
                            break;
                    }
                    continue;
                }
                #endregion

                #region 显示模式
                if (line.StartsWith("window"))
                {
                    DisplayMode = DisplayMode.Window;
                    continue;
                }
                if (line.StartsWith("fullscreen"))
                {
                    DisplayMode = DisplayMode.FullScreen;
                    continue;
                }
                #endregion

                #region 缩放全屏
                if (line.StartsWith("scale"))
                {
                    IsScaleEnabled = true;
                    continue;
                }
                #endregion

                config[i] = line;
            }
            config.RemoveAll(line => line == null);
            IsLoaded = true;
        }

        /// <summary>
        /// 保存游戏配置至配置文件（ons.cfg）。
        /// </summary>
        public static void SaveConfig()
        {

            #region 游戏脚本
            config.Add("game-script=" + GameScript);
            #endregion

            #region 旧版OP
            config.Add("env[legacy_op]=" + IsLegacyOpEnabled.ToString().ToLower());
            #endregion

            #region 分辨率
            string displayResolution = "window-width=";
            if (DisplayResolution == DisplayResolution.Custom && !string.IsNullOrEmpty(CustomDisplayResolution))
            {
                displayResolution += CustomDisplayResolution;
            }
            else
            {
                switch (DisplayResolution)
                {
                    case DisplayResolution.x1280:
                        displayResolution += "1280";
                        break;
                    case DisplayResolution.x1366:
                        displayResolution += "1366";
                        break;
                    case DisplayResolution.x1440:
                        displayResolution += "1440";
                        break;
                    case DisplayResolution.x1600:
                        displayResolution += "1600";
                        break;
                    case DisplayResolution.x1920:
                        displayResolution += "1920";
                        break;
                    case DisplayResolution.x2560:
                        displayResolution += "2560";
                        break;
                    default:
                        displayResolution += "1920";
                        break;
                }
            }
            config.Add(displayResolution);
            #endregion

            #region 显示模式
            switch (DisplayMode)
            {
                case DisplayMode.Window:
                    config.Add("window");
                    break;
                case DisplayMode.FullScreen:
                    config.Add("fullscreen");
                    break;
                case DisplayMode.Auto:
                default:
                    break;
            }
            #endregion

            #region 缩放全屏
            if (IsScaleEnabled)
            {
                config.Add("scale");
            }
            #endregion

            File.WriteAllLines(ConfigPath, config);
        }
    }
}
