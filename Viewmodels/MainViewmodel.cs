using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Microsoft.CodeAnalysis;
using Path = Avalonia.Controls.Shapes.Path;


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
    [ObservableProperty]private static bool presetlistenable = false;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copytotext;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copyfromtext;
    [ObservableProperty] private bool isvisable;
    public bool Isenable => MainViewmodel.Default.Copyfromtext != "" && MainViewmodel.Default.Copytotext != "";

    [ObservableProperty] private double opaciprogress = 0.0;
    [ObservableProperty] private int selectedpreset;
    [ObservableProperty]private string selectedlistitem; 
    public static string PPresetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy";
    public static bool deleted = false;
    public ObservableCollection<string> jsonindex { get; set; } = new();
    
    [ICommand]
    public async Task DeleteSelectedPreset()
    {
        if (string.IsNullOrEmpty(selectedlistitem))
        {
            return;
        }
        ContentDialog dlg = new();
        dlg.Title = "Delete preset";
        dlg.PrimaryButtonText = "Delete";
        dlg.SecondaryButtonText = "Cancel";
        dlg.Content = new TextBlock() { Text = $"Do you want to delete the preset \"{selectedlistitem}\"?" };
        if (await dlg.ShowAsync() is ContentDialogResult.Primary && Folderitems.Contains(selectedlistitem))
        { 
            File.Delete((PPresetPath + @"\" + Selectedlistitem)); 
            Folderitems.Remove(selectedlistitem);
        }
        
    }
}
