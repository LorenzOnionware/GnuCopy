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

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if(_Blocker)
            return;
        AddpresetsViewmodel.Default.b = false;
        var a =System.IO.Path.GetInvalidFileNameChars();
        foreach (var c in a)
        {
            if(AddpresetsViewmodel.Default.Presetname.Contains(c))
                AddpresetsViewmodel.Default.b = true;
        }

        if (AddpresetsViewmodel.Default.b)
        {
            AddpresetsViewmodel.Default.Labelenable = true;
        }
        else
        {
            AddpresetsViewmodel.Default.Labelenable = false;
        }
    }

    private void ContentDialog_OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        _Blocker = true;
        AddpresetsViewmodel.Default = new AddpresetsViewmodel();
    }
}