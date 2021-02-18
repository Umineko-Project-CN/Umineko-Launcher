using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace UminekoLauncher
{
    public enum DisplayMode { Window, FullScreen, Auto }
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
        x2560
    }
    static class GameConfig
    {
        private static List<string> config;
        public static bool IsLoaded { get; set; } = false;
        public static Version GameVersion { get; set; }
        public static bool IsLegacyOpEnabled { get; set; }
        public static DisplayResolution DisplayResolution { get; set; }
        public static DisplayMode DisplayMode { get; set; }
        public static bool IsScaleEnabled { get; set; }

        public static void LoadConfig(string path)
        {

            #region 默认值
            GameVersion = new Version("8.21.2.9");
            IsLegacyOpEnabled = false;
            DisplayMode = DisplayMode.Auto;
            DisplayResolution = DisplayResolution.x1920;
            IsScaleEnabled = false;
            #endregion

            config = File.ReadAllLines(path).ToList();
            for (int i = 0; i < config.Count; i++)
            {
                string line = config[i].Trim();
                config[i] = null;

                #region 更新版本
                if (line.StartsWith("#game-version"))
                {
                    GameVersion = new Version(line.Split('=')[1]);
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
                    switch (line.Split('=')[1])
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
                            DisplayResolution = DisplayResolution.x1920;
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
        public static void SaveConfig(string path)
        {
            #region 更新版本
            config.Insert(0, "#game-version=" + GameVersion.ToString());
            #endregion

            #region 旧版OP
            config.Add("env[legacy_op]=" + IsLegacyOpEnabled.ToString().ToLower());
            #endregion

            #region 分辨率
            string displayResolution = "window-width=";
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

            File.WriteAllLines(path, config);
        }
    }
}
