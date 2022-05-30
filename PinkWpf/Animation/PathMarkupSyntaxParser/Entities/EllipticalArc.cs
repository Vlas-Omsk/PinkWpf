using System.Windows;

namespace PinkWpf.Animation.PathMarkupSyntaxParser.Entities
{
    public class EllipticalArc : Entity
    {
        public static string Command => "A";

        public Size Size { get; set; }
        public double RotationAngle { get; set; }
        public bool IsLargeArcFlag { get; set; }
        public bool SweepDirectionFlag { get; set; }
        public Point EndPoint { get; set; }

        public EllipticalArc(Size size, double rotationAngle, bool isLargeArcFlag, bool sweepDirectionFlag, Point endPoint)
        {
            Size = size;
            RotationAngle = rotationAngle;
            IsLargeArcFlag = isLargeArcFlag;
            SweepDirectionFlag = sweepDirectionFlag;
            EndPoint = endPoint;
        }

        public override Entity Substract(Entity entity)
        {
            var ellipticalArc = (EllipticalArc)entity;
            return new EllipticalArc(
                SubstractSize(Size, ellipticalArc.Size),
                RotationAngle - ellipticalArc.RotationAngle,
                ellipticalArc.IsLargeArcFlag,
                ellipticalArc.SweepDirectionFlag,
                SubstractPoint(EndPoint, ellipticalArc.EndPoint)
            );
        }

        public override Entity Add(Entity entity)
        {
            var ellipticalArc = (EllipticalArc)entity;
            return new EllipticalArc(
                AddSize(Size, ellipticalArc.Size),
                RotationAngle + ellipticalArc.RotationAngle,
                ellipticalArc.IsLargeArcFlag,
                ellipticalArc.SweepDirectionFlag,
                AddPoint(EndPoint, ellipticalArc.EndPoint)
            );
        }

        public override Entity Multiple(double value)
        {
            return new EllipticalArc(
                MultipleSize(Size, value),
                RotationAngle * value,
                IsLargeArcFlag,
                SweepDirectionFlag,
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
                Size,
                RotationAngle,
                IsLargeArcFlag,
                SweepDirectionFlag,
                EndPoint
            };
        }
    }
}
