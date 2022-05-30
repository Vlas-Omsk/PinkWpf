namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class HorizontalLine : Entity
    {
        public static string Command => "H";

        public double X { get; set; }

        public HorizontalLine(double x)
        {
            X = x;
        }

        public override Entity Substract(Entity entity)
        {
            var horizontalLine = (HorizontalLine)entity;
            return new HorizontalLine(
                X - horizontalLine.X
            );
        }

        public override Entity Add(Entity entity)
        {
            var horizontalLine = (HorizontalLine)entity;
            return new HorizontalLine(
                X + horizontalLine.X
            );
        }

        public override Entity Multiple(double value)
        {
            return new HorizontalLine(
                X * value
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
                X
            };
        }
    }
}
