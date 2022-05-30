namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class VerticalLine : Entity
    {
        public static string Command => "V";

        public double Y { get; set; }

        public VerticalLine(double y)
        {
            Y = y;
        }

        public override Entity Substract(Entity entity)
        {
            var verticalLine = (VerticalLine)entity;
            return new VerticalLine(
                Y - verticalLine.Y
            );
        }

        public override Entity Add(Entity entity)
        {
            var verticalLine = (VerticalLine)entity;
            return new VerticalLine(
                Y + verticalLine.Y
            );
        }

        public override Entity Multiple(double value)
        {
            return new VerticalLine(
                Y * value
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
                Y
            };
        }
    }
}
