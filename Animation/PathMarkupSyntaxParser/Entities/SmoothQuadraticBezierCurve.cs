using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class SmoothQuadraticBezierCurve : Entity
    {
        public static string Command => "T";

        public Point EndPoint { get; set; }

        public SmoothQuadraticBezierCurve(Point endPoint)
        {
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var smoothQuadraticBezierCurve = (SmoothQuadraticBezierCurve)entity;
            return new SmoothQuadraticBezierCurve(
                SubstractPoint(EndPoint, smoothQuadraticBezierCurve.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var smoothQuadraticBezierCurve = (SmoothQuadraticBezierCurve)entity;
            return new SmoothQuadraticBezierCurve(
                AddPoint(EndPoint, smoothQuadraticBezierCurve.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new SmoothQuadraticBezierCurve(
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
                EndPoint
            };
        }
    }
}
