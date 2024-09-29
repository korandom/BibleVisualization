using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualization.Util
{
    public static class GeometryHelper
    {
        public static PathGeometry GetStraightLine(Point start, Point end)
        {
            var figure = new PathFigure
            {
                StartPoint = start,
                IsClosed = false,
                Segments = new PathSegments
            {
                new LineSegment
                {
                    Point = end,
                }
            }
            };
            var pathGeometry = new PathGeometry
            {
                Figures = new PathFigures { figure }
            };

            return pathGeometry;
        }
    }
}
