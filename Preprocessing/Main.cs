using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "Novotný.dictionary.SQLite3";
            Preprocessor cp = new Preprocessor("C:\\Users\\marie\\Desktop\\skola\\2. leto\\bible\\BibleVizualization\\data_sources\\Dictionaries");
            cp.Process(fileName);
        }
    }
}
