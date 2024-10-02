using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Link 
    {
        public readonly Reference source;
        public readonly Reference? target;
        public int Occurance { get; set; }
        public Link(Reference source, Reference target)
        {
            this.source = source;
            this.target = target;
            Occurance = 1; 
        }
        public Link(Reference source)
        {
            this.source= source;
            this.target = null;
            Occurance = 1;
        }
        public void IncreaseOccuranceBy (int increase) => Occurance += increase;

        public static bool operator == ( Link first, Link another)
        {
            return first.source == another.source && first.target == another.target;
        }
        public static bool operator != (Link first, Link another)
        {
            return !(first == another);
        }
        public override bool Equals(object? obj)
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
            if (target is Reference validTarget)
            {
                foreach (Reference reference in requirements)
                {
                    if (validTarget.FitsInto(reference))
                    {
                        return true;
                    }
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
            if (target is Reference validTarget)
            {
                foreach (Reference reference in requirements)
                {
                    if (source.FitsInto(reference))
                    {
                        sourceFits = true;
                    }
                    if (validTarget.FitsInto(reference))
                    {
                        targetFits = true;
                    }
                }
                return sourceFits && targetFits;
            }
            return sourceFits;
        }
        public bool All(List<Reference> requirements) 
        {
            if (target is Reference validTarget)
            {
                foreach (Reference reference in requirements)
                {
                    if (source.FitsInto(reference) || validTarget.FitsInto(reference))
                    {
                        return true;
                    }
                }
                return false;
            }
            else return From(requirements);
        }
        public class OccuranceComparer : IComparer<Link>
        {
            public int Compare(Link x, Link y) => y.Occurance - x.Occurance;
        }
        public class SourceComparer : IComparer<Link> 
        {
            public int Compare(Link x, Link y) => x.source.CompareTo(y.source);
        }
        public class TargetComparer : IComparer<Link>
        {
            public int Compare(Link x, Link y)
            {
                if(x.target is Reference validXTarget && y.target is Reference validYTarget)
                {
                    return validXTarget.CompareTo(validYTarget);

                }
                return 0;
            }
        }

    }
}
