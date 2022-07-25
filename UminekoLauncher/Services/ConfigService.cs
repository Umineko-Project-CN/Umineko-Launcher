using System;
using System.Collections.Generic;
using System.IO;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services
{
    /// <summary>
    /// 游戏配置服务。
    /// </summary>
    internal static class ConfigService
    {
        private const string ConfigPath = "ons.cfg";

        /// <summary>
        /// 判断配置文件是否存在。
        /// </summary>
        /// <returns>若文件存在，则为 <see cref="bool">true</see>，否则为 <see cref="bool">false</see>。</returns>
        public static bool FileExists() => File.Exists(ConfigPath);

        /// <summary>
        /// 从文件中载入游戏配置。
        /// </summary>
        public static ConfigModel LoadConfig()
        {
            var config = new ConfigModel();
            string[] rawConfigs = File.ReadAllLines(ConfigPath);
            foreach (var rawConfig in rawConfigs)
            {
                string line = rawConfig.Trim();
                // 游戏脚本。
                if (line.StartsWith("game-script"))
                {
                    config.GameScript = line.Split('=')[1];
                    continue;
                }
                // 旧版OP。
                if (line.StartsWith("env[legacy_op]"))
                {
                    config.LegacyOp = Convert.ToBoolean(line.Split('=')[1]);
                    continue;
                }
                // 分辨率。
                if (line.StartsWith("window-width"))
                {
                    string str = line.Split('=')[1];
                    switch (str)
                    {
                        case "1280":
                            config.DisplayResolution = DisplayResolution.x720;
                            break;

                        case "1366":
                            config.DisplayResolution = DisplayResolution.x768;
                            break;

                        case "1440":
                            config.DisplayResolution = DisplayResolution.x810;
                            break;

                        case "1600":
                            config.DisplayResolution = DisplayResolution.x900;
                            break;

                        case "1920":
                            config.DisplayResolution = DisplayResolution.x1080;
                            break;

                        case "2560":
                            config.DisplayResolution = DisplayResolution.x1440;
                            break;

                        default:
                            config.DisplayResolution = DisplayResolution.Custom;
                            config.CustomDisplayResolution = str;
                            break;
                    }
                    continue;
                }
                // 显示模式。
                if (line.StartsWith("window"))
                {
                    config.DisplayMode = DisplayMode.Window;
                    continue;
                }
                if (line.StartsWith("fullscreen"))
                {
                    config.DisplayMode = DisplayMode.FullScreen;
                    continue;
                }
                // 缩放全屏。
                if (line.StartsWith("scale"))
                {
                    config.Scale = true;
                    continue;
                }
                config.UnsupportedConfigs.Add(line);
            }
            return config;
        }

        /// <summary>
        /// 保存游戏配置至文件。
        /// </summary>
        public static void SaveConfig(ConfigModel config)
        {
            var configStrings = new List<string>
            {
                // 游戏脚本
                "game-script=" + config.GameScript,
                // 旧版OP
                "env[legacy_op]=" + config.LegacyOp.ToString().ToLower()
            };
            // 分辨率
            string displayResolution = "window-width=";
            switch (config.DisplayResolution)
            {
                case DisplayResolution.x720:
                    displayResolution += "1280";
                    break;

                case DisplayResolution.x768:
                    displayResolution += "1366";
                    break;

                case DisplayResolution.x810:
                    displayResolution += "1440";
                    break;

                case DisplayResolution.x900:
                    displayResolution += "1600";
                    break;

                case DisplayResolution.x1080:
                    displayResolution += "1920";
                    break;

                case DisplayResolution.x1440:
                    displayResolution += "2560";
                    break;

                case DisplayResolution.Custom:
                    if (string.IsNullOrEmpty(config.CustomDisplayResolution))
                    {
                        goto default;
                    }
                    else
                    {
                        displayResolution += config.CustomDisplayResolution;
                    }
                    break;

                default:
                    goto case DisplayResolution.x1080;
            }
            configStrings.Add(displayResolution);
            // 显示模式
            switch (config.DisplayMode)
            {
                case DisplayMode.Window:
                    configStrings.Add("window");
                    break;

                case DisplayMode.FullScreen:
                    configStrings.Add("fullscreen");
                    break;

                case DisplayMode.Auto:
                default:
                    break;
            }
            // 缩放全屏
            if (config.Scale)
            {
                configStrings.Add("scale");
            }
            if (config.UnsupportedConfigs != null)
            {
                configStrings.AddRange(config.UnsupportedConfigs);
            }
            File.WriteAllLines(ConfigPath, configStrings);
        }
    }
}