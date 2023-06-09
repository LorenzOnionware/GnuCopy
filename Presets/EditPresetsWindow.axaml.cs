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
        PresetIndex preset = new PresetIndex()
            {
                Folder = EditPresetsViewmodel.Default.Folder.ToList(),
                Files = EditPresetsViewmodel.Default.Files.ToList()
            };
            var a = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var ab = JsonConvert.SerializeObject(preset);
            string name = (MainViewmodel.Default.Selectedlistitem + ".json");
            File.Delete(Path.Combine(a,"GnuCopy",name));
            string s = EditPresetsViewmodel.Default.Presetname + ".json";
            File.WriteAllText(Path.Combine(a, "GnuCopy", s), ab);
            IOC.Default.GetService<MainViewmodel>().selectionchaged();
    }
}