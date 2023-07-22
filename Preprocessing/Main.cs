namespace Preprocessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Preprocessor cp = new Preprocessor("C:\\Users\\marie\\Desktop\\skola\\2. leto\\bible\\BibleVizualization\\dataSources\\ToPreprocess", "C:\\Users\\marie\\Desktop\\skola\\2. leto\\bible\\BibleVizualization\\dataSources\\Preprocessed");
            cp.Process("MHWBC.commentaries.SQLite3");
        }
    }
}
