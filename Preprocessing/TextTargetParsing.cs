namespace Preprocessing
{

    static class ReferenceExtentions
    {/*
        //Possible String References:
        //B:<book number> <chapter number>:<verse number>[-[<end chapter number>:]<end verse number>]
        //B:<book number> <chapter number>:<verse number #1>,<verse number #2>[<more comma-separated verse numbers>]
        //B:<book number> <chapter number>
        //B:<book number> <chapter number start>-<chapter number end>

        */
        static public List<Reference> ToTarget(this string unparsed)
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

            if (count < 2) { throw new FormatException(); }

            int book = targetNumbers[0];
            int chapterStart = targetNumbers[1];

            if (count == 2)
            {
                targets.Add(new Reference(book, chapterStart));
                return targets;
            }
            if (delCount == 0) { throw new FormatException(); }
            if (count == 3 && targetDelimeters[0] == '-')
            {
                targets.Add(new Reference(book, chapterStart, default, targetNumbers[2], default));
                return targets;
            }
            int verseStart = targetNumbers[2];

            if (count == 3)
            {
                targets.Add(new Reference(book, chapterStart, verseStart));
                return targets;
            }
            bool verseAndSpan = (targetDelimeters[0] == ':' && targetDelimeters[1] == '-');
            if (count == 4 && delCount == 2 && verseAndSpan)
            {
                targets.Add(new Reference(book, chapterStart, verseStart, default, targetNumbers[3]));
                return targets;
            }
            if (count == 5 && delCount == 3 && verseAndSpan)
            {
                targets.Add(new Reference(book, chapterStart, verseStart, targetNumbers[3], targetNumbers[4]));
                return targets;
            }
            bool stackedVerse = targetDelimeters.Skip(1).All(d => d == ',');
            if (stackedVerse && count - 2 == delCount)
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
    public class TargetParser
    {
        /*public void ParseTextForTargetsRegex(string text) //regex or wordbyword??
        {
            string pattern = @"(<a href='B:)([0-9,: -]*)'>";
            Regex reg = new Regex(pattern);
            MatchCollection matches = reg.Matches(text);
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
                string target = match.Value.Substring(11,match.Value.Length-2-11);
                Console.WriteLine(target);
            }

        }*/
        public List<Reference> ParseTextForTargetsString(string text) //parsing text for targets with string operations
                                                                      //faster then regex
        {
            char[] delimiters = { ' ', '\'' };
            string[] devided = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int l = devided.Length;
            List<Reference> targets = new List<Reference>();
            for (int i = 0; i < l - 1; i++)
            {
                if (devided[i] == "href=" && devided[i + 1][0] == 'B')
                {

                    string stringTarget = devided[i + 1].Substring(2) + " ";
                    i = i + 2;
                    while (devided[i][0] != '>')
                    {
                        stringTarget += devided[i] + " ";
                        i++;
                    }
                    try
                    {
                        List<Reference> target = stringTarget.ToTarget();
                        foreach (Reference single in target)
                        {
                            targets.Add(single);
                        }
                    }
                    catch (FormatException) { } //skipped
                }
            }
            return targets;
        }

    }
}
