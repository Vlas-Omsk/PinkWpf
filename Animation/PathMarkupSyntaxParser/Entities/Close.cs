using System;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class Close : Entity
    {
        public static string Command => "Z";

        public override Entity Substract(Entity entity)
        {
            return new Close();
        }

        public override Entity Add(Entity entity)
        {
            return new Close();
        }

        public override Entity Multiple(double value)
        {
            return new Close();
        }

        protected override string GetCommand()
        {
            return Command;
        }

        protected override object[] GetValues()
        {
            return Array.Empty<object>();
        }
    }
}
