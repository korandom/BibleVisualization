using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using FindReferencesForRequirements;

namespace ViewModel
{
    internal class LinkBox
    {
        bool visible = false;
        string Text ="";
        string SourceLabel="";
        string TargetLabel="";
        int occurance=0;
        bool next=false;
        bool back = false;
        bool source = true;
        Link link;
        BibleText bible;
        VerseEnumerator verseEnumerator;

        public LinkBox(BibleText bible)
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
                catch (Exception e) { return "book not found"; };
                string chapterS = (r.chapterStart == 0) ? "" : $" {r.chapterStart}";
                string verseS = (r.verseStart==0) ? "" : $":{r.verseStart}";
                string chapterE = (r.chapterEnd==0) ? "" : $"-{r.chapterEnd}";
                string verseE = (r.verseEnd == 0) ? "" : $":{r.verseEnd}";
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
                Verse targetStart = new Verse(link.target.book, link.target.chapterStart, link.target.chapterStart, "");
                SourceLabel = GetStringRepresentation(link.source);
                TargetLabel = GetStringRepresentation(link.target);
                visible = true;
                SetToVerse(verseEnumerator.Current());
                occurance = link.Occurance;
            }
        }
        private void SetToVerse(Verse current)
        {
            Text = $"{current.bookShort} {current.chapter}:{current.verse}  {current.text}";
            next = verseEnumerator.CheckNext();
            back = verseEnumerator.CheckBack();
        }
        public void Next()
        {
            if (verseEnumerator.MoveNext())
            {
                SetToVerse(verseEnumerator.Current());
            }
        }
        public void Back()
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
        
    }
}
