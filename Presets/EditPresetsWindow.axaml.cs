using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1.Presets;

public partial class EditPresetsWindow : ContentDialog,IStyleable
{
    
    Type IStyleable.StyleKey => typeof(ContentDialog);
    public EditPresetsWindow()
    {
        DataContext = EditPresetsViewmodel.Default;
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void ContentDialog_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!b)
        {
            PresetIndex preset = new PresetIndex()
            {
                Folder = EditPresetsViewmodel.Default.Folder.ToList(),
                Files = EditPresetsViewmodel.Default.Files.ToList()
            };
            var a = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var ab = JsonConvert.SerializeObject(preset);
            File.WriteAllBytes(Path.Combine(a, "GnuCopy", EditPresetsViewmodel.Default.Presetname), new byte[0]);
            File.WriteAllText(Path.Combine(a, "GnuCopy", EditPresetsViewmodel.Default.Presetname), ab);
            IOC.Default.GetService<MainViewmodel>().selectionchaged();
        }
    }

    
    public bool b = false;
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        b = false;
        char[] notallowed = new char[] { '"','<', '>', ':', '/', '\\', '|', '?', '*' };
        foreach (var a in EditPresetsViewmodel.Default.Presetname)
        {
            foreach (var ab in notallowed)
            {
                if (ab == a)
                {
                    b = true;
                }
            }
        }

        if (b)
        {
            EditPresetsViewmodel.Default.Labelenable = true;
        }
        else
        {
            EditPresetsViewmodel.Default.Labelenable = false;
        }
    }
}