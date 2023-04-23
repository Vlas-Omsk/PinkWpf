using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PinkWpf.Controls
{
    public class AnimatedScrollViewer : ScrollViewer
    {
        static AnimatedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedScrollViewer), new FrameworkPropertyMetadata(typeof(AnimatedScrollViewer)));
        }

        public AnimatedScrollViewer()
        {
        }

        #region ScrollingTimeProperty

        public TimeSpan ScrollingTime
        {
            get => (TimeSpan)GetValue(ScrollingTimeProperty);
            set => SetValue(ScrollingTimeProperty, value);
        }

        public readonly static DependencyProperty ScrollingTimeProperty = DependencyProperty.Register(
            nameof(ScrollingTime),
            typeof(TimeSpan),
            typeof(AnimatedScrollViewer),
            new PropertyMetadata(TimeSpan.FromMilliseconds(500))
        );

        #endregion

        #region ScrollingSplineProperty

        public KeySpline ScrollingSpline
        {
            get => (KeySpline)GetValue(ScrollingSplineProperty);
            set => SetValue(ScrollingSplineProperty, value);
        }

        public readonly static DependencyProperty ScrollingSplineProperty = DependencyProperty.Register(
            nameof(ScrollingSpline),
            typeof(KeySpline),
            typeof(AnimatedScrollViewer),
            new PropertyMetadata(new KeySpline(0.024, 0.914, 0.717, 1))
        );

        #endregion
    }
}
