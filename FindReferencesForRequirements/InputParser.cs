using DataStructures;
namespace FindLinksForRequirements
{
    public class InputParser
    {
        Dictionary<string, int> bookNameToNumber;
        public InputParser() //sets default dictionary if no is provided
        {
            bookNameToNumber = new Dictionary<string, int> //Dictionary from short names to book numbers
                                                           //taken from czech Bible translation info description "Ekumenický překlad"
            {
                { "Gen", 10 }, { "Exo", 20 },{ "Lev", 30 },{ "Num", 40 },
                { "Deu", 50 },{ "Joz", 60 },{ "Sou", 70 },{ "Rut", 80 },
                { "1Sa", 90 },{ "2Sa", 100 },{ "1Krá", 110 },{ "2Krá", 120 },
                { "1Par", 130 },{ "2Par", 140 },{ "Ezd", 150 },{ "Neh", 160 },
                { "Est", 190 },{ "Job", 220 },{ "Žalm", 230 },{ "Přís", 240 },
                { "Kaz", 250 },{ "Pís", 260 },{ "Iz", 290 },{ "Jer", 300 },
                { "Pláč", 310 },{ "Ez", 330  },{ "Dan", 340 },{ "Oze", 350 },
                { "Joel", 360 },{ "Amos", 370 },{ "Abd", 380 },{ "Jon", 390 },
                { "Mic", 400 },{ "Nah", 410 },{ "Aba", 420 },{ "Sof", 430 },
                { "Ag", 442 },{ "Zac", 450 },{ "Mal", 460 },{ "Mat", 470 },
                { "Mar", 480 },{ "Luk", 490 },{ "Jan", 500 },{ "Skut", 510 },
                { "Řím", 520 },{ "1Kor", 530 },{ "2Kor", 540 },{ "Gal", 550 },
                { "Ef", 560 },{ "Fil", 570 },{ "Kol", 580 },{ "1Tes", 590 },
                { "2Tes", 600 },{ "1Tim", 610 },{ "2Tim", 620 },{ "Tit", 630 },
                { "Flm", 640 },{ "Žid", 650 },{ "Jak", 660 },{ "1Pe", 670 },
                { "2Pe", 680 },{ "1Jan", 690 },{ "2Jan", 700 },{ "3Jan", 710 },
                { "Jud", 720 },{ "Zjev", 730 }
            };
        }
        public InputParser(Dictionary<string, int> bookNameToNumber)
        {
            this.bookNameToNumber = bookNameToNumber;
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
            if (bookNameToNumber.TryGetValue(bookName, out int bookNumber))
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