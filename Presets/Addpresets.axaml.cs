using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Project1.Viewmodels;

namespace Project1.Presets;

public partial class Addpresets : ContentDialog, IStyleable
{
    Type IStyleable.StyleKey => typeof(ContentDialog);
    private bool _Blocker = false;

    public Addpresets()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        DataContext = AddpresetsViewmodel.Default;
        AvaloniaXamlLoader.Load(this);
    }
    

    private void ContentDialog_OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        _Blocker = true;
        AddpresetsViewmodel.Default = new AddpresetsViewmodel();
    }
}