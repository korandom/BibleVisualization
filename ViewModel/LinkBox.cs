using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using FindReferencesForRequirements;

namespace ViewModel
{
    internal class LinkBox 
    {
        bool source = true;
        VerseEnumerator verseEnumerator;

        public LinkBox( VerseEnumerator verseEnumerator)
        {
            this.verseEnumerator = verseEnumerator;
        }
    }
}
