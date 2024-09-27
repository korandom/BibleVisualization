using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualization.Util;

namespace Visualization.Diagrams
{
    public interface IDiagramBase 
    {
        public List<VisualLink> GetConnectedLinks(List<Link> links, ObservableCollection<Book> books);
        public ObservableCollection<Book> GetInitializedBooks();

        public void UpdateDiagramPosition(Rect Bounds);

        public PathGeometry GetLinkGeometry(VisualLink link);

        public PathGeometry GetBookGeometry(Book book);

    }
}
