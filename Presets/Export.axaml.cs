using System;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using static Project1.Viewmodels.ExportViewmodel;

namespace Project1.Presets;

public partial class Export : ContentDialog,IStyleable
{
    Type IStyleable.StyleKey => typeof(ContentDialog);
    public Export()
    {
        DataContext = Default;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ToggleButton_OnChecked(object? sender, RoutedEventArgs e)
    {
        Default.selectedpresets.Add(Default.Selectedpreset);
    }

    private void ToggleButton_OnUnchecked(object? sender, RoutedEventArgs e)
    {
        Default.selectedpresets.Remove(Default.Selectedpreset);
    }
}