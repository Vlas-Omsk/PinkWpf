using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class QuadraticBezierCurve : Entity
    {
        public static string Command => "Q";

        public Point ControlPoint { get; set; }
        public Point EndPoint { get; set; }

        public QuadraticBezierCurve(Point controlPoint, Point endPoint)
        {
            ControlPoint = controlPoint;
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var quadraticBezierCurve = (QuadraticBezierCurve)entity;
            return new QuadraticBezierCurve(
                SubstractPoint(ControlPoint, quadraticBezierCurve.ControlPoint),
                SubstractPoint(EndPoint, quadraticBezierCurve.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var quadraticBezierCurve = (QuadraticBezierCurve)entity;
            return new QuadraticBezierCurve(
                AddPoint(ControlPoint, quadraticBezierCurve.ControlPoint),
                AddPoint(EndPoint, quadraticBezierCurve.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new QuadraticBezierCurve(
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
