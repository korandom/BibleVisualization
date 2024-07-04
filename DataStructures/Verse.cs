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
        public static Dictionary<int, string> bookNumberToName;

        static NumberToBook()
        {
            bookNumberToName = LoadDictionaryFromConfig();
        }

        private static Dictionary<int, string> LoadDictionaryFromConfig()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            var section = (BookSection)ConfigurationManager.GetSection("bookSection");
            if (section != null)
            {
                foreach (BookElement element in section.Books)
                {
                    dictionary[element.Number] = element.ShortName;
                }
            }
            return dictionary;
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
            this.bookShort = NumberToBook.bookNumberToName[book];
            this.chapter = chapter;
            this.verse = verse;
            this.text = text;
        }
    }
}
