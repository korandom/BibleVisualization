using FindLinksForRequirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using FindReferencesForRequirements;

namespace ViewModel
{
    internal class RequirementBox
    {
        public string Text = "";
        public string RequirementDescription = "";
        InputParser ip = new InputParser();
        BibleText bible;

        public RequirementBox(BibleText bible)
        {
            this.bible = bible;
        }

        public void LoadProperties(string requirement)
        {
            try
            {
                List<Reference> requirementReferences = ip.Parse(requirement);
                StringBuilder text = new StringBuilder();
                foreach (Reference reference in requirementReferences)
                {
                    VerseEnumerator vs = bible.GetEnumerator(reference);
                    while(vs.MoveNext()) 
                    {
                        text.Append(vs.Current());
                    } 
                }
                RequirementDescription = requirement;
                Text = text.ToString();
            }
            catch (FormatException e) { Text = e.Message; }
        }
    }
}
