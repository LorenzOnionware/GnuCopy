using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
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
       settings.Listingart = SettingsViewmodel.Default.Listingarts;
       settings.Savelastpaths = SettingsViewmodel.Default.Savepaths;
       settings.Packageformat = SettingsViewmodel.Default.Comboboxselectedindex;
       settings.Overrite = SettingsViewmodel.Default.Overritechecked;
       string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
       File.WriteAllText(path,ab);
    }
}
