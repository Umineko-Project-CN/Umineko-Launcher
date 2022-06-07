using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UminekoLauncher.Services;

namespace UminekoLauncher.Views.ValueConverters
{
    internal class UpdateStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            UpdateStatus status = (UpdateStatus)value;
            string flag = parameter as string;
            switch (flag)
            {
                case "UpdateStatus":
                    switch (status)
                    {
                        case UpdateStatus.NotStarted:
                            return "正在检测";

                        case UpdateStatus.UpToDate:
                            return "最新版本";

                        case UpdateStatus.Error:
                            return "更新失败";

                        default:
                            return "需要更新";
                    }
                case "ActionIcon":
                    switch (status)
                    {
                        case UpdateStatus.ReadyToUpdate:
                            return "";

                        case UpdateStatus.NeedManualUpdate:
                            return "";

                        default:
                            return "";
                    }
                case "ActionContent":
                    switch (status)
                    {
                        case UpdateStatus.ReadyToUpdate:
                            return "获取更新";

                        case UpdateStatus.NeedManualUpdate:
                            return "前往下载";

                        default:
                            return "开始游戏";
                    }
                case "NotificationLight":
                    switch (status)
                    {
                        case UpdateStatus.UpToDate:
                            return Application.Current.FindResource("GreenLight");

                        case UpdateStatus.Error:
                            return Application.Current.FindResource("RedLight");

                        default:
                            return Application.Current.FindResource("YellowLight");
                    }
                case "StatusFontWeight":
                    if (status == UpdateStatus.NotStarted || status == UpdateStatus.UpToDate)
                    {
                        return FontWeights.Normal;
                    }
                    else
                    {
                        return FontWeights.Bold;
                    }
                case "StatusForeground":
                    if (status == UpdateStatus.NotStarted || status == UpdateStatus.UpToDate)
                    {
                        return Brushes.DarkGray;
                    }
                    else
                    {
                        return Brushes.White;
                    }
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}