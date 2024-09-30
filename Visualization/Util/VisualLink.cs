using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualization.Util
{
    public partial class VisualLink: ObservableObject
    {
        public double startPosition { get; }
        public double endPosition { get; }

        [ObservableProperty]
        private IBrush colorBrush;

        [ObservableProperty]
        private PathGeometry geometry;

        private Link link;

        public string Information => $"S: {link.source}{(link.target != null ? $"\nT: {((Reference)link.target)}" : "")}";
        public VisualLink(double startPosition, double endPosition, Link link, IBrush color)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.colorBrush = color;
            this.link = link;
        }
    }
}
