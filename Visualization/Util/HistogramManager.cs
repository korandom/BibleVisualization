using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Visualization.Util
{
    public enum HistogramType
    {
        none,
        book,
        chapter,
        verse
    }
    public partial class HistogramItem : ObservableObject
    {
        public double StartPosition { get; }
        public double EndPosition { get; }
        public double LengthProportional { get; set; }

        private readonly string information = "";

        public string Information => information;

        private readonly int occurance;

        public int Occurance => occurance;

        private readonly IBrush brush;
        public IBrush Brush => brush;

        [ObservableProperty]
        public PathGeometry geometry;

        public HistogramItem(Book book)
        {
            this.StartPosition = book.StartPosition;
            this.EndPosition = book.EndPosition;
            this.occurance = book.Occurance;
            this.information = $"{book.Name}\nOccurance: {occurance}";
            this.brush = book.ColorBrush;
        }

        public HistogramItem(Book book, Chapter chapter) 
        {
            this.StartPosition = chapter.StartPosition;
            this.EndPosition = chapter.EndPosition;
            this.occurance = chapter.Occurance + book.DistributableOccurance;
            this.information = $"{book.Name} {chapter.Number}\nOccurance: {occurance}";
            this.brush = book.ColorBrush;
        }

        public HistogramItem(double startPosition, string information, int occurance, IBrush brush)
        {
            this.StartPosition = startPosition;
            this.EndPosition = startPosition;
            this.occurance = occurance;
            this.information = $"{information}\nOccurance: {occurance}";
            this.brush = brush;
        }
    }
    public class HistogramManager
    {
        HistogramType type;
        public HistogramManager() 
        {
            type = ConfigManager.GetEnumValue<HistogramType>("Histogram");
        }
        public bool IsHistogramActive => type != HistogramType.none;

        public bool isHistogramVerses => type == HistogramType.verse;

        public List<HistogramItem> GetHistogram(IEnumerable<Book> books)
        {
            switch (type)
            {
                case HistogramType.none:
                    return new List<HistogramItem>();
                case HistogramType.book:
                    return GetBookHistogram(books);
                case HistogramType.chapter:
                    return GetChapterHistogram(books);
                case HistogramType.verse:
                    return GetVerseHistogram(books);
                default:
                    return new List<HistogramItem>();
            }
        }

        private List<HistogramItem> GetBookHistogram(IEnumerable<Book> books)
        {
            List<HistogramItem > histogramItems = new List<HistogramItem>();
            foreach (Book book in books)
            {
                if(book.Occurance > 0)
                {
                    histogramItems.Add(new HistogramItem(book));
                }
            }
            CalculateProportionalLengths(histogramItems);
            return histogramItems;
        }
        private List<HistogramItem> GetChapterHistogram(IEnumerable<Book> books)
        {
            List<HistogramItem> histogramItems = new List<HistogramItem>();
            foreach(Book book in books)
            {
                foreach( var chapter in book.Chapters.Values)
                {
                    if(chapter.Occurance + book.DistributableOccurance > 0)
                    {
                        histogramItems.Add(new HistogramItem(book, chapter));
                    }
                }
            }
            CalculateProportionalLengths(histogramItems);
            return histogramItems;
        }

        private List<HistogramItem> GetVerseHistogram(IEnumerable<Book> books)
        {
            List<HistogramItem> histogramItems = new List<HistogramItem>();
            foreach (Book book in books)
            {
                foreach (var chapter in book.Chapters.Values)
                {
                    foreach(var kvpVerse in chapter.Verses)
                    {
                        Verse verse = kvpVerse.Value;
                        int occurance = verse.Occurance + chapter.DistributableOccurance + book.DistributableOccurance;
                        if(occurance > 0)
                        {
                            histogramItems.Add(new HistogramItem(verse.Position, $"{chapter.Number}:{kvpVerse.Key}", occurance, book.ColorBrush));
                        }
                    }
                }
            }
            CalculateProportionalLengths(histogramItems);
            return histogramItems;
        }

        private void CalculateProportionalLengths(List<HistogramItem> histogramItems)
        {
            int maxOccurance = histogramItems.Max(item => item.Occurance);
            foreach( HistogramItem histogramItem in histogramItems)
            {
                histogramItem.LengthProportional = Math.Sqrt((double)histogramItem.Occurance)/Math.Sqrt(maxOccurance);
            }
        }
    }
}
