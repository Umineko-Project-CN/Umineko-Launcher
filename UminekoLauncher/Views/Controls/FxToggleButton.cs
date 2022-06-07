using System.ComponentModel;
using System.Windows;

namespace UminekoLauncher.Views.Controls
{
    internal class FxToggleButton : FxButton
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FxToggleButton), new PropertyMetadata(false));

        static FxToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FxToggleButton), new FrameworkPropertyMetadata(typeof(FxToggleButton)));
        }

        public FxToggleButton() : base()
        {
            Click += (sender, e) => IsChecked = !IsChecked;
        }

        [Description("获取或设置开关按钮的开关状态。"), Category("公共")]
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
    }
}