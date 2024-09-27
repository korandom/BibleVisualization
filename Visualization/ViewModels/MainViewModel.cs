﻿using Avalonia;
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
    private IDiagramBase Diagram { get; set; }

    [ObservableProperty]
    private double bookThickness;

    [ObservableProperty]
    private double linkThickness;

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
        Books = Diagram.GetInitializedBooks();
        Links = Diagram.GetConnectedLinks(links, Books);
    }
    public MainViewModel(List<Link> links, string theme)
    {
        this.theme = theme;
        this.withTheme = true;
        Diagram = new TwoLinesDiagramModel(true);
        BookThickness = ConfigManager.GetConfigProperty("BookThickness");
        LinkThickness = ConfigManager.GetConfigProperty("LinkThickness");
        Books = Diagram.GetInitializedBooks();
        Links = Diagram.GetConnectedLinks(links, Books);
    }

    public void Update(Rect Bounds)
    {
        ThemePoint=new Point((Bounds.Width - ThemeWidth) / 2, Bounds.Height / 4  - ThemeHeight);
        Diagram.UpdateDiagramPosition(Bounds);
        UpdateBookGeometries();
        UpdateLinks();
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
}
