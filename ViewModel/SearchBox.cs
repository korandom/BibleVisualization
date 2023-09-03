using FindLinksForRequirements;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    internal class SearchBoxRequierement
    {
        public string currentBible;
        List<string> bibleNames= new List<string>();
        string requirement1="";
        string requirement2="";
        int stateIndex=0;
        bool addedRequirement=false;

        public SearchBoxRequierement(string path)
        {
            string[] filePaths = Directory.GetFiles(path);
            foreach (string file in filePaths)
            {
                bibleNames.Add(Path.GetFileName(file));
            }
            currentBible = ConfigurationManager.AppSettings.Get("FirstPickBible")??"";
        }
        public void AddRequirement() => addedRequirement = true;
        public void RemoveRequirement() => addedRequirement = false;
        public void ChangeStateIndex(int index) => stateIndex = index;
        public void ChangeBible(string bible) => currentBible = bible;
    }
}
