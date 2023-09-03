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

namespace ViewModel
{
    internal class ModelViewRequirementBase
    {
        string BiblesPath;
        BibleText bt;
        bool initialized;
        int currentLinkIndex=0;
        List<Link> _links;
        LinkBox[] linkBoxes = new LinkBox[5];
        RequirementBox requirementBox;
        SearchBoxRequierement searchBox;
        ReferenceLinkLoader rLinkLoader;

        public ModelViewRequirementBase()
        {
            BiblesPath = ConfigurationManager.AppSettings.Get("DataSourcePath") + "\\BibleTranslations";
            searchBox = new SearchBoxRequierement(BiblesPath);
            bt = new BibleText(BiblesPath + "\\" +searchBox.currentBible);
            _links = new List<Link>();
            for (int i = 0; i < 5; i++)
            {
                linkBoxes[i]=new LinkBox(bt);
            }
            requirementBox = new RequirementBox(bt);
            //todo - if not up to date - preprocess
        }

        //Vizulization
        public void More() { }
        public void Search() 
        {    

        }

    }
}
