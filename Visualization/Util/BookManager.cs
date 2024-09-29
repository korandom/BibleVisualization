using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

using System.Linq;

namespace Visualization.Util
{
    public class Book : ObservableObject
    {
        private readonly string name;
        private readonly IBrush colorBrush;
        private PathGeometry geometry;
        public int Number { get; }

        private double startPosition;

        public double StartPosition
        {
            get
            {
                if (startPosition == 0)
                {
                    startPosition = GetStartPosition();
                }
                return startPosition;
            }
        }
        private double endPosition;
        public double EndPosition
        {
            get
            {
                if (endPosition == 0)
                {
                    endPosition = GetEndPosition();
                }
                return endPosition;
            }
        }
        public string Name => name;
        public IBrush ColorBrush => colorBrush;

        public PathGeometry Geometry
        {
            get => geometry;
            set => SetProperty(ref geometry, value);
        }
        public Dictionary<int, Chapter> Chapters { get; } = new Dictionary<int, Chapter>();

        public int VerseCount => Chapters.Values.Sum(chapter => chapter.VerseCount);

        private int bookOccurance = 0;
        public int DistributableOccurance { get; set; } = 0;
        public int Occurance => bookOccurance;
        public Book(int number, string name, string color, List<int> versesPerChapter)
        {
            this.Number = number;
            this.name = name;
            this.colorBrush = Brush.Parse(color);
            this.geometry = new PathGeometry();
            int i = 1;
            foreach(int verseCount in versesPerChapter)
            {
                Chapters[i] = new Chapter(i,verseCount);
                i++;
            }
        }
        private double GetStartPosition()
        {
            Chapter firstChapter = Chapters.Values.First();
            Verse firstVerse = firstChapter.Verses.Values.First();
            return firstVerse.Position;
        }

        private double GetEndPosition()
        {
            Chapter lastChapter = Chapters.Values.Last();
            Verse lastVerse = lastChapter.Verses.Values.Last();
            return lastVerse.Position;
        }

        public double GetVersePosition(int chapterNumber, int verseNumber)
        {
            if (chapterNumber == 0) chapterNumber = 1;
            if(verseNumber == 0) verseNumber = 1;
            return Chapters[chapterNumber].Verses[verseNumber].Position;
        }

        public void MarkOccurance(Reference reference, int occurance)
        {
            bookOccurance += occurance;
            if(reference.chapterStart == 0)
            {
                DistributableOccurance += occurance;
            }
            else
            {
                int chapterStop = reference.chapterEnd == 0 ? reference.chapterStart : reference.chapterEnd;
                for(int i = reference.chapterStart; i <= chapterStop; i++)
                {
                    if(Chapters.TryGetValue(i, out Chapter chapter))
                    {
                        chapter.MarkOccurance(reference, occurance);
                    }
                }
            }
        }
    }

    public class Chapter
    {
        public int Number;

        private double startPosition;

        public double StartPosition
        {
            get
            {
                if (startPosition == 0)
                {
                    startPosition = Verses.Values.First().Position;  
                }
                return startPosition;  
            }
        }
        private double endPosition;
        public double EndPosition 
        {
            get
            {
                if(endPosition == 0)
                {
                    endPosition = Verses.Values.Last().Position;
                }
                return endPosition;
            }
        }
        public Dictionary<int, Verse> Verses { get; } = new Dictionary<int, Verse>();

        public int VerseCount { get; }

        private int chapterOccurance = 0;
        public int DistributableOccurance { get; set; } = 0;
        public int Occurance => chapterOccurance;

        // Create chapter with number of verses from beggining
        public Chapter(int number, int verseCount)
        {
            Number = number;
            VerseCount = verseCount;
            for (int i = 1; i <= VerseCount; i++)
            {
                Verses[i] = new Verse();
            }
        }

        public void MarkOccurance(Reference reference, int occurance)
        {
            chapterOccurance += occurance;
            if (reference.verseStart == 0 || (Number > reference.chapterStart && Number < reference.chapterEnd))
            {
                DistributableOccurance += occurance;
                return;
            }
            int chapterStop = (reference.chapterEnd == 0) ? reference.chapterStart : reference.chapterEnd;
            int verseStop = (reference.verseEnd == 0) ? reference.verseStart : reference.verseEnd;

            // spanning one chapter only
            if (chapterStop == reference.chapterStart)
            {
                for (int i = reference.verseStart; i <= verseStop; i++)
                {
                    if (Verses.TryGetValue(i, out Verse verse))
                    {
                        verse.Occurance += occurance;
                    }
                }
                return;
            }
            // first chapter in a range of chapters - verses after verseStart count
            if (Number == reference.chapterStart)
            {
                foreach (var kvp in Verses)
                {
                    if (kvp.Key >= reference.verseStart)
                    {
                        kvp.Value.Occurance += occurance;
                    }
                }
                return;
            }
            // last chapter in range - verses before verseEnd count
            if (Number == reference.chapterEnd)
            {
                foreach (var kvp in Verses)
                {
                    if(kvp.Key <= reference.verseEnd)
                    {
                        kvp.Value.Occurance += occurance;
                    }
                }
            }
        }
    }

    public class Verse
    {
        // angle for chord, distance for line
        public double Position { get; set; }
        public int Occurance { get; set; }
    }

    public class BookManager
    {
        ObservableCollection<Book> books = new ObservableCollection<Book>();

        public void LoadBooks(List<int> bookNumbers)
        {
            var section = (BookSection)ConfigurationManager.GetSection("bookSection");
            if (section != null)
            {
                foreach (BookElement element in section.Books)
                {
                    if (bookNumbers.Contains(element.Number))
                    {
                        List<int> chapterVerses = element.Chapters.Split(',').Select(int.Parse).ToList();
                        books.Add(new Book(element.Number, element.LongName, element.Color, chapterVerses));
                    }
                }
            }

        }

        public ObservableCollection<Book> GetBooksWithAngles()
        {
            int totalVerseCount = books.Sum(book => book.VerseCount);
            int gap = getGap();
            int gapSum = gap * books.Count;
            double anglePerVerse = 2 * Math.PI / (totalVerseCount + gapSum);
            double equalAnglePerBook = (2 * Math.PI - (gapSum * anglePerVerse)) / books.Count;
            bool sameLengthBooks = areBooksSameLength();
            double specificBookAngle = 0.0;
            double currentAngle = 0;

            // Assign angles to each verse in each book
            foreach (var book in books)
            {
                currentAngle += gap * anglePerVerse;

                // calculate verse distribution if books are the same length
                if (sameLengthBooks)
                {
                    specificBookAngle = equalAnglePerBook/book.VerseCount;
                }
                foreach (var chapter in book.Chapters.Values)
                {
                    foreach (var verse in chapter.Verses.Values)
                    {
                        verse.Position = currentAngle;
                        currentAngle += sameLengthBooks ? specificBookAngle : anglePerVerse;
                    }
                }
            }

            return books;
        }

        public ObservableCollection<Book> GetBooksOnLine(bool topLine)
        {
            int topFactor = -1;
            if (topLine)
            {
                topFactor = 1;
            }
            int totalVerseCount = books.Sum(book => book.VerseCount);
            int gap = getGap();
            int gapSum = gap * (books.Count - 1);
            double partPerVerse =( 1.0 / (totalVerseCount + gapSum)) * topFactor;
            double equalPartPerBook = (1.0 - (gapSum * partPerVerse)) / books.Count;
            bool sameLengthBooks = areBooksSameLength();
            double specificBookPart = 0.0;

            // top line is defined with positions higher than zero, so take one filler verse
            double currentPosition = topLine ? partPerVerse : 0;

            // Assign procentual position to each verse in each book
            foreach (var book in books)
            {
                // calculate verse distribution if books are the same length
                if (sameLengthBooks)
                {
                    specificBookPart = (equalPartPerBook / book.VerseCount) * topFactor;
                }
                foreach (var chapter in book.Chapters.Values)
                {
                    foreach (var verse in chapter.Verses.Values)
                    {
                        verse.Position = currentPosition;
                        currentPosition += sameLengthBooks ? specificBookPart: partPerVerse;
                    }
                }
                currentPosition += gap * partPerVerse;
            }
            return books;
        }

        private int getGap()
        {
            string? gapString = ConfigurationManager.AppSettings.Get("BookGap");
            int gap;
            if (int.TryParse(gapString, out gap)) {
                return gap;
            }
            throw new Exception("Invalid configuration for BookGap, must be an integer.");
        }

        private bool areBooksSameLength()
        {
            string? visualBookLength = ConfigurationManager.AppSettings.Get("VisualBookLength");
            if (visualBookLength == "sameForAll")
            {
                return true;
            }
            if(visualBookLength == "proportional")
            {
                return false;
            }
            throw new Exception("Invalid configuration for VisualBookLength, must be \"proportional\" or \"sameForAll\".");
        }

    }
}
