using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class Presetvievmodel
{
    public static Presetvievmodel Default = new();
    public ObservableCollection<string> Dataformatlist { get; set; } = new();
    public ObservableCollection<string> Folderlist { get; set; } = new();
    public bool editing = false;

    [ObservableProperty]
    private string textboxtext;

    [ObservableProperty] public string dataformatstext;

    [ObservableProperty] public string folderstext;

    [ObservableProperty] 
    private string textbox2text;

    [ObservableProperty] private int folderitemlistindex = -1;
    [ObservableProperty] private int dataformatlistIndex = -1;
}