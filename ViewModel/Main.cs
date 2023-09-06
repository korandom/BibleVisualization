using DataStructures;
using FindLinksForRequirements;
namespace ViewModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Link link = new Link(new Reference(10, 2, 1, 2, 25), new Reference(10, 2, 1, 0, 3));
            Reference r = new Reference(10, 2, 1);
            Console.WriteLine(link.source.FitsInto(r) && link.target.FitsInto(r));
        }
    }
}