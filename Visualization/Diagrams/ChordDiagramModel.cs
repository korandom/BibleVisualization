using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DataStructures;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Visualization.Util;
using static System.Reflection.Metadata.BlobBuilder;

namespace Visualization.Diagrams
{
    internal class ChordDiagramModel : IDiagramBase
    {
        public Point center;
        private double radius;

        private double curveParam;
        private LinkColorType linkColorType;
        private IBrush forwardLinkBrush;
        private IBrush backLinkBrush;

        private double histogramMaxLength = 200;
        private double marginAroundBooks = 5;

        public ChordDiagramModel()
        {
            curveParam = ConfigManager.GetConfigProperty("CurvingParameter");
            backLinkBrush = ConfigManager.GetConfigColor("BackReferenceColor");
            forwardLinkBrush = ConfigManager.GetConfigColor("ForwardReferenceColor");
            linkColorType = ConfigManager.GetEnumValue<LinkColorType>("LinkColor");
        }
        public PathGeometry GetBookGeometry(Book book)
        {
            double start = book.StartPosition;
            double end = book.EndPosition;

            Point startPoint = GetPointOnCircle(start, radius);
            Point endPoint = GetPointOnCircle(end, radius);
            ArcSegment arcSegment = new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = end - start > Math.PI
            };

            PathFigure figure = new PathFigure
            {
                StartPoint = startPoint,
                Segments = new PathSegments { arcSegment },
                IsClosed = false,
                IsFilled = false
            };
            var pathGeometry = new PathGeometry
            {
                Figures = new PathFigures { figure }
            };

            return pathGeometry;
        }

        public List<VisualLink> GetConnectedLinks(List<Link> links, ObservableCollection<Book> books, bool calculateHistogramData)
        {
            ConcurrentBag<VisualLink> visualLinks = new ConcurrentBag<VisualLink>();

            Dictionary<int, Book> bookDic = books.ToDictionary(book => book.Number);

            Parallel.ForEach(links, link =>
            {
                if (link.target == null) return;
                Reference source = link.source;
                Reference target = (Reference)link.target;

                bool result = bookDic.TryGetValue(source.book, out Book sourceBook);
                bool result2 = bookDic.TryGetValue(target.book, out Book targetBook);

                if (result && result2)
                {
                    try
                    {
                        double sourceAngle = sourceBook.GetVersePosition(source.chapterStart, source.verseStart);
                        double targetAngle = targetBook.GetVersePosition(target.chapterStart, target.verseStart);
                        IBrush brush;
                        switch (linkColorType)
                        {
                            case LinkColorType.byTargetBook:
                                {
                                    brush = targetBook.ColorBrush;
                                    break;
                                }
                            case LinkColorType.bySourceBook:
                                {
                                    brush = sourceBook.ColorBrush;
                                    break;
                                }
                            case LinkColorType.byOrder:
                                {
                                    brush = (source.CompareTo(target) > 0) ? backLinkBrush : forwardLinkBrush;
                                    break;
                                }
                            default:
                                {
                                    brush = sourceBook.ColorBrush;
                                    break;
                                }
                        }

                        visualLinks.Add(new VisualLink(sourceAngle, targetAngle, link, brush));
                        if(calculateHistogramData)
                        {
                            sourceBook.MarkOccurance(source, link.Occurance);
                            targetBook.MarkOccurance(target, link.Occurance);
                        }
                    }
                    catch{  } // link not found, skip
                }
            });
            return visualLinks.ToList();
        }

        public ObservableCollection<Book> GetInitializedBooks()
        {
            List<int> booksIncluded = ConfigManager.GetBookList("BooksInDiagram");
            BookManager bookManager = new BookManager();
            bookManager.LoadBooks(booksIncluded);
            return bookManager.GetBooksWithAngles();
        }

        public PathGeometry GetLinkGeometry(VisualLink link)
        {
            Point endPoint = GetPointOnCircle(link.endPosition, radius - marginAroundBooks);
            Point startPoint = GetPointOnCircle(link.startPosition, radius - marginAroundBooks);

            Point controlPoint = GetControlPoint(curveParam, endPoint, startPoint);

            QuadraticBezierSegment curveSegment = new QuadraticBezierSegment
            {
                Point1 = controlPoint,
                Point2 = endPoint
            };

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = startPoint,
                Segments = new PathSegments { curveSegment },
                IsClosed = false,
                IsFilled = false
            };
            pathGeometry.Figures?.Add(pathFigure);

            return pathGeometry;
        }

        public void UpdateDiagramPosition(Rect Bounds)
        {
            center = Bounds.Center;
            radius = Math.Min(Bounds.Width, Bounds.Height) / 2 - 100;
        }

        private Point GetPointOnCircle(double angle, double r)
        {
            double x = center.X + r * Math.Cos(angle);
            double y = center.Y + r * Math.Sin(angle);
            return new Point(x, y);
        }

        private Point GetControlPoint(double curveParam, Point endPoint, Point startPoint)
        {
            Point midPoint = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);

            double deltaX = endPoint.X - startPoint.X;
            double deltaY = endPoint.Y - startPoint.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            var vectorToCenter = new Point(center.X - midPoint.X, center.Y - midPoint.Y);

            double scaleFactor = distance / (2 * radius);

            return new Point(
                midPoint.X + vectorToCenter.X * curveParam * scaleFactor,
                midPoint.Y + vectorToCenter.Y * curveParam * scaleFactor
            );
        }

        public PathGeometry GetHistogramGeometry(HistogramItem histogramItem)
        {
            if (histogramItem.Occurance == 0)
            {
                return new PathGeometry();
            }
            double start = histogramItem.StartPosition;
            double end = histogramItem.EndPosition;
            double histogramLength = histogramItem.LengthProportional * histogramMaxLength;

            Point A = GetPointOnCircle(start, radius + marginAroundBooks);

            Point C = GetPointOnCircle(start, radius + marginAroundBooks + histogramLength);

            if (start == end)
            {
                return GeometryHelper.GetStraightLine(A, C);
            }

            Point B = GetPointOnCircle(end, radius + marginAroundBooks);

            Point D = GetPointOnCircle (end, radius + marginAroundBooks + histogramLength);

            var figure = new PathFigure
            {
                StartPoint = A,
                IsClosed = true,
                Segments = new PathSegments
                {
                    new ArcSegment
                    {
                        Point = B,
                        Size = new Size(radius + marginAroundBooks, radius + marginAroundBooks),
                        SweepDirection = SweepDirection.Clockwise,
                        IsLargeArc = end - start > Math.PI
                    },
                    new LineSegment
                    {
                        Point = D,
                    },
                    new ArcSegment
                    {
                        Point = C,
                        Size = new Size(radius + marginAroundBooks + histogramLength, radius + marginAroundBooks + histogramLength),
                        SweepDirection = SweepDirection.Clockwise,
                        IsLargeArc = end - start > Math.PI
                    },
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
