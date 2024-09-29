using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Visualization.Diagrams;
using Visualization.Util;

namespace Visualization.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private ObservableCollection<Book> books = new ObservableCollection<Book>();
    public ObservableCollection<Book> Books
    {
        get => books;
        set => SetProperty(ref books, value);
    }
    private List<VisualLink> links = new List<VisualLink>();

    public List<VisualLink> Links
    {
        get => links;
        private set
        {
            links = value;
            OnPropertyChanged(nameof(Links));
        }
    }

    private List<HistogramItem> histogram = new List<HistogramItem>();
    public List<HistogramItem> Histogram
    {
        get => histogram;
        private set
        {
            histogram = value;
            OnPropertyChanged(nameof(Histogram));
        }
    }
    private IDiagramBase Diagram { get; set; }

    [ObservableProperty]
    private double bookThickness;

    [ObservableProperty]
    private double linkThickness;

    [ObservableProperty]
    private double histogramStrokeThickness;

    private readonly string theme = "";

    public string Theme => theme;
    [ObservableProperty]
    private bool withTheme = false;
    [ObservableProperty]
    private Point themePoint;
    [ObservableProperty]
    private double themeHeight;
    [ObservableProperty]
    private double themeWidth;

    private readonly HistogramManager histogramManager = new HistogramManager();

    public MainViewModel(List<Link> links, DiagramType type)
    {
        switch (type) {
            case DiagramType.Chord:
                {
                    Diagram = new ChordDiagramModel();
                    break;
                }
            case DiagramType.TwoLines:
                {
                    Diagram = new TwoLinesDiagramModel(false);
                    break;
                }
            default:
                {
                    throw new Exception("Undefined Diagram Type.");
                }
        }

        BookThickness = ConfigManager.GetConfigProperty("BookThickness");
        LinkThickness = ConfigManager.GetConfigProperty("LinkThickness");
        HistogramStrokeThickness = (histogramManager.isHistogramVerses) ? 0.5 : 0;
        Books = Diagram.GetInitializedBooks();
        Links = Diagram.GetConnectedLinks(links, Books, histogramManager.IsHistogramActive);
        Histogram = histogramManager.GetHistogram(Books);
    }
    public MainViewModel(List<Link> links, string theme)
    {
        this.theme = theme;
        this.withTheme = true;
        Diagram = new TwoLinesDiagramModel(true);
        BookThickness = ConfigManager.GetConfigProperty("BookThickness");
        LinkThickness = ConfigManager.GetConfigProperty("LinkThickness");
        HistogramStrokeThickness = (histogramManager.isHistogramVerses) ? 0.5 : 0;
        Books = Diagram.GetInitializedBooks();
        Links = Diagram.GetConnectedLinks(links, Books, histogramManager.IsHistogramActive);
        Histogram = histogramManager.GetHistogram(Books);
    }

    public void Update(Rect Bounds)
    {
        ThemePoint=new Point((Bounds.Width - ThemeWidth) / 2, Bounds.Height / 4  - ThemeHeight);
        Diagram.UpdateDiagramPosition(Bounds);
        UpdateBookGeometries();
        UpdateLinks();
        if(histogramManager.IsHistogramActive)
        {
            UpdateHistogramGeometries();
        }
    }

    private void UpdateLinks()
    {
        foreach (var link in Links)
        {
            link.Geometry = Diagram.GetLinkGeometry(link);
        }
    }

    private void UpdateBookGeometries()
    {
        foreach (var book in Books)
        {
            try
            {
                book.Geometry = Diagram.GetBookGeometry(book);
            }
            catch
            {
                Console.WriteLine("Book mapping not succesful");
            }
        };
    }

    private void UpdateHistogramGeometries()
    {
        foreach(var histogramItem in Histogram)
        {
            histogramItem.Geometry = Diagram.GetHistogramGeometry(histogramItem);
        }
    }
}
