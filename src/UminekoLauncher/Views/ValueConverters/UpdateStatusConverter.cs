using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UminekoLauncher.Localization;
using UminekoLauncher.Services;

namespace UminekoLauncher.Views.ValueConverters;

internal class UpdateStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return DependencyProperty.UnsetValue;
        }
        UpdateStatus status = (UpdateStatus)value;
        string flag = (string)parameter;
        return flag switch
        {
            "UpdateStatus" => status switch
            {
                UpdateStatus.NotStarted => Lang.Checking,
                UpdateStatus.UpToDate => Lang.Latest,
                UpdateStatus.Error => Lang.Failed,
                _ => Lang.Updatable,
            },
            "ActionIcon" => status switch
            {
                UpdateStatus.ReadyToUpdate => "",
                UpdateStatus.NeedManualUpdate => "",
                _ => "",
            },
            "ActionContent" => status switch
            {
                UpdateStatus.ReadyToUpdate => Lang.Update,
                UpdateStatus.NeedManualUpdate => Lang.Download,
                _ => Lang.Start,
            },
            "NotificationLight" => status switch
            {
                UpdateStatus.UpToDate => Application.Current.FindResource("GreenLight"),
                UpdateStatus.Error => Application.Current.FindResource("RedLight"),
                _ => Application.Current.FindResource("YellowLight"),
            },
            "StatusFontWeight" => status == UpdateStatus.NotStarted
            || status == UpdateStatus.UpToDate
                ? FontWeights.Normal
                : FontWeights.Bold,
            "StatusForeground" => status == UpdateStatus.NotStarted
            || status == UpdateStatus.UpToDate
                ? Brushes.DarkGray
                : Brushes.White,
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
