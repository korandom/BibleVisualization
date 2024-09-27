using System.Runtime.ExceptionServices;
using System.Threading.Channels;

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
        public static bool operator ==(Reference first, Reference second)
        {
            return first.book == second.book &&
                   first.chapterStart == second.chapterStart &&
                   first.verseStart == second.verseStart &&
                   first.chapterEnd == second.chapterEnd &&
                   first.verseEnd == second.verseEnd;
        }
        public static bool operator !=(Reference first, Reference second)
        {
            return !(first == second);
        }
        public override bool Equals(object? obj)
        {
            return obj is Reference reference && Equals(reference);
        }
        public bool Equals(Reference reference)
        {
            return this == reference;
        }
        public override int GetHashCode()
        {

            const int prime1 = 31;
            const int prime2 = 37;
            const int prime3 = 41;
            const int primeMOD = 997;
            int hash1 = (book * prime1) %  primeMOD;
            int hash2 = (chapterStart * prime2  + chapterEnd) % primeMOD;
            int hash3 = (verseStart * prime3 + verseEnd) % primeMOD;
            int hash = hash1 + (hash2 * 1000) + (hash3 *1000000);
            return hash;
            
        } 
        public bool FitsInto(Reference requirement)
        {
            bool fits;
            if (requirement.book == 0) 
                return true;
            fits = book == requirement.book;
            if (requirement.chapterStart == 0)
                return fits;
            bool sameChapter = chapterStart == requirement.chapterStart && (chapterEnd == 0 || chapterEnd == chapterStart);
            if (requirement.verseStart == 0 && (requirement.chapterEnd == 0 || requirement.chapterEnd == requirement.chapterStart))
            {
                fits &= sameChapter;
                return fits;
            }
            if(requirement.verseStart == 0)
            {
                fits &= FitsIntoChapters(requirement);
                return fits;
            }
            if (requirement.chapterEnd == 0 || requirement.chapterEnd == requirement.chapterStart)
            {
                fits &= sameChapter;
                if(requirement.verseEnd == 0)
                {
                    fits &= requirement.verseStart == verseStart && verseEnd == 0;
                    return fits;
                }
                int verseEndImag = verseEnd == 0 ? verseStart : verseEnd;
                fits &= requirement.verseStart <= verseStart &&  requirement.verseEnd >= verseEndImag;
                return fits;
            }
            fits &= FitsIntoChapters(requirement) && FitsIntoMultipleVerses(requirement);
            return fits;

        }
        private bool FitsIntoChapters(Reference requirement)
        {
            return  chapterStart >= requirement.chapterStart && ((chapterEnd == 0 ? chapterStart: chapterEnd) <= requirement.chapterEnd);

        }
        private bool FitsIntoMultipleVerses(Reference requirement)
        {
            bool fits = true;
            if(chapterStart == requirement.chapterStart)
            {
                fits &= requirement.verseStart <= verseStart;
            }
            if (chapterEnd == requirement.chapterEnd)
            {
                fits &= requirement.verseEnd >= verseEnd;
            }
            return fits;
        }
        public int CompareTo(Reference y)
        {
            if (book == y.book)
            {
                if (chapterStart == y.chapterStart)
                {
                    if (verseStart == y.verseStart)
                    {
                        if (chapterEnd == y.chapterEnd)
                        {
                            if (verseEnd == y.verseEnd)
                            {
                                return 0;
                            }
                            return verseEnd - y.verseEnd;
                        }
                        return chapterEnd - y.chapterEnd;
                    }
                    return verseStart - y.verseStart;
                }
                return chapterStart - y.chapterStart;
            }
            return book - y.book;
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
        static public List<Reference> ToReference(this string unparsed) //input is a string reference without "B:"
        {
            int l = unparsed.Length;
            char[] delimeters = new char[] { ' ', ':', '-', ',', '.' };
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
                else { throw new FormatException("Invalid Delimeters or Characters"); }
            }
            if(number != 0) targetNumbers.Add(number);
            int count = targetNumbers.Count;
            int delCount = targetDelimeters.Count;

            if (count == 0) { throw new FormatException("No Valid Requirement"); }

            int book = targetNumbers[0];
            if (count == 1)         //B:<book number>
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
            if (delCount == 0) { throw new FormatException("Missing Delimeters in Requirement"); }
        
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
            bool verseAndSpan = ((targetDelimeters[0] == ':' || targetDelimeters[0] == '.') && targetDelimeters[1] == '-');
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
            throw new FormatException("Wrong Requirements Format");
        }
        
    }

}