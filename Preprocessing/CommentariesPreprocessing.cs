using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    internal class CommentariesPreprocessing : SpecificFilePreprocessing
    {
        public readonly string createCrossreferenceTableText = 
            @"
                   CREATE TABLE ""cross_references"" (""book"" NUMERIC, ""chapter"" NUMERIC,
                                                      ""verse"" NUMERIC, ""chapter_end"" NUMERIC, ""verse_end"" NUMERIC,
                                                      ""book_to"" NUMERIC, ""chapter_to"" NUMERIC,
                                                      ""verse_to"" NUMERIC, ""chapter_to_end"" NUMERIC,
                                                      ""verse_to_end"" NUMERIC, ""votes"" NUMERIC) 
            ";
        public SqliteCommand PrepareInsertQuery(SqliteConnection connection, SqliteTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                @"
                            INSERT INTO ""cross_references""
                                        (""book"", ""chapter"", ""verse"",
                                        ""chapter_end"", ""verse_end"",
                                        ""book_to"", ""chapter_to"", ""verse_to"",
                                        ""chapter_to_end"", ""verse_to_end"", ""votes"")
                            VALUES ($bookFrom, $chapterFromStart, $verseFromStart, $chapterFromEnd, $verseFromEnd, $bookTo, $chapterToStart, $verseToStart, $chapterToEnd, $verseToEnd, $votes);   
                ";
            command.Parameters.AddWithValue("$bookFrom", 0);
            command.Parameters.AddWithValue("$chapterFromStart", 0);
            command.Parameters.AddWithValue("$verseFromStart", 0);
            command.Parameters.AddWithValue("$chapterFromEnd", 0);
            command.Parameters.AddWithValue("$verseFromEnd", 0);
            command.Parameters.AddWithValue("$votes", -1);
            command.Parameters.AddWithValue("$bookTo", 0);
            command.Parameters.AddWithValue("$chapterToStart", 0);
            command.Parameters.AddWithValue("$verseToStart", 0);
            command.Parameters.AddWithValue("$chapterToEnd", 0);
            command.Parameters.AddWithValue("$verseToEnd", 0);
            return command;
        }

        public void ReadAndWriteData(TargetParser targetParser, SqliteDataReader reader, SqliteCommand insertCommand)
        {
            while (reader.Read())
            {
                var bookFrom = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                var chapterFromStart = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                var verseFromStart = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                var chapterFromEnd = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                var verseFromEnd = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                var text = reader.IsDBNull(5) ? "" : reader.GetString(5);

                List<Reference> targets = targetParser.ParseTextForTargetsString(text);
                AddReferencesToFile(bookFrom, chapterFromStart, verseFromStart, chapterFromEnd, verseFromEnd, targets, insertCommand);
            }
        }
        private void AddReferencesToFile(int bookFrom, int chapterFromStart, int verseFromStart, int chapterFromEnd,
                                         int verseFromEnd, List<Reference> targets, SqliteCommand command)
        {
            command.Parameters["$bookFrom"].Value = bookFrom;
            command.Parameters["$chapterFromStart"].Value = chapterFromStart;
            command.Parameters["$verseFromStart"].Value = verseFromStart;
            command.Parameters["$chapterFromEnd"].Value = chapterFromEnd;
            command.Parameters["$verseFromEnd"].Value = verseFromEnd;
            foreach (var reference in targets)
            {
                command.Parameters["$bookTo"].Value = reference.book;
                command.Parameters["$chapterToStart"].Value = reference.chapterStart;
                command.Parameters["$verseToStart"].Value = reference.verseStart;
                command.Parameters["$chapterToEnd"].Value = reference.chapterEnd;
                command.Parameters["$verseToEnd"].Value = reference.verseEnd;

                command.ExecuteNonQuery();
            }
        }
    }
}
