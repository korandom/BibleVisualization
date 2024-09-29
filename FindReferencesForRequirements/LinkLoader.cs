using DataStructures;
using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindLinksForRequirements
{
    public enum OneTextBoxState {  inside, to, from, all };
    public enum OriginType { crossreferences, commentaries, dictionary };
    public struct CrossreferenceData
    {
        public readonly string folderPath;
        public readonly OriginType originType;
        public List<string> filesToUse = new List<string>();
        public bool all = true;

        public CrossreferenceData(string folderPath, OriginType origin)
        {
            this.folderPath = folderPath;
            this.originType = origin;
        }
    }
    public class ReferenceLinkLoader
    {
        CrossreferenceData crossreferences;
        CrossreferenceData fromCommentaries;
        CrossreferenceData dictionaries;
        public Dictionary<string, List<Link>> themeLinks = new Dictionary<string, List<Link>>();
        public List<Link> referenceLinks = new List<Link>();
        public ReferenceLinkLoader(string folderPathFromCommentaries, string folderPathCrossreferences, string folderPathDictionaries,
                                   List<string>? commentariesList = null, List<string>? crossreferencesList = null,
                                   List<string>? dictionariesList = null)
        {
            this.fromCommentaries = new CrossreferenceData(folderPathFromCommentaries, OriginType.commentaries);
            this.crossreferences = new CrossreferenceData(folderPathCrossreferences, OriginType.crossreferences);
            this.dictionaries = new CrossreferenceData(folderPathDictionaries, OriginType.dictionary);
            if (commentariesList != null) { AddFileRestriction(commentariesList, fromCommentaries); }
            if (crossreferencesList != null) { AddFileRestriction(crossreferencesList, crossreferences); }
            if (dictionariesList != null) { AddFileRestriction(dictionariesList, dictionaries); }
            SetReferenceLinkList();
            SetThemeLinkList();
        }
        // Find Reference Links for one requirement
        public List<Link> FindLinks(List<Reference> requierements, OneTextBoxState state)
        {
            ConcurrentBag<Link> links = new ConcurrentBag<Link>();


            Parallel.ForEach(referenceLinks, link =>
            {
                if (LinkSatisfiesRequirements(link, requierements, state))
                {
                    links.Add(link);
                }
            });

            return links.ToList();
        }

        // Find Reference Links between two requirements
        public List<Link> FindLinksTwo(List<Reference> requirementsFrom, List<Reference> requirementsTo)
        {
            ConcurrentBag<Link> links = new ConcurrentBag<Link>();


            Parallel.ForEach(referenceLinks, link =>
            {
                if (LinkSatisfiesRequirementsTWO(link, requirementsFrom, requirementsTo))
                {
                    links.Add(link);
                }
            });

            return links.ToList();
        }
        // Find Links for Theme 
        public List<Link> FindThemeLinks(string theme)
        {
            if (themeLinks.ContainsKey(theme)) {
                return themeLinks[theme];
            }
            else return new List<Link>();
        }

        public List<string> GetThemes ()
        {
            return themeLinks.Select(kvp => kvp.Key).ToList();
        }

        private void AddFileRestriction(List<string> fileList, CrossreferenceData data)
        {
            data.filesToUse = fileList;
            data.all = false;
        } 
        private void SetThemeLinkList()
        {
            string[] allFilePaths = Directory.GetFiles(dictionaries.folderPath);

            // load from files
            List<Dictionary<string, List<Link>>> listFromAll = new List<Dictionary<string, List<Link>>>();
            Dictionary<string, Dictionary< int, List<Link>>> noDuplicates = new Dictionary<string, Dictionary<int, List<Link>>>();
            Parallel.ForEach(allFilePaths, filePath =>
            {
                string fileName = Path.GetFileName(filePath);
                if (dictionaries.all == true || dictionaries.filesToUse.Contains(fileName))
                {
                    string[] parts = fileName.Split(".");
                    if ((parts[1] == "dictionary" && parts[2] == "crossreferences"))
                    {
                        Dictionary<string, List<Link>> dic = LoadLinksFromDictionary(filePath);
                        lock (listFromAll)
                        {
                            listFromAll.Add(dic);
                        }
                    }
                }
            });

            // remove duplicates
            foreach(var dictionary in listFromAll)
            {
                foreach(var kvp in dictionary)
                {
                    if (!noDuplicates.ContainsKey(kvp.Key))
                    {
                        noDuplicates[kvp.Key] = new Dictionary<int, List<Link>>();
                    }
                    foreach(var link in kvp.Value)
                    {
                        int helpKey = link.GetHashCode();
                        if (!noDuplicates[kvp.Key].ContainsKey(helpKey))
                        {
                            noDuplicates[kvp.Key][helpKey] = new List<Link>{link};
                        }
                        else
                        {
                            bool found = false;
                            foreach (Link linkFromDic in noDuplicates[kvp.Key][helpKey])
                            {
                                if (linkFromDic == link)
                                {
                                    found = true;
                                    int occurance = link.Occurance;
                                    linkFromDic.IncreaseOccuranceBy(occurance);
                                    break;
                                }
                            }
                            if (!found)
                            {
                                noDuplicates[kvp.Key][helpKey].Add(link);
                            }
                        }

                    }
                }
            }
            // remove help key
            Dictionary<string, List<Link>> links = new Dictionary<string, List<Link>>();
            foreach(var topic in noDuplicates.Keys)
            {
                if (!links.ContainsKey(topic))
                {
                    links[topic] = new List<Link>();
                }
                foreach (var helpKey in noDuplicates[topic].Keys)
                {
                    links[topic].AddRange(noDuplicates[topic][helpKey]);
                }
            }

            themeLinks = links;
        }
        private void SetReferenceLinkList()
        {
            Dictionary<int, Dictionary<int, List<Link>>> dictionary = GetAllLinksWithoutDuplicates();
            List<Link> links = new List<Link>();


            foreach(int book in dictionary.Keys)
            {
                foreach (int hash in dictionary[book].Keys)
                {
                    foreach (Link link in dictionary[book][hash])
                    {
                        links.Add(link);
                    }

                }
            }

            referenceLinks = links;

        }


        private Dictionary<int, Dictionary<int, List<Link>>> GetAllLinksWithoutDuplicates()
        {
            List<List<Link>> links = new List<List<Link>>
            {
                GetLinksFromAllFiles( crossreferences),
                GetLinksFromAllFiles( fromCommentaries)
            };
            Dictionary<int, Dictionary<int, List<Link>>> noDuplicatesLinks = new Dictionary<int, Dictionary<int, List<Link>>>();
            for (int i = 0; i < links.Count; i++)
            {
                foreach (Link link in links[i])
                {
                    int bookNumber = link.source.book;
                    int key = link.GetHashCode();
                    if (noDuplicatesLinks.ContainsKey(bookNumber))
                    {
                        if (noDuplicatesLinks[bookNumber].ContainsKey(key))
                        {
                            bool found = false;
                            foreach (Link linkFromDic in noDuplicatesLinks[bookNumber][key])
                            {
                                if (linkFromDic == link)
                                {
                                    found = true;
                                    int occurance = link.Occurance;
                                    linkFromDic.IncreaseOccuranceBy(occurance);
                                    break;
                                }
                            }
                            if (!found)
                            {
                                noDuplicatesLinks[bookNumber][key].Add(link);
                            }
                        }
                        else
                        {
                            noDuplicatesLinks[bookNumber][key] = new List<Link> { link };
                        }
                    }
                    else
                    {
                        noDuplicatesLinks[bookNumber] = new Dictionary<int, List<Link>>();
                        noDuplicatesLinks[bookNumber][key] = new List<Link> { link };
                    }
                }
            }

            return noDuplicatesLinks;
        }
        private List<Link> GetLinksFromAllFiles(CrossreferenceData data)
        {
            string[] allFilePaths = Directory.GetFiles(data.folderPath);
            
            List<Link> final = new List<Link>();
            List<List<Link>> listFromAll = new List<List<Link>>();
            Parallel.ForEach(allFilePaths, filePath =>
            {
                string fileName = Path.GetFileName(filePath);
                if (data.all == true || data.filesToUse.Contains(fileName))
                {
                    string[] parts = fileName.Split(".");
                    if ((parts[1] == "commentaries" && parts[2] == "crossreferences") || parts[1] == "crossreferences")
                    {
                        List<Link> list = LoadLinksOneFile(filePath, data.originType);
                        listFromAll.Add(list);
                    }
                }
            });
            foreach (List<Link> list in listFromAll)
            {
                foreach (Link link in list)
                {
                    final.Add(link);
                }
            }
            return final;
        }
        private List<Link> LoadLinksOneFile(string dataFilePath, OriginType origin)
        {
            List<Link> results = new List<Link>();

            using (var connection = new SqliteConnection($"Data Source={dataFilePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                string Columns = "";
                if (origin == OriginType.commentaries)
                {
                    Columns = @"book, chapter, verse, chapter_end, verse_end,
                            book_to, chapter_to, verse_to, chapter_to_end,
                            verse_to_end";
                }
                else if (origin == OriginType.crossreferences)
                {
                    Columns = @"book, chapter, verse, verse_end,
                         book_to, chapter_to, verse_to_start,
                         verse_to_end";
                }
                command.CommandText = $"SELECT {Columns} FROM cross_references";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var book = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        var chapter = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        var verse = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        int i = 3;
                        int chapterEnd;
                        if (origin == OriginType.commentaries)
                        {
                            chapterEnd = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        }
                        else
                        {
                            chapterEnd = 0;
                        }
                        var verseEnd = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        var bookTo = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        var chapterTo = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        var verseTo = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        int chapterToEnd;
                        if (origin == OriginType.commentaries)
                        {
                            chapterToEnd = reader.IsDBNull(i) ? 0 : reader.GetInt32(i++);
                        }
                        else
                        {
                            chapterToEnd = 0;
                        }
                        var verseToEnd = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        Reference source = new Reference(book, chapter, verse, chapterEnd, verseEnd);
                        Reference target = new Reference(bookTo, chapterTo, verseTo, chapterToEnd, verseToEnd);
                        Link link = new Link(source, target);

                        results.Add(link);

                    }
                }
            }
            return results;
        }

        private Dictionary<string, List<Link>> LoadLinksFromDictionary(string dataFilePath)
        {
            Dictionary<string, List<Link>> results = new Dictionary<string, List<Link>> ();

            using (var connection = new SqliteConnection($"Data Source={dataFilePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                string Columns = @"topic, book, chapter, verse, chapter_end, verse_end";
                
                command.CommandText = $"SELECT {Columns} FROM dictionary_references";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string topic = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        var book = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        var chapter = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        var verse = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        var chapterEnd = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        var verseEnd = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                        
                        Reference source = new Reference(book, chapter, verse, chapterEnd, verseEnd);
                        Link link = new Link(source);

                        if(!results.ContainsKey(topic))
                        {
                            results[topic] = new List<Link>();
                        }
                        results[topic].Add(link);
                    }
                }
            }
            return results;
        }

        private static bool LinkSatisfiesRequirements(Link link, List<Reference> requirements, OneTextBoxState state)
        {
            return state switch
            {
                OneTextBoxState.from => link.From(requirements),
                OneTextBoxState.to => link.To(requirements),
                OneTextBoxState.all => link.All(requirements),
                OneTextBoxState.inside => link.Inside(requirements),
                _ => false,
            };
        }
        private static bool LinkSatisfiesRequirementsTWO(Link link, List<Reference> requirementsFrom,
                                                                    List<Reference> requirementsTo)
        {
            return link.From(requirementsFrom) && link.To(requirementsTo);
        }

        public Dictionary<int, Dictionary<int, int>> GetHistogramData(List<Link> links)
        {
            Dictionary<int, Dictionary<int, int>> data = new Dictionary<int, Dictionary<int, int>>();
            int count = links.Count;
            for (int i = 0; i < count; i++)
            {
                AddToData(links[i].source, data, links[i].Occurance);
                if (links[i].target is Reference validTarget)
                {
                    AddToData(validTarget, data, links[i].Occurance);
                }
            }
            return data;
        }

        private void AddToData(Reference reference, Dictionary<int, Dictionary<int, int>> data, int occurance)
        {
            if (!data.ContainsKey(reference.book))
            {
                data[reference.book] = new Dictionary<int, int>();
            }
            for (int i = reference.chapterStart; i <= reference.chapterEnd; i++)
            {
                if (!data[reference.book].ContainsKey(i))
                {
                    data[reference.book][i] = occurance;
                }
                else
                {
                    data[reference.book][i] += occurance;
                }
            }
        }
    }
}
