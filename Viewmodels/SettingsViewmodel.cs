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

    [ObservableProperty] private bool overritechecked;
    [ObservableProperty] private bool clearforchecked ;
    [ObservableProperty] private bool clearafterchecked;
}