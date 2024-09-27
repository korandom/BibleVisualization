using Avalonia.Controls;
using System;
using Visualization.Diagrams;
using Visualization.ViewModels;

namespace Visualization.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.SizeChanged += Diagram_SizeChanged;
    }

    private void Diagram_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Update(Bounds);
        }
    }
    private void OnThemeTextLayoutUpdated(object? sender, EventArgs e)
    {
        if (sender is Border themeBlock && DataContext is MainViewModel viewModel)
        {
            viewModel.ThemeWidth = themeBlock.Bounds.Width;
            viewModel.ThemeHeight = themeBlock.Bounds.Height;
        }
    }
}
