using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using DataStructures;
using FindLinksForRequirements;
using FindReferencesForRequirements;
using System.ComponentModel;
using Preprocessing;
using System.Security;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    
    public enum SortingWay {target, source, occurance}
    public class ModelViewRequirementBase :INotifyPropertyChanged
    {
        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value) return;
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        private SortingWay _sortingWay;
        public SortingWay SortingWay
        {
            get { return _sortingWay; }
            set {
                if (_sortingWay == value) return;
                _sortingWay = value;
                SortLinks();
                }
        }
        private bool sources;
        private bool _more;
        public bool More 
        {
            get { return _more; } 
            set {
                if (_more!= value)
                {
                    _more = value;
                    OnPropertyChanged(nameof(More));
                }
            }
        }
        private bool _previous;
        public bool Previous
        {
            get { return _previous; }
            set
            {
                if (_previous!= value)
                {
                    _previous = value;
                    OnPropertyChanged(nameof(Previous));
                }
            }
        }
        string BiblesPath;
        BibleText bt;

        int currentLinkIndex=0;
        List<Link> _links;
        public static int numberOfLinkBoxes = 7;
        public LinkBox[] linkBoxes = new LinkBox[numberOfLinkBoxes];
        public RequirementBox requirementBox;
        public SearchBoxRequierement searchBox;
        ReferenceLinkLoader rLinkLoader;
        InputParser parser = new InputParser();  
        public ModelViewRequirementBase()
        {
            string? sourcesPath = ConfigurationManager.AppSettings["DataSourcePath"];
            BiblesPath = sourcesPath + "\\BibleTranslations";
            searchBox = new SearchBoxRequierement(BiblesPath);
            bt = new BibleText(BiblesPath + "\\" + searchBox.currentBible);

            _links = new List<Link>();
            for (int i = 0; i < numberOfLinkBoxes; i++)
            {
                linkBoxes[i]=new LinkBox(bt);
            }
            requirementBox = new RequirementBox(bt);

            rLinkLoader = GetLoader(sourcesPath);
            More = false;
            Previous = false;
            sources = true;
            SortingWay = SortingWay.occurance;
            Preprocess(sourcesPath);
        }
        private void Preprocess(string sourcepath)
        {
            string? preprocess = ConfigurationManager.AppSettings.Get("PreProcessingNeeded");
            if(preprocess != null && preprocess =="true") 
            {
                Preprocessor p = new Preprocessor(sourcepath + "\\ToPreprocess", sourcepath + "\\Preprocessed");
                string? list = ConfigurationManager.AppSettings.Get("DataToPreprocessList");
                if (list == null) return;
                if (list == "")
                {
                    p.ProcessAll();
                }
                else
                {
                    List<string> dataFiles = GetList(list);
                    foreach(string file in dataFiles)
                    {
                        p.Process(file);
                    }
                }

            }
        }
        private ReferenceLinkLoader GetLoader(string sourcePath)
        {
            string? selectedCommentaries = ConfigurationManager.AppSettings.Get("CommentariesToUse");
            List<string>? comToUse = (selectedCommentaries=="") ? null : GetList (selectedCommentaries);
            string? selectedCrossreferences = ConfigurationManager.AppSettings.Get("CrossreferenceToUse");
            List<string>? crossToUse = (selectedCrossreferences == "") ? null : GetList(selectedCrossreferences);
            return new ReferenceLinkLoader(sourcePath + "\\Preprocessed", sourcePath + "\\CrossReferences", comToUse, crossToUse);
        }
        private List<string>? GetList(string? text)
        {
            return (text != null) ? text.Split(";").ToList() : null;
        }
        
        public void LoadMore()  //Loading Next Links in _links to show in boxes in UI
        {
            
            if(Count> currentLinkIndex + numberOfLinkBoxes)
            {
                currentLinkIndex += numberOfLinkBoxes;
                RefreshLinkBoxes();
            }
        }
        public void LoadPrevious()
        {
            if(currentLinkIndex >= numberOfLinkBoxes) 
            {
                currentLinkIndex -= numberOfLinkBoxes;
                RefreshLinkBoxes();
            }
            
        }
        public void RefreshLinkBoxes()
        {
            
            int numberOfLinksLeft = (Count - currentLinkIndex > numberOfLinkBoxes) ? numberOfLinkBoxes : Count - currentLinkIndex;

            for (int i = 0; i < numberOfLinksLeft; i++)
            {
                linkBoxes[i].LoadProperties(_links[currentLinkIndex + i]);
            }
            for (int i = numberOfLinksLeft; i < numberOfLinkBoxes; i++)
            {
                linkBoxes[i].Visible = false;
            }
            Previous = currentLinkIndex - numberOfLinkBoxes >= 0;
            More = Count > currentLinkIndex + numberOfLinkBoxes;
        }
        private List<Reference> ParseAndValidateInput(string input, out Color resultingColor)
        {
            try
            {
                List<Reference> references = parser.Parse(input);
                resultingColor = Color.Black;
                return references;
            }
            catch(FormatException ex)
            {
                resultingColor = Color.Red;
                Clean(ex.Message);
                return new List<Reference>();
            }
        }
        public void Search(string req1, string req2)
        {
            currentLinkIndex = 0;

            // parse and validate input for first value
            Color result;
            List<Reference> first = ParseAndValidateInput(req1, out result);
            searchBox.InputTextColor1 = result;
            if (result == Color.Red) return;

            List<Reference> second = new List<Reference>();
            // if there is two requirements, first parse and validate and then load links
            if (searchBox.addedRequirement)
            {
                Color resultSecond;
                second = ParseAndValidateInput(req2, out resultSecond);
                searchBox.InputTextColor2 = resultSecond;
                if (resultSecond == Color.Red) return;

                LoadLinksTwo(first, second);
            }

            else LoadLinksOne(first);

            SortLinks();
            requirementBox.RequirementDescription = searchBox.addedRequirement ? req1 + req2 : req1;
            Count = _links.Count;
            RefreshLinkBoxes();
            

            if (Count > 0)
            {
                if(searchBox.addedRequirement) first.AddRange(second);
                requirementBox.LoadProperties(first);
            }
            else requirementBox.Text = "No links found";

            
        }
        private void Clean( string message)
        {
            _links = new List<Link>();
            Count = 0;
            requirementBox.Text = message;
            requirementBox.RequirementDescription = "";
            for (int i = 0; i < numberOfLinkBoxes; i++)
            {
                linkBoxes[i].Visible = false;
            }
        }
        private void LoadLinksOne(List<Reference> references)
        {
            _links = rLinkLoader.FindLinks(references, (OneTextBoxState)searchBox.StateIndex);
        }
        private void LoadLinksTwo(List<Reference> from, List<Reference> to)
        {
                _links = rLinkLoader.FindLinksTwo(from, to);
            }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Reset()
        {
            bt = new BibleText(BiblesPath + "\\" + searchBox.currentBible);
            _links = new List<Link>();
            for (int i = 0; i < numberOfLinkBoxes; i++)
            {
                linkBoxes[i].ChangeBibleText(bt);
            }
            requirementBox.ChangeBible(bt);
        }
        public void ShowSources()
        {
            if (!sources)
            {
                sources = true;
                foreach (LinkBox linkbox in linkBoxes)
                {
                    linkbox.ShowSource();
                }
            }
        }
        public void ShowTargets()
        {
            if (sources)
            {
                sources = false;
                foreach(LinkBox linkbox in linkBoxes)
                {
                    linkbox.ShowTarget();
                }
            }
        }
        public void ChangeSortingWay(int index)
        {
            SortingWay = (SortingWay)index;
            currentLinkIndex = 0;
            RefreshLinkBoxes();
        }
        public void SortLinks()
        {
            switch(SortingWay)
            {
                case SortingWay.occurance:
                    {
                        _links.Sort(new Link.OccuranceComparer());
                        break;
                    }
                case SortingWay.source:
                    {
                        _links.Sort(new Link.SourceComparer());
                        break;
                    }
                case SortingWay.target:
                    {
                        _links.Sort(new Link.TargetComparer());
                        break;
                    }
                default: { break; }

            }
        }

    }
}
