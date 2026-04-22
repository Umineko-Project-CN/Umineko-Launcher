using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UminekoLauncher.Views.ValueConverters
{
    internal class MainAvailablityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool canAction = (bool)values[0],
                     configPopupOpen = (bool)values[1],
                     aboutPopupOpen = (bool)values[2];
                if (!canAction || configPopupOpen || aboutPopupOpen)
                    return false;
                else
                    return true;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}