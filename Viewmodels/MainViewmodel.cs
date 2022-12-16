using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class MainViewmodel
{
    
    public static MainViewmodel Default = new();
    [ObservableProperty]
    private int progressmax = 100;
    [ObservableProperty]
    private int progressvalue;

    public static bool openwindow = false;
    public static bool openwindow1 = false;

    [ObservableProperty]
    private string textboxtext;

    [ObservableProperty] 
    private string textbox2text;

    public static string SelectedListItem;
    
    public string loadimagepath => (System.Reflection.Assembly.GetEntryAssembly().Location + @"icons\aktualisieren.png");
}
