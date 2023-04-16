using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class SettingsViewmodel
{
    public static SettingsViewmodel Default = new();
    public string settingspath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy\Settings\Settings.json";

    [ObservableProperty] private bool overritechecked = IOC.Default.GetService<Settings>().Overrite;
    [ObservableProperty] private bool clearforchecked = IOC.Default.GetService<Settings>().Clearforcopy;
    [ObservableProperty] private bool clearafterchecked = IOC.Default.GetService<Settings>().Clearaftercopy;
    [ObservableProperty] private bool savepaths = IOC.Default.GetService<Settings>().Savelastpaths;
    [ObservableProperty] private int comboboxselectedindex;
    [ObservableProperty] private string selectedcomboboxitem = IOC.Default.GetService<Settings>().Packageformat;

    [ObservableProperty] private string listingart;
    [ObservableProperty] private bool? listingarts = IOC.Default.GetService<Settings>().Listingart;
}