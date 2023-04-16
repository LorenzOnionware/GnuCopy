using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Generics.Extensions;
using Avalonia.Interactivity;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Path = Avalonia.Controls.Shapes.Path;
using static Project1.IFileDialogService;


namespace Project1.Viewmodels;

[ObservableObject]
public partial class MainViewmodel
{
    public MainViewmodel(IFileDialogService FileDialogservice)
    {
        _fileDialogService = FileDialogservice;
    }

    private IFileDialogService _fileDialogService;
 
    [ObservableProperty]
    public int progressmax = 100;
    [ObservableProperty]
    public int progressvalue;

    public List<string> Jsonfile => JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(SettingsViewmodel.Default.settingspath));
    public ObservableCollection<string>? Folderitems { get; set; } = new();
    public static MainViewmodel Default = IOC.Default.GetService<MainViewmodel>();
    public static bool openwindow = false;
    public static bool openwindow1 = false;
    public static bool deleted = false;
    [ObservableProperty]private static bool presetlistenable;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copytotext;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copyfromtext;
    [ObservableProperty] private bool isvisable;   
    [ObservableProperty] private int selectedpreset;
    [ObservableProperty]public List<string> ignorefiles =new();
    [ObservableProperty]public List<string> ignorefolder =new();
    
    public bool Isenable => Copyfromtext != "" && Copytotext != "";

    [ObservableProperty] private double opaciprogress = 0.0;
 
    private string selectedlistitem;

    public string Selectedlistitem
    {
        get => selectedlistitem;
        set
        {
            if (SetProperty(ref selectedlistitem, value)&& value != null)
            {
                selectionchaged();
            }
        }
    }
    public static string PPresetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy";

    #region Commands

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

    [ICommand]
    public async void Copybutton()
    {
        await IOC.Default.GetService<StartCopyService>().Start();
    }

    [ICommand]
    private async Task CopySourceDialog()
    {
        Copyfromtext = await _fileDialogService.PickFolder();
    }
    
    [ICommand]
    private async Task CopyTargetDialog()
    {
        Copytotext = await _fileDialogService.PickFolder();
    }
  
    public void selectionchaged()
    {
        
        Ignorefiles = ReadPresets.Splitt(true, System.IO.Path.Combine(PPresetPath,Selectedlistitem)); 
        Ignorefolder = ReadPresets.Splitt(false, System.IO.Path.Combine(PPresetPath,Selectedlistitem));
    }

    [ICommand]
    private async void Settings()
    {
        var window = new Project1.SettingsControl();
        await window.ShowAsync();
    }
     #endregion
}
