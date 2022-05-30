using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class Move : Entity
    {
        public static string Command => "M";

        public Point StartPoint { get; set; }

        public Move(Point startPoint)
        {
            StartPoint = startPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var move = (Move)entity;
            return new Move(
                SubstractPoint(StartPoint, move.StartPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var move = (Move)entity;
            return new Move(
                AddPoint(StartPoint, move.StartPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new Move(
                MultiplePoint(StartPoint, value)
            );
        }

        protected override string GetCommand()
        {
            return Command;
        }

        protected override object[] GetValues()
        {
            return new object[] {
                StartPoint
            };
        }
    }
}
