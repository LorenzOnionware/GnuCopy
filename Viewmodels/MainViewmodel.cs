using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Project1.Viewmodels;

[ObservableObject]
public partial class MainViewmodel
{
    
    public static MainViewmodel Default = new();
    [ObservableProperty]
    private int progressmax = 100;
    [ObservableProperty]
    private int progressvalue;

    public ObservableCollection<string>? Folderitems { get; set; } = new();
    
    public static bool openwindow = false;
    public static bool openwindow1 = false;


    public static string SelectedListItem; 
    public static string PPresetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy";
    
    public string loadimagepath => (System.Reflection.Assembly.GetEntryAssembly().Location + @"icons\aktualisieren.png");

    public static bool deleted = false;
    
    public ObservableCollection<string> jsonindex { get; set; } = new();
}
