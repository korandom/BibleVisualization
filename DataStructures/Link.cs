using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    struct Colour
    {
        int opacity;
        int red;
        int green;
        int blue;

        public Colour()
        {

        }
    }
    public class Link 
    {
        public readonly Reference source;
        public readonly Reference target;
        public int Occurance { get; set; }
        public Link(Reference source, Reference target)
        {
            this.source = source;
            this.target = target;
            Occurance = 1; 
        }
        public void IncreaseOccurance()=>Occurance++;
        public void IncreaseOccuranceBy (int increase) => Occurance += increase;
        public int GetOccurance() => Occurance;

        public static bool operator == ( Link first, Link another)
        {
            return first.source == another.source && first.target == another.target;
        }
        public static bool operator != (Link first, Link another)
        {
            return !(first == another);
        }
        public override bool Equals(object obj)
        {
            return obj is Link  && Equals((Link)obj);
        }
        public bool Equals(Link other)
        {
            return source == other.source && target == other.target;
        }

        public override int GetHashCode()
        {
            int result = source.GetHashCode() ^  target.GetHashCode();
            return result;
        }

        public bool To(List<Reference> requirements)
        {
            foreach(Reference reference in requirements)
            {
                if (target.FitsInto(reference))
                {
                    return true;
                }
            }
            return false;
        }
        public bool From(List<Reference> requirements) 
        {
            foreach (Reference reference in requirements)
            {
                if (source.FitsInto(reference))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Inside(List<Reference> requirements)
        {
            bool sourceFits = false;
            bool targetFits = false;
            foreach (Reference reference in requirements)
            {
                if (source.FitsInto(reference))
                {
                    sourceFits = true;
                }
                if (target.FitsInto(reference))
                {
                    targetFits = true;
                }
            }
            return sourceFits && targetFits;
        }
        public bool All(List<Reference> requirements) 
        {
            foreach (Reference reference in requirements)
            {
                if (source.FitsInto(reference)||target.FitsInto(reference))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
