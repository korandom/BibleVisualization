using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    internal class DictionaryPreprocessing : SpecificFilePreprocessing
    {
        public readonly string createCrossreferenceTableText =
            @"
                   CREATE TABLE ""dictionary_references"" (""topic"" TEXT NOT NULL DEFAULT '',
                                                      ""book"" NUMERIC, ""chapter"" NUMERIC,
                                                      ""verse"" NUMERIC, ""chapter_end"" NUMERIC, ""verse_end"" NUMERIC) 
            ";
        public SqliteCommand PrepareInsertQuery(SqliteConnection connection, SqliteTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                @"
                            INSERT INTO ""dictionary_references""
                                        (""topic"",
                                        ""book"", ""chapter"", ""verse"",
                                        ""chapter_end"", ""verse_end"")
                            VALUES ($topic, $book, $chapter, $verse, $chapterEnd, $verseEnd);   
                ";
            command.Parameters.AddWithValue("$topic", "");
            command.Parameters.AddWithValue("$book", 0);
            command.Parameters.AddWithValue("$chapter", 0);
            command.Parameters.AddWithValue("$verse", 0);
            command.Parameters.AddWithValue("$chapterEnd", 0);
            command.Parameters.AddWithValue("$verseEnd", 0);
            return command;
        }

        public void ReadAndWriteData(TargetParser targetParser, SqliteDataReader reader, SqliteCommand insertCommand)
        {
            while (reader.Read())
            {
                var topic = reader.GetString(0);
                var text = reader.GetString(1);

                List<Reference> targets = targetParser.ParseTextForTargetsString(text);
                AddReferencesToFile(topic, targets, insertCommand);
            }
        }
        private void AddReferencesToFile(string topic, List<Reference> targets, SqliteCommand command)
        {
            command.Parameters["$topic"].Value = topic;
            foreach (var reference in targets)
            {
                command.Parameters["$book"].Value = reference.book;
                command.Parameters["$chapter"].Value = reference.chapterStart;
                command.Parameters["$verse"].Value = reference.verseStart;
                command.Parameters["$chapterEnd"].Value = reference.chapterEnd;
                command.Parameters["$verseEnd"].Value = reference.verseEnd;

                command.ExecuteNonQuery();
            }
        }
    }
}
