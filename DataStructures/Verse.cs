using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public static class NumberToBook
    {
        public static Dictionary<int, string> bookNumberToShortName = new Dictionary<int, string>();
        public static Dictionary<int, string> bookNumberToLongName = new Dictionary<int, string>();

        static NumberToBook()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var section = (BookSection)ConfigurationManager.GetSection("bookSection");
            if (section != null)
            {
                foreach (BookElement element in section.Books)
                {
                    bookNumberToShortName[element.Number] = element.ShortName;
                    bookNumberToLongName[element.Number] = element.LongName;
                }
            }
        }
    }
    public struct Verse
    {
        public readonly int book;
        public readonly string bookShort;
        public readonly int chapter;
        public readonly int verse;
        public readonly string text;

        public Verse(int book, int chapter, int verse, string text)
        {
            this.book = book;
            this.bookShort = NumberToBook.bookNumberToShortName[book];
            this.chapter = chapter;
            this.verse = verse;
            this.text = text;
        }
    }
}
