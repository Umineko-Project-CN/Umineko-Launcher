using System;
using System.Collections.Generic;
using System.IO;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services
{
    /// <summary>
    /// 游戏配置服务。
    /// </summary>
    internal class Config
    {
        private readonly string _path;
        private Language _language = Language.CHS;
        private static Config _config = null;

        /// <summary>
        /// 获取全局配置。
        /// </summary>
        /// <returns><see cref="Config"/> 全局配置对象。</returns>
        public static Config GetConfig()
        {
            if (_config != null)
            {
                return _config;
            }
            return _config = new Config();
        }

        /// <summary>
        /// 判断配置文件是否存在。
        /// </summary>
        /// <returns>若文件存在，则为 <see cref="bool">true</see>，否则为 <see cref="bool">false</see>。</returns>
        public bool FileExists() => File.Exists(_path);

        /// <summary>
        /// 当 <see cref="DisplayResolution"/> 设置为 <see cref="DisplayResolution.Custom"/> 时，表示自定义分辨率的值。
        /// </summary>
        public string CustomDisplayResolution { get; set; } = null;

        /// <summary>
        /// 获取或设置游戏显示模式。
        /// </summary>
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Auto;

        /// <summary>
        /// 获取或设置游戏分辨率。
        /// </summary>
        public DisplayResolution DisplayResolution { get; set; } = DisplayResolution.x1080;

        /// <summary>
        /// 获取或设置当前游戏脚本。
        /// </summary>
        public string GameScript { get; set; } = "cn.file";

        /// <summary>
        /// 获取或设置当前游戏语言。
        /// </summary>
        public Language Language
        {
            get => _language;
            set
            {
                string ext = GameScript.Split('.')[1];
                switch (value)
                {
                    case Language.CHS:
                        GameScript = $"cn.{ext}";
                        break;

                    case Language.CHT:
                        if (!Misc.LangCHTResourceExist())
                        {
                            return;
                        }
                        GameScript = $"cht.{ext}";
                        break;

                    case Language.EN:
                        GameScript = $"en.{ext}";
                        break;

                    default:
                        break;
                }
                _language = value;
            }
        }

        /// <summary>
        /// 获取或设置经典OP配置。
        /// </summary>
        public bool LegacyOp { get; set; } = false;

        /// <summary>
        /// 获取或设置包含其他游戏配置的列表。
        /// </summary>
        public List<string> UnsupportedConfigs { get; set; } = new List<string>();

        /// <summary>
        /// 获取或设置缩放全屏配置。
        /// </summary>
        public bool Scale { get; set; } = false;

        /// <summary>
        /// 使用指定的路径初始化配置。
        /// </summary>
        /// <param name="path"></param>
        public Config(string path = "ons.cfg")
        {
            _path = path;
        }

        /// <summary>
        /// 从文件中载入游戏配置。
        /// </summary>
        public void Load()
        {
            string[] rawConfigs = File.ReadAllLines(_path);
            foreach (var rawConfig in rawConfigs)
            {
                string line = rawConfig.Trim();
                // 游戏脚本。
                if (line.StartsWith("game-script"))
                {
                    GameScript = line.Split('=')[1];
                    switch (GameScript.Split('.')[0])
                    {
                        case "cn":
                            Language = Language.CHS;
                            break;

                        case "cht":
                            Language = Language.CHT;
                            break;

                        case "en":
                            Language = Language.EN;
                            break;

                        default:
                            Language = Language.Other;
                            break;
                    }
                    continue;
                }
                // 旧版OP。
                if (line.StartsWith("env[legacy_op]"))
                {
                    LegacyOp = Convert.ToBoolean(line.Split('=')[1]);
                    continue;
                }
                // 分辨率。
                if (line.StartsWith("window-width"))
                {
                    string str = line.Split('=')[1];
                    switch (str)
                    {
                        case "1280":
                            DisplayResolution = DisplayResolution.x720;
                            break;

                        case "1366":
                            DisplayResolution = DisplayResolution.x768;
                            break;

                        case "1440":
                            DisplayResolution = DisplayResolution.x810;
                            break;

                        case "1600":
                            DisplayResolution = DisplayResolution.x900;
                            break;

                        case "1920":
                            DisplayResolution = DisplayResolution.x1080;
                            break;

                        case "2560":
                            DisplayResolution = DisplayResolution.x1440;
                            break;

                        default:
                            DisplayResolution = DisplayResolution.Custom;
                            CustomDisplayResolution = str;
                            break;
                    }
                    continue;
                }
                // 显示模式。
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
                // 缩放全屏。
                if (line.StartsWith("scale"))
                {
                    Scale = true;
                    continue;
                }
                UnsupportedConfigs.Add(line);
            }
        }

        /// <summary>
        /// 保存游戏配置至文件。
        /// </summary>
        public void Save()
        {
            var configStrings = new List<string>
            {
                // 游戏脚本
                "game-script=" + GameScript,
                // 旧版OP
                "env[legacy_op]=" + LegacyOp.ToString().ToLower()
            };
            // 分辨率
            string displayResolution = "window-width=";
            switch (DisplayResolution)
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
                    if (string.IsNullOrEmpty(CustomDisplayResolution))
                    {
                        goto default;
                    }
                    else
                    {
                        displayResolution += CustomDisplayResolution;
                    }
                    break;

                default:
                    goto case DisplayResolution.x1080;
            }
            configStrings.Add(displayResolution);
            // 显示模式
            switch (DisplayMode)
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
            if (Scale)
            {
                configStrings.Add("scale");
            }
            if (UnsupportedConfigs != null)
            {
                configStrings.AddRange(UnsupportedConfigs);
            }
            File.WriteAllLines(_path, configStrings);
        }
    }
}