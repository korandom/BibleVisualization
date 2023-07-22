using Microsoft.Data.Sqlite;
using System.IO;
namespace Preprocessing
{
    
    public interface SpecificFilePreprocessing
    {
        SqliteCommand PrepareInsertQuery(SqliteConnection connection, SqliteTransaction transaction);
        void ReadAndWriteData(TargetParser targetParser, SqliteDataReader reader, SqliteCommand insertCommand);
    }

    public class Preprocessor
    {
        readonly TargetParser targetParser;
        readonly string sourceFolderPath;
        readonly string resultFolderPath;
        public Preprocessor(string sourceFolderPath, string  resultFolderPath)
        {
            targetParser = new TargetParser();
            this.sourceFolderPath = sourceFolderPath;
            this.resultFolderPath = resultFolderPath;
        }
        public void ProcessAll()
        {
            string[] allFilesPaths = Directory.GetFiles(sourceFolderPath);
            foreach (var filePath in allFilesPaths)
            {
                Process(Path.GetFileName(filePath));
            }
        }
        public void Process(string fileName)
        {
            string fileType = GetFileType(fileName);
            if (fileType == "commentaries") 
            {
                CommentariesPreprocessing commentaries = new CommentariesPreprocessing();
                ToCrossReference<CommentariesPreprocessing>(fileName, commentaries.createCrossreferenceTableText,fileType, commentaries.usedColumns, commentaries);
            }
            else if (fileType == "dictionary") 
            {
                DictionaryPreprocessing dictionary = new DictionaryPreprocessing();
                ToCrossReference<DictionaryPreprocessing>(fileName, dictionary.createCrossreferenceTableText, fileType,dictionary.usedColumns, dictionary);
            }
            else throw new FormatException("Invalid File Format Name");
        }
        private void ToCrossReference<T>(string fileName, string createCrossreferenceTableText, string tableType, string usedColumns,  T SpecificFileType) where T : SpecificFilePreprocessing
        {
            string newFileName = GetName(fileName,tableType);
            string pathCrossreference = Path.Combine(resultFolderPath, newFileName);
            bool alreadyExists = false;
            using (var connection = new SqliteConnection($"Data Source={pathCrossreference}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = createCrossreferenceTableText;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException) { alreadyExists = true; } // to rewrite or to skip?  --> skip
            }
            if (!alreadyExists)
            {
                string pathOriginal = Path.Combine(sourceFolderPath, fileName);
                using (var connection = new SqliteConnection($"Data Source={pathOriginal}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT {usedColumns} FROM {tableType}";

                    using (var reader = command.ExecuteReader())
                    {
                        using (var connectionCrossreference = new SqliteConnection($"Data Source={pathCrossreference}"))
                        {
                            connectionCrossreference.Open();
                            using (var transaction = connectionCrossreference.BeginTransaction())
                            {
                                var insertCommand = SpecificFileType.PrepareInsertQuery(connectionCrossreference, transaction);
                                SpecificFileType.ReadAndWriteData(targetParser, reader, insertCommand);
                                transaction.Commit();
                            }
                        }
                    }
                }
            }

        }
        private string GetName(string fileName, string originalTableType)
        {
            string realName = "";
            int l = fileName.Length;
            for (int i = 0; i < l - 1; i++)
            {
                realName += fileName[i];
                if (fileName[i] == '.') break; 
            }
            realName += originalTableType + ".crossreferences.SQLite3";
            return realName;
        }
        private string GetFileType(string  fileName)
        {
            string type = "";
            int l = fileName.Length;
            bool firstDot = false;
            for(int i = 0; i < l -1; i++)
            {
                if (firstDot && fileName[i] == '.') break;
                if (firstDot)
                {
                    type += fileName[i];
                }
                if (fileName[i] == '.') firstDot = true;  
            }
            return type;
        }
    }  
}