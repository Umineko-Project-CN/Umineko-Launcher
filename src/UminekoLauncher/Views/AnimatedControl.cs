using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace UminekoLauncher.Views
{
    public class AnimatedControl : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(AnimatedControl), new PropertyMetadata(false));

        protected readonly BlurEffect _blurEffect = new BlurEffect() { Radius = 20 };

        protected readonly DoubleAnimation _blurFadeIn = new DoubleAnimation()
        {
            From = 20,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2)
        };

        protected readonly DoubleAnimation _blurFadeOut = new DoubleAnimation()
        {
            From = 0,
            To = 20,
            Duration = TimeSpan.FromSeconds(0.2)
        };

        protected readonly DoubleAnimation _opacityFadeIn = new DoubleAnimation()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.2)
        };

        protected readonly DoubleAnimation _opacityFadeOut = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2)
        };

        private readonly DependencyPropertyDescriptor _descriptor =
            DependencyPropertyDescriptor.FromProperty(IsOpenProperty, typeof(AnimatedControl));

        private readonly Storyboard _fadeInAnimation = new Storyboard();
        private readonly Storyboard _fadeOutAnimation = new Storyboard();
        private readonly ObjectAnimationUsingKeyFrames _visibilityFadeIn = new ObjectAnimationUsingKeyFrames();
        private readonly ObjectAnimationUsingKeyFrames _visibilityFadeOut = new ObjectAnimationUsingKeyFrames();

        public AnimatedControl()
        {
            Visibility = Visibility.Collapsed;
            Effect = _blurEffect;
            _descriptor.AddValueChanged(this, IsOpenPropertyChanged);
            _visibilityFadeIn.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.FromSeconds(0.0)));
            _visibilityFadeOut.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.FromSeconds(0.0)));
            _visibilityFadeOut.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Collapsed, TimeSpan.FromSeconds(0.2)));
            _fadeInAnimation.Children.Add(_opacityFadeIn);
            _fadeInAnimation.Children.Add(_blurFadeIn);
            _fadeInAnimation.Children.Add(_visibilityFadeIn);
            _fadeOutAnimation.Children.Add(_opacityFadeOut);
            _fadeOutAnimation.Children.Add(_blurFadeOut);
            _fadeOutAnimation.Children.Add(_visibilityFadeOut);
            Storyboard.SetTargetProperty(_opacityFadeIn, new PropertyPath("Opacity"));
            Storyboard.SetTargetProperty(_opacityFadeOut, new PropertyPath("Opacity"));
            Storyboard.SetTargetProperty(_blurFadeIn, new PropertyPath("Effect.Radius"));
            Storyboard.SetTargetProperty(_blurFadeOut, new PropertyPath("Effect.Radius"));
            Storyboard.SetTargetProperty(_visibilityFadeIn, new PropertyPath("Visibility"));
            Storyboard.SetTargetProperty(_visibilityFadeOut, new PropertyPath("Visibility"));
        }

        [Description("表示该动画控件是否展开（显示）。"), Category("公共")]
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private void IsOpenPropertyChanged(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                _fadeInAnimation.Begin(this);
            }
            else
            {
                _fadeOutAnimation.Begin(this);
            }
        }
    }
}