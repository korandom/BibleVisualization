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

namespace ViewModel
{
    public class ModelViewRequirementBase :INotifyPropertyChanged
    {
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
            bt = new BibleText(BiblesPath + "\\" +searchBox.currentBible);

            _links = new List<Link>();
            for (int i = 0; i < numberOfLinkBoxes; i++)
            {
                linkBoxes[i]=new LinkBox(bt);
            }
            requirementBox = new RequirementBox(bt);

            rLinkLoader = GetLoader(sourcesPath);
            More = false;
            Previous = false;
            
            //TODO - if not up to date - preprocess
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
            int count = _links.Count;
            if(count> currentLinkIndex + numberOfLinkBoxes)
            {
                currentLinkIndex += numberOfLinkBoxes;
                int nextlinksLeft = (count - currentLinkIndex > numberOfLinkBoxes) ? numberOfLinkBoxes : count - currentLinkIndex;
                for(int i = 0; i < nextlinksLeft; i++)
                {
                    linkBoxes[i].LoadProperties(_links[currentLinkIndex + i]);
                }
                for(int i = nextlinksLeft; i < numberOfLinkBoxes; i++) 
                { 
                    linkBoxes[i].Visible = false; 
                }
            }
            Previous = currentLinkIndex - numberOfLinkBoxes >= 0;
            More = count > currentLinkIndex + numberOfLinkBoxes;
        }
        public void LoadPrevious()
        {
            if(currentLinkIndex >= numberOfLinkBoxes) 
            {
                currentLinkIndex -= numberOfLinkBoxes;
                for (int i = 0; i < numberOfLinkBoxes; i++)
                {
                    linkBoxes[i].LoadProperties(_links[currentLinkIndex + i]);
                }
                
            }
            Previous = currentLinkIndex - numberOfLinkBoxes >= 0;
            More = _links.Count > currentLinkIndex + numberOfLinkBoxes;
        }
        public void Search()
        {
            try
            {
                if (searchBox.addedRequirement)
                    LoadLinksTwo();
                else LoadLinksOne();
                int count = _links.Count;
                if (count> 0)
                {
                    requirementBox.LoadProperties($"{searchBox.requirement1};{searchBox.requirement2}");
                    int numberOfLinkLeft = (count  > numberOfLinkBoxes) ? numberOfLinkBoxes: count;
                    for (int i = 0; i < numberOfLinkLeft; i++)
                    {
                        linkBoxes[i].LoadProperties(_links[i]);
                    }
                }
                else requirementBox.Text = "No links found";
                Previous = false;
                More = count > numberOfLinkBoxes;
            }
            catch (FormatException e) { requirementBox.Text = e.Message; }
        }
        private void LoadLinksOne()
        {
            List < Reference > references= parser.Parse(searchBox.requirement1);
            _links = rLinkLoader.FindLinks(references, (OneTextBoxState)searchBox.stateIndex);
        }
        private void LoadLinksTwo()
        {
            
                List<Reference> references1 = parser.Parse(searchBox.requirement1);
                List<Reference> references2 = parser.Parse(searchBox.requirement2);
                _links = rLinkLoader.FindLinksTwo(references1, references2);
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

    }
}
