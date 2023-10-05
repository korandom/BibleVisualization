using FindLinksForRequirements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class SearchBoxRequierement : INotifyPropertyChanged
    {
        private int _currentBibleIndex;
        public int CurrentBibleIndex
        {
            get { return _currentBibleIndex; }
            set
            {
                if (_currentBibleIndex == value) return;
                _currentBibleIndex = value;
                OnPropertyChanged(nameof(CurrentBibleIndex));
            }
        }

        public string currentBible;
        public List<string> bibleNames= new List<string>();
        public string requirement1="";
        public string requirement2="";
        public int stateIndex=0;
        public bool addedRequirement=false;

        public SearchBoxRequierement(string path)
        {
            string[] filePaths = Directory.GetFiles(path);
            foreach (string file in filePaths)
            {
                bibleNames.Add(Path.GetFileName(file));
            }
            currentBible = ConfigurationManager.AppSettings.Get("FirstPickBible")??"";
            CurrentBibleIndex = bibleNames.IndexOf(currentBible);
        }
        public void AddRequirement() => addedRequirement = true;
        public void RemoveRequirement() => addedRequirement = false;
        public void ChangeStateIndex(int index) => stateIndex = index;
        public void ChangeBible(int index)
        {
            currentBible = bibleNames[index];
            CurrentBibleIndex = index;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
