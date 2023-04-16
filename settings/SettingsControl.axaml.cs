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
        switch (IOC.Default.GetService<Settings>().Listingart)
        {
            case false:
                SettingsViewmodel.Default.Listingart = "Black List";
                break;
            case true:
                SettingsViewmodel.Default.Listingart = "White List";
                break;
            default:
                SettingsViewmodel.Default.Listingart = "Copy All Content";
                break;
        }
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void ContentDialog_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
       IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
       string path = Path.Combine(SettingsViewmodel.Default.settingspath);
       string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
       File.WriteAllText(path,ab);
    }
}
