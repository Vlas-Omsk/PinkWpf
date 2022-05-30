using System.Linq;
using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public abstract class Entity
    {
        public abstract Entity Substract(Entity entity);
        public abstract Entity Add(Entity entity);
        public abstract Entity Multiple(double value);
        protected abstract string GetCommand();
        protected abstract object[] GetValues();

        protected Point SubstractPoint(Point p1, Point p2)
        {
            return new Point(
                p1.X - p2.X,
                p1.Y - p2.Y
            );
        }

        protected Size SubstractSize(Size p1, Size p2)
        {
            return new Size(
                p1.Width - p2.Width,
                p1.Height - p2.Height
            );
        }

        protected Point AddPoint(Point p1, Point p2)
        {
            return new Point(
                p1.X + p2.X,
                p1.Y + p2.Y
            );
        }

        protected Size AddSize(Size p1, Size p2)
        {
            return new Size(
                p1.Width + p2.Width,
                p1.Height + p2.Height
            );
        }

        protected Point MultiplePoint(Point point, double value)
        {
            return new Point(
                point.X * value,
                point.Y * value
            );
        }

        protected Size MultipleSize(Size size, double value)
        {
            return new Size(
                size.Width * value,
                size.Height * value
            );
        }

        private string DoubleToString(double value)
        {
            return value.ToString().Replace(',', '.');
        }

        public override string ToString()
        {
            return GetCommand() + string.Join(" ",
                GetValues()
                .Select(value =>
                {
                    string result;
                    if (value is Point)
                    {
                        var point = (Point)value;
                        result = DoubleToString(point.X) + "," + DoubleToString(point.Y);
                    }
                    else if (value is Size)
                    {
                        var size = (Size)value;
                        result = DoubleToString(size.Width) + "," + DoubleToString(size.Height);
                    }
                    else if (value is double)
                        result = DoubleToString((double)value);
                    else if (value is bool)
                        result = (bool)value ? "1" : "0";
                    else
                        result = value.ToString();
                    return result;
                })
            );
        }
    }
}
