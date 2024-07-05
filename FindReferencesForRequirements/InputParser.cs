using DataStructures;
using System.Configuration;
using System.Globalization;
using System.Text;
namespace FindLinksForRequirements
{
    public class InputParser
    {
        Dictionary<string, int> bookShortNameToNumber;
        public InputParser()
        {
            bookShortNameToNumber = LoadDictionaryFromConfig();
        }

        private Dictionary<string, int> LoadDictionaryFromConfig()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            var section = (BookSection)ConfigurationManager.GetSection("bookSection");
            if (section != null)
            {
                foreach (BookElement element in section.Books)
                {
                    string universalShortName = RemoveDiacritics(element.ShortName).ToLower();
                    dictionary[universalShortName] = element.Number;
                }
            }
            return dictionary;
        }
        private static string RemoveDiacritics(string text)
        {
            var decomposedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in decomposedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        public List<Reference> Parse(string input) //FormatException if Wrong Format- correct format is <bookName> <chapterNumer>:<verseNumber>;<bookname ; [morereferences]
                                                   //and alike defined in DataStructures.Reference.cs
        {
            if (string.IsNullOrEmpty(input)) throw new FormatException("No requirement input");
            List<Reference> references = new List<Reference>();
            string[] devided = input.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string individual in devided)
            {
                string withBookNumber = ChangeNameForNumber(individual);
                List<Reference> refs = withBookNumber.ToReference();
                for(int i = 0; i < refs.Count; i++)
                {
                    references.Add(refs[i]);
                }
            }
            return references;
        }
        
        public string  ChangeNameForNumber(string textReference)
        {
            int l = textReference.Length;
            string bookName = "";
            string rest = "";
            int i = 0;
            while (i<l && textReference[i] == ' ') i++;
            while (i < l && textReference[i] != ' ')
            {
                bookName += textReference[i];
                i++;
            }
            while (i < l)
            {
                rest += textReference[i];
                i++;
            }
            bookName = RemoveDiacritics(bookName).ToLower();
            if (bookShortNameToNumber.TryGetValue(bookName, out int bookNumber))
            {
                return bookNumber + rest;
            }
            else
            {
                throw new FormatException("Invalid book name.");
            }
        } 
    }
}