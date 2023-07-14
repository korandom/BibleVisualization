using Microsoft.Data.Sqlite;

namespace CommentariesPreprocessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "JFB-c.commentaries.SQLite3";
            CommentariesToCrossReference(fileName);
        }
        static void CommentariesToCrossReference(string fileName)
        {
            using (var connection = new SqliteConnection($"Data Source={fileName}"))
            {
                connection.Open();

                int book_number = 10;
                int chapter_number_from = 1;

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT text
                    FROM commentaries
                    WHERE  book_number = $book_number AND chapter_number_from = $chapter_number_from
                ";
                command.Parameters.AddWithValue("$book_number", book_number);
                command.Parameters.AddWithValue("$chapter_number_from", chapter_number_from);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);

                        Console.WriteLine($"Hello, {name}!");
                    }
                }
            }
        }
        struct Target { int book; int chapterStart; int verseStart = 0; int chapterEnd = 0; int verseEnd = 0;

            public Target(int book, int chapter, int verse)
            {
                this.book = book;
                this.chapterStart = chapter;
                this.verseStart = verse;
            }
            public Target(int book, int chapterStart, int verseStart, int chapterEnd, int verseEnd)
            {
                this.book = book;
                this.chapterStart = chapterStart;
                this.verseStart = verseStart;
                this.chapterEnd = chapterEnd;
                this.verseEnd = verseEnd;
            }
        }
    }
}
