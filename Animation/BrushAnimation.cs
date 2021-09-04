using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace PinkWpf.Animation
{
    public class BrushAnimation : AnimationBase<Brush>
    {
        public override Type TargetPropertyType => typeof(Brush);

        public override Brush GetCurrentValue(Brush from, Brush to, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return Brushes.Transparent;

            if (animationClock.CurrentProgress.Value == 0)
                return from;
            if (animationClock.CurrentProgress.Value == 1)
                return to;

            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = from,
                Child = new Border()
                {
                    Background = to,
                    Opacity = GetEasedValue(animationClock.CurrentProgress.Value),
                }
            });
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }
    }
}
