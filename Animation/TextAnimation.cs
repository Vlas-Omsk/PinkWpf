using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace PinkWpf.Animation
{
    public class TextAnimation : AnimationBase<string>
    {
        public override Type TargetPropertyType => typeof(string);

        protected bool IsTextAnimationTypeChanged { get; set; } = true;

        public TextAnimationType TextAnimationType
        {
            get { return (TextAnimationType)GetValue(TextAnimationTypeProperty); }
            set { SetValue(TextAnimationTypeProperty, value); IsTextAnimationTypeChanged = true; }
        }

        private double _centerPercent;
        private double _fromMultiplier;
        private double _toMultiplier;
        private int _equalsLength;

        public override string GetCurrentValue(string from, string to, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return null;

            if (animationClock.CurrentProgress.Value == 0)
                return from;
            if (animationClock.CurrentProgress.Value == 1)
                return to;

            var originalFrom = from;

            if (TextAnimationType == TextAnimationType.EraseDifference)
            {
                if (IsFromChanged || IsToChanged || IsTextAnimationTypeChanged)
                    for (_equalsLength = 0; _equalsLength < Math.Min(from.Length, to.Length); _equalsLength++)
                        if (from[_equalsLength] != to[_equalsLength])
                            break;
                from = from.Substring(_equalsLength);
                to = to.Substring(_equalsLength);
            }

            if (IsFromChanged || IsToChanged || IsTextAnimationTypeChanged)
            {
                IsFromChanged = false;
                IsToChanged = false;
                IsTextAnimationTypeChanged = false;

                _centerPercent = from.Length / (double)(from.Length + to.Length);
                _fromMultiplier = 1 / _centerPercent;
                _toMultiplier = 1 / (1 - _centerPercent);
            }

            var progress = GetEasedValue(animationClock.CurrentProgress.Value);

            if (TextAnimationType == TextAnimationType.Erase || TextAnimationType == TextAnimationType.EraseDifference)
            {
                string value;
                if (progress < _centerPercent)
                {
                    var visibleLength = (int)(from.Length * ((_centerPercent - progress) * _fromMultiplier));
                    if (visibleLength >= from.Length)
                        value = from;
                    else if (visibleLength <= 0)
                        value = string.Empty;
                    else
                        value = from.Substring(0, visibleLength);
                }
                else
                {
                    var visibleLength = (int)(to.Length * ((progress - _centerPercent) * _toMultiplier));
                    if (visibleLength >= to.Length)
                        value = to;
                    else if (visibleLength <= 0)
                        value = string.Empty;
                    else
                        value = to.Substring(0, visibleLength);
                }
                if (TextAnimationType == TextAnimationType.EraseDifference)
                    value = originalFrom.Substring(0, _equalsLength) + value;
                return value;
            }
            else if (TextAnimationType == TextAnimationType.Scan || TextAnimationType == TextAnimationType.Change)
            {
                string value = string.Empty;
                var maxDifference = 0;
                for (var i = 0; i < Math.Max(from.Length, to.Length); i++)
                {
                    var fromChar = i >= from.Length ? ' ' : from[i];
                    var toChar = i >= to.Length ? ' ' : to[i];
                    var difference = toChar - fromChar;
                    if (TextAnimationType == TextAnimationType.Scan)
                    {
                        if (difference > maxDifference)
                            maxDifference = difference;
                    }
                    else
                        value += (char)(fromChar + (difference * progress));
                }
                if (TextAnimationType == TextAnimationType.Scan)
                    for (var i = 0; i < Math.Max(from.Length, to.Length); i++)
                    {
                        var fromChar = i >= from.Length ? ' ' : from[i];
                        var toChar = i >= to.Length ? ' ' : to[i];
                        var current = fromChar + ((fromChar > toChar ? -maxDifference : maxDifference) * progress);
                        if (fromChar < toChar)
                        {
                            if (current > toChar)
                                current = toChar;
                        }
                        else if (current < toChar)
                            current = toChar;
                        value += (char)current;
                    }
                return value;
            }
            throw new NotImplementedException();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new TextAnimation();
        }

        public static readonly DependencyProperty TextAnimationTypeProperty =
            DependencyProperty.Register("TextAnimationType", typeof(TextAnimationType), typeof(TextAnimation));
    }
}
