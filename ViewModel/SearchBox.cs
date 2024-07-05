
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using DataStructures;

namespace ViewModel
{
    public class SearchBoxRequierement : INotifyPropertyChanged
    {
        private readonly string _helpText = GetHelpText();
        public string HelpText
        {
            get { return _helpText; }
        }

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
        private static string GetHelpText()
        {
            StringBuilder sb = new StringBuilder();
            var section = (BookSection)ConfigurationManager.GetSection("bookSection");
            if (section != null)
            {
                foreach (BookElement element in section.Books)
                {
                    sb.AppendLine($"  {element.ShortName} - {element.LongName}");
                }
            }
            return sb.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
