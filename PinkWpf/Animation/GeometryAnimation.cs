using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PinkWpf.Animation.PathMarkupSyntaxParser;

namespace PinkWpf.Animation
{
    public class GeometryAnimation : AnimationBase<Geometry>
    {
        public override Type TargetPropertyType => typeof(Geometry);

        private GeometryData _fromGeometryData = new GeometryData();
        private GeometryData _toGeometryData = new GeometryData();
        private GeometryData _differanceGeometryData;

        public override Geometry GetCurrentValue(Geometry from, Geometry to, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return null;

            if (IsFromChanged || IsToChanged)
            {
                if (IsFromChanged)
                {
                    _fromGeometryData.FromGeometry(from);
                    IsFromChanged = false;
                }
                if (IsToChanged)
                {
                    _toGeometryData.FromGeometry(to);
                    IsToChanged = false;
                }
                _differanceGeometryData = _toGeometryData.Substract(_fromGeometryData);
            }

            Geometry result;

            if (animationClock.CurrentProgress.Value == 0)
                result = from;
            else if (animationClock.CurrentProgress.Value == 1)
                result = to;
            else
                result = _fromGeometryData.Add(_differanceGeometryData.Multiple(GetEasedValue(animationClock.CurrentProgress.Value))).ToGeometry();

            return result;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GeometryAnimation();
        }
    }
}
