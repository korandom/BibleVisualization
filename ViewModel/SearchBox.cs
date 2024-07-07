
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Text;
using DataStructures;
using FindLinksForRequirements;

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
        private int _stateIndex;
        public int StateIndex
        {
            get { return _stateIndex; }
            set
            {
                if (_stateIndex == value) return;
                _stateIndex = value;
                OnPropertyChanged(nameof(StateIndex));
            }
        }

        public string currentBible;
        public List<string> bibleNames= new List<string>();

        private Color _inputTextColor1 = Color.Black;
        public Color InputTextColor1
        {
            get { return _inputTextColor1; }
            set
            {
                if (_inputTextColor1 == value) return;
                _inputTextColor1 = value;
                OnPropertyChanged(nameof(InputTextColor1));
            }
        }
        private Color _inputTextColor2 = Color.Black;
        public Color InputTextColor2
        {
            get { return _inputTextColor2; }
            set
            {
                if (_inputTextColor2 == value) return;
                _inputTextColor2 = value;
                OnPropertyChanged(nameof(InputTextColor2));
            }
        }

        public bool addedRequirement = false;

        public SearchBoxRequierement(string path)
        {
            string[] filePaths = Directory.GetFiles(path);
            foreach (string file in filePaths)
            {
                bibleNames.Add(Path.GetFileName(file));
            }
            currentBible = ConfigurationManager.AppSettings.Get("FirstPickBible")??"";
            CurrentBibleIndex = bibleNames.IndexOf(currentBible);

            StateIndex = Int32.TryParse(ConfigurationManager.AppSettings.Get("SearchTypeIndex"), out int result) && result < 4 && result >=0 ? result : 3;
        }


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
