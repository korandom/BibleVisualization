namespace DataStructures
{
    public struct Reference
    {
        public int book;
        public int chapterStart;
        public int verseStart = 0;
        public int chapterEnd = 0;
        public int verseEnd = 0;

        public Reference(int book, int chapter, int verse = 0)
        {
            this.book = book;
            this.chapterStart = chapter;
            this.verseStart = verse;
        }
        public Reference(int book, int chapterStart, int verseStart, int chapterEnd, int verseEnd)
        {
            this.book = book;
            this.chapterStart = chapterStart;
            this.verseStart = verseStart;
            this.chapterEnd = chapterEnd;
            this.verseEnd = verseEnd;
        }
    }
    public static class ReferenceExtentions
    {/*
        //Possible String References:
        //B:<book number> <chapter number>:<verse number>[-[<end chapter number>:]<end verse number>]
        //B:<book number> <chapter number>:<verse number #1>,<verse number #2>[<more comma-separated verse numbers>]
        //B:<book number> <chapter number>
        //B:<book number> <chapter number start>-<chapter number end>
        //B:<book number>
        */
        static public List<Reference> ToTarget(this string unparsed) //input is a string reference without "B:"
        {
            int l = unparsed.Length;
            char[] delimeters = new char[] { ' ', ':', '-', ',' };
            List<char> targetDelimeters = new List<char>();
            List<int> targetNumbers = new List<int>();
            List<Reference> targets = new List<Reference>();
            int number = 0;
            for (int i = 0; i < l; i++)
            {
                int newDigit = unparsed[i] - '0';
                if (newDigit >= 0 && newDigit <= 9)
                {
                    number = number * 10 + newDigit;
                }
                else if (delimeters.Contains(unparsed[i]))
                {
                    targetNumbers.Add(number);
                    number = 0;
                    if (unparsed[i] != ' ') { targetDelimeters.Add(unparsed[i]); }
                }
                else { throw new FormatException(); }
            }
            int count = targetNumbers.Count;
            int delCount = targetDelimeters.Count;

            if (count == 0) { throw new FormatException(); }

            int book = targetNumbers[0];
            if (count == 1)         //B:<book number
            {
                targets.Add(new Reference(book, 0));
                return targets;
            }
            
            int chapterStart = targetNumbers[1];

            if (count == 2) //B:<book number> <chapter number>
            {
                targets.Add(new Reference(book, chapterStart));
                return targets;
            }
            if (delCount == 0) { throw new FormatException(); }
        
            if (count == 3 && targetDelimeters[0] == '-') //B:<book number> <chapter number start>-<chapter number end>
            {
                targets.Add(new Reference(book, chapterStart, default, targetNumbers[2], default));
                return targets;
            }
            int verseStart = targetNumbers[2];

            if (count == 3) //B:<book number> <chapter number>:<verse number>
            {
                targets.Add(new Reference(book, chapterStart, verseStart));
                return targets;
            }
            bool verseAndSpan = (targetDelimeters[0] == ':' && targetDelimeters[1] == '-');
            if (count == 4 && delCount == 2 && verseAndSpan) //B:<book number> <chapter number>:<verse number>-<end verse number>
            {
                targets.Add(new Reference(book, chapterStart, verseStart, default, targetNumbers[3]));
                return targets;
            }
            if (count == 5 && delCount == 3 && verseAndSpan) //B:<book number> <chapter number>:<verse number>-<end chapter number>:<end verse number>
            {
                targets.Add(new Reference(book, chapterStart, verseStart, targetNumbers[3], targetNumbers[4]));
                return targets;
            }
            bool stackedVerse = targetDelimeters.Skip(1).All(d => d == ',');
            if (stackedVerse && count - 2 == delCount) //B:<book number> <chapter number>:<verse number #1>,<verse number #2>[<more comma-separated verse numbers>]
            {
                for (int i = 2; i < count; i++)
                {
                    targets.Add(new Reference(book, chapterStart, targetNumbers[i]));
                }
                return targets;
            }
            throw new FormatException();
        }
    }
}