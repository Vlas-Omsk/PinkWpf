using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class Line : Entity
    {
        public static string Command => "L";

        public Point EndPoint { get; set; }

        public Line(Point endPoint)
        {
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var line = (Line)entity;
            return new Line(
                SubstractPoint(EndPoint, line.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var line = (Line)entity;
            return new Line(
                AddPoint(EndPoint, line.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new Line(
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
