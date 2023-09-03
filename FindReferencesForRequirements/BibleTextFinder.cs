using DataStructures;
using Microsoft.Data.Sqlite;
using System.Collections;

namespace FindReferencesForRequirements
{
    
    public class BibleText : IEnumerable
    {
        string pathBible;
        public Dictionary<(int, int), List<string>> verses;

        public BibleText(string pathBible)
        {
            this.pathBible = pathBible;
            verses = LoadBible();
        }
        public void ChangePath(string path)
        {
            pathBible = path;
            verses = LoadBible();
        }
        private Dictionary<(int, int), List<string>> LoadBible()
        {
            Dictionary<(int, int), List<string>> verses = new Dictionary<(int, int), List<string>>();
            using (var connection = new SqliteConnection($"Data Source={pathBible}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM verses";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var book = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        var chapter = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                        var text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        if (!verses.ContainsKey((book, chapter)))
                        {
                            verses[(book, chapter)] = new List<string>();
                        }
                        verses[(book, chapter)].Add(text);
                    }
                }
            }
            return verses;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator(new Reference(0, 0));
        }

        public VerseEnumerator GetEnumerator(Reference reference)
        {
            return new VerseEnumerator(verses, reference);
        }
    }

    public class VerseEnumerator : IEnumerator
    {
        int firstbook = 10;
        private Dictionary<(int, int), List<string>> verses;
        Reference reference;
        Verse current;
        bool initialized;

        public VerseEnumerator(Dictionary<(int, int), List<string>> verses, Reference reference)
        {
            this.verses = verses;
            this.reference = reference;
            this.initialized = false;
            current = new Verse();
        }
        private bool Initialize()
        {
            int book = (reference.book == 0) ? firstbook : reference.book;
            int chapter = (reference.chapterStart == 0) ? 1 : reference.chapterStart;
            int verse = (reference.verseStart == 0) ? 1 : reference.verseStart;

            if (verses.ContainsKey((book, chapter)) && verses[(book, chapter)].Count >= verse)
            {
                current = new Verse(book, chapter, verse, verses[(book, chapter)][verse - 1]);
                return true;
            }
            return false;

        }
        object IEnumerator.Current
        {
            get
            {
                return current;
            }
        }
        public Verse Current()
        {
            return current;
        }

        private Verse ToVerse(Reference next)
        {
            return new Verse(next.book, next.chapterStart, next.verseStart,
                             verses[(next.book, next.chapterStart)][next.verseStart - 1]);
        }
        public bool CheckNext()
        {
            return Next(out Verse _);
        }
        public bool MoveNext()
        {
            if (Next(out Verse verse))
            {
                current = verse;
                return true;
            }
            return false;
        }
        private bool Next(out Verse verse)
        {
            if (!initialized && Initialize())
            {
                initialized = true;
                verse = current;
                return true;
            }
            if (verses[(current.book, current.chapter)].Count >= current.verse + 1)
            {
                Reference next = new Reference(current.book, current.chapter, current.verse + 1);
                verse = ToVerse(next);
                return next.FitsInto(reference);
            }
            if (verses.ContainsKey((current.book, current.chapter + 1)))
            {
                Reference next = new Reference(current.book, current.chapter + 1, 1);
                verse = ToVerse(next);
                return next.FitsInto(reference);
            }
            verse = new Verse();
            return false;
        }
        public bool MoveBack()
        {
            if(Back(out Verse verse))
            {
                current = verse;
                return true;
            }
            return false; 
        }
        public bool CheckBack()
        {
            return Back(out Verse _);
        }
        private bool Back(out Verse verse)
        {
            if (current.verse - 1 > 0)
            {
                Reference next = new Reference(current.book, current.chapter, current.verse - 1);
                verse = ToVerse(next);
                return next.FitsInto(reference);
            }
            if (verses.ContainsKey((current.book, current.chapter - 1)))
            {
                Reference next = new Reference(current.book, current.chapter - 1, verses[(current.book, current.chapter - 1)].Count);
                verse = ToVerse(next);
                return next.FitsInto(reference);
            }
            verse = new Verse();
            return false;
        }
        public void Reset()
        {
            initialized = false;
            current = new Verse();
        }
    }
}
