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
        MainViewModel mainViewModel;
        public VisualizationForm(List<Link> links, DiagramType type )
        {
            InitializeComponent();
            mainViewModel = new MainViewModel( links, type );
            avaloniaHost.Content = new MainView { DataContext = mainViewModel };
        }

        public VisualizationForm(List<Link> links, string theme)
        {
            InitializeComponent();
            mainViewModel = new MainViewModel(links, theme);
            avaloniaHost.Content = new MainView { DataContext = mainViewModel };

        }

        public void ShowBold(Link link)
        {
            mainViewModel.ShowBold(link);
        }
    }
}
