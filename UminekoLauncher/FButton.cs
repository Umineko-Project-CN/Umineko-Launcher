using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace UminekoLauncher
{
    public class FButton : Button
    {
        static FButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FButton), new FrameworkPropertyMetadata(typeof(FButton)));
        }

        [Description("表示按钮边框的圆角半径。不能为负值。"), Category("外观")]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FButton), new PropertyMetadata(new CornerRadius(5)));

        [Description("获取或设置要应用到按钮内容的效果。"), Category("外观")]
        public Effect ContentEffect
        {
            get { return (Effect)GetValue(ContentEffectProperty); }
            set { SetValue(ContentEffectProperty, value); }
        }
        public static readonly DependencyProperty ContentEffectProperty =
            DependencyProperty.Register("ContentEffect", typeof(Effect), typeof(FButton), new PropertyMetadata());

        [Description("表示按钮的图标。参考 Material Icons Codepoints。"), Category("文本")]
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(FButton), new PropertyMetadata());

        [Description("表示按钮图标的外边距。"), Category("文本")]
        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(FButton), new PropertyMetadata());

        [Description("表示按钮图标的大小。"), Category("文本")]
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(FButton), new PropertyMetadata(32.0));

        [Description("表示按钮文本的外边距。"), Category("文本")]
        public Thickness FontMargin
        {
            get { return (Thickness)GetValue(FontMarginProperty); }
            set { SetValue(FontMarginProperty, value); }
        }
        public static readonly DependencyProperty FontMarginProperty =
            DependencyProperty.Register("FontMargin", typeof(Thickness), typeof(FButton), new PropertyMetadata());
    }
}
