using System.Windows;
using System.Windows.Media.Animation;

namespace PinkWpf.Animation
{
    public abstract class AnimationBase<T> : AnimationTimeline
    {
        protected bool IsFromChanged { get; set; } = true;
        protected bool IsToChanged { get; set; } = true;

        public virtual T From
        {
            get { return (T)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); IsFromChanged = true; }
        }

        public virtual T To
        {
            get { return (T)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); IsToChanged = true; }
        }

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            defaultOriginValue = (object)From ?? defaultOriginValue;
            defaultDestinationValue = (object)To ?? defaultDestinationValue;

            return GetCurrentValue((T)defaultOriginValue, (T)defaultDestinationValue, animationClock);
        }

        public virtual T GetCurrentValue(T from, T to, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return from;

            if (animationClock.CurrentProgress.Value == 1)
                return to;
            return from;
        }

        protected double GetEasedValue(double value)
        {
            if (EasingFunction != null)
                value = EasingFunction.Ease(value);
            return value;
        }

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(AnimationBase<T>));
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(T), typeof(AnimationBase<T>));
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(T), typeof(AnimationBase<T>));
    }
}
