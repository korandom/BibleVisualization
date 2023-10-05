using FindLinksForRequirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using FindReferencesForRequirements;
using System.ComponentModel;

namespace ViewModel
{
    public class RequirementBox : INotifyPropertyChanged
    {
        private string _text = "";
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }
        private string _requirementDescription = "";
        public string RequirementDescription
        {
            get { return _requirementDescription; }
            set
            {
                if(_requirementDescription != value)
                {
                    _requirementDescription = value;
                    OnPropertyChanged(nameof(RequirementDescription));
                }
            }
        }
        InputParser ip = new InputParser();
        BibleText bible;

        public RequirementBox(BibleText bible)
        {
            this.bible = bible;
        }
        public void ChangeBible(BibleText bible)
        {
            this.bible = bible;
        }
        public void LoadProperties(string requirement)
        {
                List<Reference> requirementReferences = ip.Parse(requirement);
                StringBuilder text = new StringBuilder();
                foreach (Reference reference in requirementReferences)
                {
                    VerseEnumerator vs = bible.GetEnumerator(reference);
                    while(vs.MoveNext()) 
                    {
                        Verse current = vs.Current();
                        text.Append($"{current.bookShort} {current.chapter}:{current.verse}  {current.text}" + Environment.NewLine + Environment.NewLine);
                    } 
                }
                RequirementDescription = requirement;
                Text = text.ToString();
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
