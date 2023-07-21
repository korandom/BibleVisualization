using DataStructures;
namespace Preprocessing
{
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
