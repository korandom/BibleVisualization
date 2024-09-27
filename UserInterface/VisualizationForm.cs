using Avalonia.Win32.Interoperability;
using DataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Visualization.Util;
using Visualization.ViewModels;
using Visualization.Views;

namespace UserInterface
{
    public partial class VisualizationForm : Form
    {
        public VisualizationForm(List<Link> links, DiagramType type )
        {
            InitializeComponent();
            avaloniaHost.Content = new MainView { DataContext = new MainViewModel(links, type) };
        }

        public VisualizationForm(List<Link> links, string theme)
        {
            InitializeComponent();
            avaloniaHost.Content = new MainView { DataContext = new MainViewModel(links, theme) };

        }
    }
}
