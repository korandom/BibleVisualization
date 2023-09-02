using DataStructures;
using Microsoft.Data.Sqlite;

namespace FindLinksForRequirements
{
    public enum OneTextBoxState { all, from, to, inside };
    public enum OriginType { crossreferences, commentaries, dictionary };
    public struct CrossreferenceData
    {
        public readonly string folderPath;
        public readonly OriginType originType;
        public List<string>? filesToUse = null;
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
        public List<Link> allLinks;
        public ReferenceLinkLoader(string folderPathFromCommentaries, string folderPathCrossreferences,
                                   List<string>? commentariesList = null, List<string>? crossreferencesList = null)
        {
            this.fromCommentaries = new CrossreferenceData(folderPathFromCommentaries, OriginType.commentaries);
            this.crossreferences = new CrossreferenceData(folderPathCrossreferences, OriginType.crossreferences);
            if (commentariesList != null) { AddCommentariesList(commentariesList); }
            if (crossreferencesList != null) { AddCrossreferencesList(crossreferencesList); }
            allLinks = new List<Link>();
            SetAllLinkList();
        }
        private void AddCommentariesList(List<string> commentariesList)
        {
            fromCommentaries.filesToUse = commentariesList;
            fromCommentaries.all = false;
        }
        private void AddCrossreferencesList(List<string> crossreferencesList)
        {
            crossreferences.filesToUse = crossreferencesList;
            crossreferences.all = false;
        }
        private void SetAllLinkList()
        {
            Dictionary<int, Dictionary<int, List<Link>>> dictionary = GetAllLinksWithoutDuplicates();
            List<Link> links = new List<Link>();

            //Parallel.ForEach(dictionary.Keys, book =>//
            foreach(int book in dictionary.Keys)
            {
                foreach (int hash in dictionary[book].Keys)
                {
                    foreach (Link link in dictionary[book][hash])
                    {

                        links.Add(link);


                    }

                }
            }//);

            allLinks = links;

        }
        public List<Link> FindLinks(List<Reference> requierements, OneTextBoxState state)
        {
            List<Link> links = new List<Link>();


            foreach (Link link in allLinks)
            {
                if (LinkSatisfiesRequirements(link, requierements, state))
                {

                    links.Add(link);

                }
            }

            return links;
        }
        public List<Link> FindLinksTwo(List<Reference> requirementsFrom, List<Reference> requirementsTo)
        {
            List<Link> links = new List<Link>();


            foreach (Link link in allLinks)
            {
                if (LinkSatisfiesRequirementsTWO(link, requirementsFrom, requirementsTo))
                {
                    links.Add(link);
                }
            }

            return links;
        }
        public Dictionary<int, Dictionary<int, int>> GetHistogramData(List<Link> links)
        {
            Dictionary<int, Dictionary<int, int>> data = new Dictionary<int, Dictionary<int, int>>();
            int count = links.Count;
            for (int i = 0; i < count; i++)
            {
                AddToData(links[i].source, data, links[i].Occurance);
                AddToData(links[i].target, data, links[i].Occurance);
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


        public Dictionary<int, Dictionary<int, List<Link>>> GetAllLinksWithoutDuplicates()
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
                    int key = link.GetHashCode();
                    if (noDuplicatesLinks.ContainsKey(link.source.book))
                    {
                        if (noDuplicatesLinks[link.source.book].ContainsKey(key))
                        {
                            bool found = false;
                            foreach (Link linkFromDic in noDuplicatesLinks[link.source.book][key])
                            {
                                if (linkFromDic == link)
                                {
                                    found = true;
                                    int occurance = link.GetOccurance();
                                    linkFromDic.IncreaseOccuranceBy(occurance);
                                    break;
                                }
                            }
                            if (!found)
                            {
                                noDuplicatesLinks[link.source.book][key].Add(link);
                            }
                        }
                        else
                        {
                            noDuplicatesLinks[link.source.book][key] = new List<Link> { link };
                        }
                    }
                    else
                    {
                        noDuplicatesLinks[link.source.book] = new Dictionary<int, List<Link>>();
                        noDuplicatesLinks[link.source.book][key] = new List<Link> { link };
                    }
                }
            }

            return noDuplicatesLinks;
        }
        private List<Link> GetLinksFromAllFiles(CrossreferenceData data)
        {
            string[] allFilePaths = Directory.GetFiles(data.folderPath);
            //HashSet<Link> links = new HashSet<Link>();
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
                            chapterEnd = chapter;
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
                            chapterToEnd = chapterTo;
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
    }
}
