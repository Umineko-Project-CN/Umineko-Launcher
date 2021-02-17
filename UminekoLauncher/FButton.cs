using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UminekoLauncher
{
    public class FButton : Button
    {
        static FButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FButton), new FrameworkPropertyMetadata(typeof(FButton)));
        }

        /// <summary>
        /// 设置 FButton 的圆角
        /// </summary>
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

        [Description("表示按钮的图标。参考 Material Icons Codepoints。"), Category("公共")]
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(FButton), new PropertyMetadata());

        [Description("表示按钮图标的外边距。"), Category("公共")]
        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(FButton), new PropertyMetadata());

        [Description("表示按钮图标的大小。"), Category("公共")]
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(FButton), new PropertyMetadata(32.0));
    }
}
