using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace UminekoLauncher.Views.Controls
{
    internal class FxButton : Button
    {
        public static readonly DependencyProperty ContentEffectProperty =
            DependencyProperty.Register(nameof(ContentEffect), typeof(Effect), typeof(FxButton), new PropertyMetadata());

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(FxButton), new PropertyMetadata(new CornerRadius(5)));

        public static readonly DependencyProperty FontMarginProperty =
            DependencyProperty.Register(nameof(FontMargin), typeof(Thickness), typeof(FxButton), new PropertyMetadata());

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(nameof(IconMargin), typeof(Thickness), typeof(FxButton), new PropertyMetadata());

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FxButton), new PropertyMetadata());

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(FxButton), new PropertyMetadata(32.0));

        public static readonly DependencyProperty TextSpacingProperty =
            DependencyProperty.Register(nameof(TextSpacing), typeof(bool), typeof(FxButton), new PropertyMetadata());

        static FxButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FxButton), new FrameworkPropertyMetadata(typeof(FxButton)));
        }

        [Description("获取或设置要应用到按钮内容的效果。"), Category("外观")]
        public Effect ContentEffect
        {
            get => (Effect)GetValue(ContentEffectProperty);
            set => SetValue(ContentEffectProperty, value);
        }

        [Description("表示按钮边框的圆角半径。不能为负值。"), Category("外观")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        [Description("表示按钮文本的外边距。"), Category("文本")]
        public Thickness FontMargin
        {
            get => (Thickness)GetValue(FontMarginProperty);
            set => SetValue(FontMarginProperty, value);
        }

        [Description("表示按钮的图标。参考 Material Icons Codepoints。"), Category("文本")]
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        [Description("表示按钮图标的外边距。"), Category("文本")]
        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        [Description("表示按钮图标的大小。"), Category("文本")]
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        [Description("获取或设置是否应用按钮文本间距"), Category("文本")]
        public bool TextSpacing
        {
            get => (bool)GetValue(TextSpacingProperty);
            set => SetValue(TextSpacingProperty, value);
        }
    }

    internal class TextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return DependencyProperty.UnsetValue;
            }
            if ((bool)values[1])
            {
                var str = values[0] as string;
                return str?.Aggregate(string.Empty, (a, b) => $"{a} {b}");
            }
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}