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

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        AddpresetsViewmodel.Default. b = false;
        char[] notallowed = new char[] { '"','<', '>', ':', '/', '\\', '|', '?', '*' };
        foreach (var a in AddpresetsViewmodel.Default.Presetname)
        {
            foreach (var ab in notallowed)
            {
                if (ab == a)
                {
                    AddpresetsViewmodel.Default. b = true;
                }
            }
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
}