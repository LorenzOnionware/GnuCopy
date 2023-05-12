using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Project1.Viewmodels;

namespace Project1.Presets;

public partial class Addpresets : ContentDialog, IStyleable
{
    Type IStyleable.StyleKey => typeof(ContentDialog);

    public Addpresets()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        DataContext = AddpresetsViewmodel.Default;
        AvaloniaXamlLoader.Load(this);
    }
}