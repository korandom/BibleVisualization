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

        private void InitialLinkSetUP(Link link)
        {
            SourceLabel = link.source.ToString();
            TargetLabel = "";
            if(link.target != null)
            {
                TargetLabel = ((Reference)link.target).ToString();
            } 
            if (source)
            {
                verseEnumerator = bible.GetEnumerator(link.source);
            }
            else if (link.target is Reference validTarget)
            {
                verseEnumerator = bible.GetEnumerator(validTarget);
            }
            Occurance = link.Occurance;
        }
        public void LoadProperties(Link link)
        {
            this.link = link;

            InitialLinkSetUP(link);

            if (verseEnumerator.MoveNext())
            {
                Visible = true;
                SetToVerse(verseEnumerator.Current());
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
            if (!source && link is not null)
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
            if (source && link is not null && link.target is Reference validTarget)
            {
                source = false;
                verseEnumerator = bible.GetEnumerator(validTarget);
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
