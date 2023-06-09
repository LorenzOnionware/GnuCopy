using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using FluentAvalonia.UI.Controls;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1;

public partial class SettingsControl : ContentDialog, IStyleable
{
    private Settings settings = IOC.Default.GetService<Settings>();
    Type IStyleable.StyleKey => typeof(ContentDialog);
    
    public SettingsControl()
    {
        DataContext = SettingsViewmodel.Default;
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void ContentDialog_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
       string path = Path.Combine(SettingsViewmodel.Default.settingspath);
       settings.Clearaftercopy = SettingsViewmodel.Default.Clearafterchecked;
       settings.Clearforcopy = SettingsViewmodel.Default.Clearforchecked;
       settings.Savelastpaths = SettingsViewmodel.Default.Savepaths;
       settings.Packageformat = SettingsViewmodel.Default.Comboboxselectedindex;
       settings.Overrite = SettingsViewmodel.Default.Overritechecked;
       settings.DateAsName = SettingsViewmodel.Default.Dateasname;
       settings.ZipName = SettingsViewmodel.Default.Zipname;
       settings.TempfolderPath = SettingsViewmodel.Default.Costumepath;
       settings.MultipleSources = SettingsViewmodel.Default.Multiplesources;
       settings.CreateOwnFolder = SettingsViewmodel.Default.Createfolder;
       settings.OwnFolderName = SettingsViewmodel.Default.Createfoldername;
       settings.OwnFolderDate = SettingsViewmodel.Default.Ownfolderdate;
       string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
       File.WriteAllText(path,ab);
    }

    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        uri(false);
    }

    private void InputElement_OnTapped2(object? sender, TappedEventArgs e)
    {
        uri(true);
    }

    private async void uri(bool What)
    {
        if (What)
        {
            await Task.Run(() => Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/LorenzOnionware",
                UseShellExecute = true
            }));
        }
        else
        {
            await Task.Run(() => Process.Start(new ProcessStartInfo
            {
                FileName = "mailto:contact.onionware@gmail.com",
                UseShellExecute = true
            }));
        }
    }
}
