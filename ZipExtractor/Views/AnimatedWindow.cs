using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ZipExtractor.Views
{
    public class AnimatedWindow : Window
    {
        protected readonly BlurEffect _blurEffect = new BlurEffect() { Radius = 15 };

        protected readonly DoubleAnimation _blurFadeIn = new DoubleAnimation()
        {
            From = 15,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.15)
        };

        protected readonly DoubleAnimation _blurFadeOut = new DoubleAnimation()
        {
            From = 0,
            To = 15,
            Duration = TimeSpan.FromSeconds(0.15)
        };

        protected readonly DoubleAnimation _opacityFadeIn = new DoubleAnimation()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.15)
        };

        protected readonly DoubleAnimation _opacityFadeOut = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.15)
        };

        private readonly Storyboard _fadeInAnimation = new Storyboard();
        private readonly Storyboard _fadeOutAnimation = new Storyboard();
        private bool _closing = false;
        private bool? _result = null;

        public AnimatedWindow()
        {
            Effect = _blurEffect;
            Storyboard.SetTargetProperty(_opacityFadeIn, new PropertyPath("Opacity"));
            Storyboard.SetTargetProperty(_opacityFadeOut, new PropertyPath("Opacity"));
            Storyboard.SetTargetProperty(_blurFadeIn, new PropertyPath("Effect.Radius"));
            Storyboard.SetTargetProperty(_blurFadeOut, new PropertyPath("Effect.Radius"));
            _fadeInAnimation.Children.Add(_opacityFadeIn);
            _fadeInAnimation.Children.Add(_blurFadeIn);
            _fadeOutAnimation.Children.Add(_opacityFadeOut);
            _fadeOutAnimation.Children.Add(_blurFadeOut);
            _fadeOutAnimation.Completed += (a, b) =>
            {
                if (_result.HasValue)
                    DialogResult = _result;
                else
                    Close();
            };
            Loaded += Window_Loaded;
            Closing += Window_Closing;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_closing)
            {
                return;
            }
            _result = DialogResult;
            _closing = true;
            e.Cancel = true;
            _fadeOutAnimation.Begin(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _fadeInAnimation.Begin(this);
        }
    }
}