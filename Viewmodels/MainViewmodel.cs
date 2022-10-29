using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
}
