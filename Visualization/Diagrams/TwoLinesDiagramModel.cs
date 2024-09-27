using Avalonia;
using Avalonia.Media;
using DataStructures;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Visualization.Util;
using static System.Reflection.Metadata.BlobBuilder;

namespace Visualization.Diagrams
{
    internal class TwoLinesDiagramModel : IDiagramBase
    {
        public bool withTheme;

        private double lineLength;
        private Point startTopLine;

        private LinkColorType linkColorType;
        private IBrush forwardLinkBrush;
        private IBrush backLinkBrush;

        public TwoLinesDiagramModel(bool withTheme)
        {
            this.withTheme = withTheme;
            backLinkBrush = ConfigManager.GetConfigColor("BackReferenceColor");
            forwardLinkBrush = ConfigManager.GetConfigColor("ForwardReferenceColor");
            linkColorType = ConfigManager.GetLinkColorType();
        }
        public PathGeometry GetBookGeometry(Book book)
        {
            double start = book.GetStartPosition();
            double end = book.GetEndPosition();

            Point startPoint = GetPointOnLine(start);
            Point endPoint = GetPointOnLine(end);

            return GetStraightLine(endPoint, startPoint);
        }

        public List<VisualLink> GetConnectedLinks(List<Link> links, ObservableCollection<Book> books)
        {
            ConcurrentBag<VisualLink> visualLinks = new ConcurrentBag<VisualLink>();
            List<Book> bookList = books.ToList();

            if (withTheme)
            {
               foreach(Link link in links)
                {
                    Reference source = link.source;
                    var sourceBook = bookList.FirstOrDefault(book => book.Number == source.book);
                    if (sourceBook != null)
                    {
                        IBrush brush = sourceBook.ColorBrush;
                        try
                        {
                            double sourcepos = sourceBook.GetVersePosition(source.chapterStart, source.verseStart);
                            double targetpos = 0.5; // middle of top line

                            visualLinks.Add(new VisualLink(sourcepos, targetpos, link, brush));

                        }
                        catch {
                            Console.WriteLine();
                        } //source not found, skip
                    }
                }
                    
            }
            else
            {
                Parallel.ForEach(links, link =>
                {
                    Reference target = (Reference)link.target;
                    Reference source = link.source;
                    var targetBook = bookList.FirstOrDefault(book => book.Number == target.book && book.GetStartPosition() > 0);
                    var sourceBook = bookList.FirstOrDefault(book => book.Number == source.book && book.GetStartPosition() <= 0);
                    if(sourceBook == null || targetBook == null)
                    {
                        targetBook = books.FirstOrDefault(book => book.Number == target.book && book.GetStartPosition() <= 0);
                        sourceBook = books.FirstOrDefault(book => book.Number == source.book && book.GetStartPosition() > 0);
                    }
                    if (sourceBook != null && targetBook != null)
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
                        }
                        catch { }// reference does not exist or is not displayed - so skip
                    }
                });
            }

                
            return  visualLinks.ToList();
        }


        public ObservableCollection<Book> GetInitializedBooks()
        {
            if (withTheme)
            {
                return GetBooksOnLine("BooksInDiagram", false);
            }
            else
            {
                ObservableCollection<Book> firstGroup = GetBooksOnLine("FirstGroupBooks", true);
                ObservableCollection<Book> secondGroup = GetBooksOnLine("SecondGroupBooks", false);
                foreach (var book in secondGroup)
                {
                    firstGroup.Add(book);
                }
                return firstGroup;
            }
        }

        public PathGeometry GetLinkGeometry(VisualLink link)
        {
            Point startPoint = GetPointOnLine(link.startPosition);
            Point endPoint = GetPointOnLine(link.endPosition);

            return GetStraightLine(startPoint, endPoint);
        }

        public void UpdateDiagramPosition(Rect Bounds)
        {
            lineLength = 0.8 * Bounds.Width;
            double startX = 0.1 * Bounds.Width;
            startTopLine = new Point(startX, Bounds.Height / 4);
        }

        private Point GetPointOnLine(double proportionalPosition)
        {
            double pointX = startTopLine.X + Math.Abs(proportionalPosition) * lineLength;
            double pointY = (proportionalPosition > 0) ? startTopLine.Y : startTopLine.Y * 3;
            return new Point(pointX, pointY);
        }

        private ObservableCollection<Book> GetBooksOnLine(string name, bool topLine)
        {
            List<int> includedBooks = ConfigManager.GetBookList(name);
            BookManager bookManager = new BookManager();
            bookManager.LoadBooks(includedBooks);
            return bookManager.GetBooksOnLine(topLine);
        }

        private PathGeometry GetStraightLine(Point start, Point end)
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
