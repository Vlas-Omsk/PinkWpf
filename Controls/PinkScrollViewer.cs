using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PinkWpf.Controls
{
    public class PinkScrollViewer : ScrollViewer
    {
        static PinkScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PinkScrollViewer), new FrameworkPropertyMetadata(typeof(PinkScrollViewer)));
        }

        public PinkScrollViewer()
        {
        }

        #region ScrollingTimeProperty
        public TimeSpan ScrollingTime
        {
            get => (TimeSpan)GetValue(ScrollingTimeProperty);
            set => SetValue(ScrollingTimeProperty, value);
        }

        public readonly static DependencyProperty ScrollingTimeProperty =
            DependencyProperty.Register(nameof(ScrollingTime), typeof(TimeSpan), typeof(PinkScrollViewer), new PropertyMetadata(TimeSpan.FromMilliseconds(500)));
        #endregion

        #region ScrollingSplineProperty
        public KeySpline ScrollingSpline
        {
            get => (KeySpline)GetValue(ScrollingSplineProperty);
            set => SetValue(ScrollingSplineProperty, value);
        }

        public readonly static DependencyProperty ScrollingSplineProperty =
            DependencyProperty.Register(nameof(ScrollingSpline), typeof(KeySpline), typeof(PinkScrollViewer), new PropertyMetadata(new KeySpline(0.024, 0.914, 0.717, 1)));
        #endregion
    }
}
