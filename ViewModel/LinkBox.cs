using DataStructures;
using FindReferencesForRequirements;
using System.ComponentModel;

namespace ViewModel
{
    public class LinkBox : INotifyPropertyChanged
    {
        private bool _visible = false;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged(nameof(Visible));
                }
            }
        }

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

        private string _sourceLabel = "label";
        public string SourceLabel
        {
            get { return _sourceLabel; }
            set
            {
                if (_sourceLabel != value)
                {
                    _sourceLabel = value;
                    OnPropertyChanged(nameof(SourceLabel));
                }
            }
        }
        private string _targetLabel = "label";
        public string TargetLabel
        {
            get { return _targetLabel; }
            set
            {
                if (value != _targetLabel)
                {
                    _targetLabel = value;
                    OnPropertyChanged(nameof(TargetLabel));
                }
            }
        }

        private int _occurance = 0;
        public int Occurance
        {
            get { return _occurance; }
            set
            {
                if(_occurance != value)
                {
                    _occurance = value;
                    OnPropertyChanged(nameof(Occurance));
                }

            }
        }

        bool _next = false;
        public bool Next
        {
            get { return _next; }
            set
            {
                if(value != _next)
                {
                    _next = value;
                    OnPropertyChanged(nameof(Next));
                }
            }
        }
        bool _back = false;
        public bool Back
        {
            get { return _back; }
            set
            {
                if (_back != value)
                {
                    _back = value;
                    OnPropertyChanged(nameof(Back));
                }
            }
        }

        bool source = true;
        Link link;
        BibleText bible;
        VerseEnumerator verseEnumerator;

        public LinkBox(BibleText bible)
        {
            this.bible = bible;
        }
        public void ChangeBibleText(BibleText bible)
        {
            this.bible = bible;
        }
        private string GetStringRepresentation(Reference r)
        {
            if (r.book != 0)
            {
                string book;
                try
                {
                    book = NameToBook.bookNumberToName[r.book];
                }
                catch { return "book not found"; };
                string chapterS = (r.chapterStart == 0) ? "" : $" {r.chapterStart}";
                string verseS = (r.verseStart == 0) ? "" : $":{r.verseStart}";
                string chapterE = (r.chapterEnd == 0 || r.chapterEnd == r.chapterStart) ? "" : $"-{r.chapterEnd}";
                string verseE = (r.verseEnd == 0) ? "" : (chapterE=="")? $"-{r.verseEnd}":$":{r.verseEnd}";
                return book + chapterS + verseS + chapterE + verseE;
            }
            return "all";
        }
        public void LoadProperties(Link link)
        {
            this.link = link;
            verseEnumerator = bible.GetEnumerator(link.source);
            if (verseEnumerator.MoveNext())
            {
                SourceLabel = GetStringRepresentation(link.source);
                TargetLabel = GetStringRepresentation(link.target);
                Visible = true;
                SetToVerse(verseEnumerator.Current());
                Occurance = link.Occurance;
            }
            else
            {
                Visible = false;
            }
        }
        private void SetToVerse(Verse current)
        {
            Text = $"{current.bookShort} {current.chapter}:{current.verse}  {current.text}";
            Next = verseEnumerator.CheckNext();
            Back = verseEnumerator.CheckBack();
        }
        public void NextVerse()
        {
            if (verseEnumerator.MoveNext())
            {
                SetToVerse(verseEnumerator.Current());
                
            }
        }
        public void BackVerse()
        {
            if (verseEnumerator.MoveBack())
            {
                SetToVerse(verseEnumerator.Current());
                
            }
        }
        public void ShowSource()
        {
            if (!source)
            {
                source = true;
                verseEnumerator = bible.GetEnumerator(link.source);
                if (verseEnumerator.MoveNext())
                {
                    SetToVerse(verseEnumerator.Current());
                }
            }
        }
        public void ShowTarget()
        {
            if (source)
            {
                source = false;
                verseEnumerator = bible.GetEnumerator(link.target);
                if (verseEnumerator.MoveNext())
                {
                    SetToVerse(verseEnumerator.Current());
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
