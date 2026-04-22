using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UminekoLauncher.Services;
using UminekoLauncher.Localization;

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
                            return Lang.Checking;

                        case UpdateStatus.UpToDate:
                            return Lang.Latest;

                        case UpdateStatus.Error:
                            return Lang.Failed;

                        default:
                            return Lang.Updatable;
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
                            return Lang.Update;

                        case UpdateStatus.NeedManualUpdate:
                            return Lang.Download;

                        default:
                            return Lang.Start;
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