using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class SmoothCubicBezierCurve : Entity
    {
        public static string Command => "S";

        public Point ControlPoint { get; set; }
        public Point EndPoint { get; set; }

        public SmoothCubicBezierCurve(Point controlPoint, Point endPoint)
        {
            ControlPoint = controlPoint;
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var smoothCubicBezierCurve = (SmoothCubicBezierCurve)entity;
            return new SmoothCubicBezierCurve(
                SubstractPoint(ControlPoint, smoothCubicBezierCurve.ControlPoint),
                SubstractPoint(EndPoint, smoothCubicBezierCurve.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var smoothCubicBezierCurve = (SmoothCubicBezierCurve)entity;
            return new SmoothCubicBezierCurve(
                AddPoint(ControlPoint, smoothCubicBezierCurve.ControlPoint),
                AddPoint(EndPoint, smoothCubicBezierCurve.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new SmoothCubicBezierCurve(
                MultiplePoint(ControlPoint, value),
                MultiplePoint(EndPoint, value)
            );
        }

        protected override string GetCommand()
        {
            return Command;
        }

        protected override object[] GetValues()
        {
            return new object[]
            {
                ControlPoint,
                EndPoint
            };
        }
    }
}
