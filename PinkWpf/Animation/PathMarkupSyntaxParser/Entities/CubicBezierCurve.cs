using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class CubicBezierCurve : Entity
    {
        public static string Command => "C";

        public Point ControlPoint1 { get; set; }
        public Point ControlPoint2 { get; set; }
        public Point EndPoint { get; set; }

        public CubicBezierCurve(Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var cubicBezierCurve = (CubicBezierCurve)entity;
            return new CubicBezierCurve(
                SubstractPoint(ControlPoint1, cubicBezierCurve.ControlPoint1),
                SubstractPoint(ControlPoint2, cubicBezierCurve.ControlPoint2),
                SubstractPoint(EndPoint, cubicBezierCurve.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var cubicBezierCurve = (CubicBezierCurve)entity;
            return new CubicBezierCurve(
                AddPoint(ControlPoint1, cubicBezierCurve.ControlPoint1),
                AddPoint(ControlPoint2, cubicBezierCurve.ControlPoint2),
                AddPoint(EndPoint, cubicBezierCurve.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new CubicBezierCurve(
                MultiplePoint(ControlPoint1, value),
                MultiplePoint(ControlPoint2, value),
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
                ControlPoint1,
                ControlPoint2,
                EndPoint
            };
        }
    }
}
