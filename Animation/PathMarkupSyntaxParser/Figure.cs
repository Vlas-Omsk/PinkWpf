using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PinkWpf.Animation.PathMarkupSyntaxParser.Entities;

namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class Figure : List<Entity>
    {
        private IEnumerator<Token> _enumerator;

        public Figure()
        {
        }

        public void FromSource(string source)
        {
            Clear();

            var lexer = new Lexer(source);
            _enumerator = lexer.GetEnumerator();

            while (_enumerator.MoveNext())
                Add(ReadCommand());
        }

        private Entity ReadCommand()
        {
            var str = ReadValue<string>(TokenType.String, false);

            if (str.Equals(Move.Command, StringComparison.OrdinalIgnoreCase))
                return new Move(ReadPoint());
            else if (str.Equals(Line.Command, StringComparison.OrdinalIgnoreCase))
                return new Line(ReadPoint());
            else if (str.Equals(HorizontalLine.Command, StringComparison.OrdinalIgnoreCase))
                return new HorizontalLine(ReadDouble());
            else if (str.Equals(VerticalLine.Command, StringComparison.OrdinalIgnoreCase))
                return new VerticalLine(ReadDouble());
            else if (str.Equals(CubicBezierCurve.Command, StringComparison.OrdinalIgnoreCase))
                return new CubicBezierCurve(ReadPoint(), ReadPoint(), ReadPoint());
            else if (str.Equals(QuadraticBezierCurve.Command, StringComparison.OrdinalIgnoreCase))
                return new QuadraticBezierCurve(ReadPoint(), ReadPoint());
            else if (str.Equals(SmoothCubicBezierCurve.Command, StringComparison.OrdinalIgnoreCase))
                return new SmoothCubicBezierCurve(ReadPoint(), ReadPoint());
            else if (str.Equals(SmoothQuadraticBezierCurve.Command, StringComparison.OrdinalIgnoreCase))
                return new SmoothQuadraticBezierCurve(ReadPoint());
            else if (str.Equals(EllipticalArc.Command, StringComparison.OrdinalIgnoreCase))
                return new EllipticalArc(ReadSize(), ReadDouble(), ReadBool(), ReadBool(), ReadPoint());
            else if (str.Equals(Close.Command, StringComparison.OrdinalIgnoreCase))
                return new Close();
            else
                throw InvalidTokenException.Create("Unknown command!", _enumerator.Current.Position, "");
        }

        private Point ReadPoint()
        {
            return new Point(
                ReadDouble(),
                ReadDouble()
            );
        }

        private Size ReadSize()
        {
            return new Size(
                ReadDouble(),
                ReadDouble()
            );
        }

        private bool ReadBool()
        {
            return ReadValue<double>(TokenType.Number) == 1 ? true : false;
        }

        private double ReadDouble()
        {
            return ReadValue<double>(TokenType.Number);
        }

        private T ReadValue<T>(TokenType type, bool moveNext = true)
        {
            if (moveNext && !_enumerator.MoveNext())
                throw new Exception("Unexpected ending");
            if (_enumerator.Current.Type != type)
                throw new Exception("Token type is not " + Enum.GetName(typeof(TokenType), type));

            return (T)_enumerator.Current.Value;
        }

        public Geometry ToGeometry()
        {
            return Geometry.Parse(ToString());
        }

        public override string ToString()
        {
            return string.Join(" ", this);
        }

        public static Figure Parse(string source)
        {
            var figure = new Figure();
            figure.FromSource(source);
            return figure;
        }
    }
}
