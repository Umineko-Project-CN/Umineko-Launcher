using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UminekoLauncher.Views.ValueConverters
{
    class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            return Enum.Parse(targetType, value.ToString());
        }
    }
}
