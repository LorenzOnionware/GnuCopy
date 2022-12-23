using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class Presetvievmodel
{
    public static Presetvievmodel Default = new();
    public ObservableCollection<string> Dataformatlist { get; set; } = new();
    public ObservableCollection<string> Folderlist { get; set; } = new();

    [ObservableProperty]
    private string textboxtext;

    public string dataformatstext { get; set;}
    public string folderstext { get; set;}

    [ObservableProperty] 
    private string textbox2text;
}