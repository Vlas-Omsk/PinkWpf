using System;
using System.Collections.Generic;
using System.Windows.Media;
using PinkWpf.Animation.PathMarkupSyntaxParser.Entities;

namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class GeometryData
    {
        public List<Figure> Figures { get; } = new();

        public GeometryData Substract(GeometryData geometryData)
        {
            return Transform(geometryData, (e1, e2) => e1.Substract(e2));
        }

        public GeometryData Add(GeometryData geometryData)
        {
            return Transform(geometryData, (e1, e2) => e1.Add(e2));
        }

        public GeometryData Multiple(double value)
        {
            var result = new GeometryData();

            for (var i = 0; i < Figures.Count; i++)
            {
                var figure = new Figure();
                for (var j = 0; j < Figures[i].Count; j++)
                    figure.Add(Figures[i][j].Multiple(value));
                result.Figures.Add(figure);
            }

            return result;
        }

        private GeometryData Transform(GeometryData geometryData, Func<Entity, Entity, Entity> callback)
        {
            if (Figures.Count != geometryData.Figures.Count)
                throw new Exception("Figures count are not equals");

            var result = new GeometryData();

            for (var i = 0; i < Figures.Count; i++)
            {
                if (Figures[i].Count != geometryData.Figures[i].Count)
                    throw new Exception("Entities count are not equals");

                var figure = new Figure();
                for (var j = 0; j < Figures[i].Count; j++)
                    figure.Add(callback(Figures[i][j], geometryData.Figures[i][j]));
                result.Figures.Add(figure);
            }

            return result;
        }

        public void FromGeometry(Geometry geometry)
        {
            Figures.Clear();

            var figures = PathGeometry.CreateFromGeometry(geometry).Figures;
            foreach (var figure in figures)
                Figures.Add(ParseSegments(figure));
        }

        private Figure ParseSegments(PathFigure pathFigure)
        {
            var figure = new Figure();

            figure.Add(new Move(pathFigure.StartPoint));

            foreach (var segment in pathFigure.Segments)
            {
                var type = segment.GetType();
                if (type == typeof(LineSegment))
                    figure.Add(new Line(((LineSegment)segment).Point));
                else if (type == typeof(PolyLineSegment))
                {
                    var points = ((PolyLineSegment)segment).Points;
                    for (var i = 0; i < points.Count; i++)
                        figure.Add(new Line(points[i]));
                }
                else if (type == typeof(BezierSegment))
                {
                    var bezierSegment = (BezierSegment)segment;
                    figure.Add(new CubicBezierCurve(bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3));
                }
                else if (type == typeof(PolyBezierSegment))
                {
                    var points = ((PolyBezierSegment)segment).Points;
                    for (var i = 0; i < points.Count; i += 3)
                        figure.Add(new CubicBezierCurve(points[i], points[i + 1], points[i + 2]));
                }
                else if (type == typeof(QuadraticBezierSegment))
                {
                    var quadraticBezierSegment = (QuadraticBezierSegment)segment;
                    figure.Add(new QuadraticBezierCurve(quadraticBezierSegment.Point1, quadraticBezierSegment.Point2));
                }
                else if (type == typeof(PolyQuadraticBezierSegment))
                {
                    var points = ((PolyQuadraticBezierSegment)segment).Points;
                    for (var i = 0; i < points.Count; i += 2)
                        figure.Add(new QuadraticBezierCurve(points[i], points[i + 1]));
                }
                else if (type == typeof(ArcSegment))
                {
                    var arcSegment = (ArcSegment)segment;
                    figure.Add(new EllipticalArc(arcSegment.Size, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection == SweepDirection.Clockwise, arcSegment.Point));
                }
            }

            if (pathFigure.IsClosed)
                figure.Add(new Close());

            return figure;
        }

        public Geometry ToGeometry()
        {
            if (Figures.Count == 0)
                return null;
            else if (Figures.Count == 1)
                return Figures[0].ToGeometry();
            else
            {
                var geometry = new GeometryGroup();
                foreach (var figure in Figures)
                    geometry.Children.Add(figure.ToGeometry());
                return geometry;
            }
        }
    }
}