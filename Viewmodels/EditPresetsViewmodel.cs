using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Project1.Presets;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class EditPresetsViewmodel
{
    public static EditPresetsViewmodel Default = new();
    private static PresetIndex index => IOC.Default.GetService<GetSetPresetIndex>().getpresetindex();
    
    public ObservableCollection<string> Folder { get; } = new ObservableCollection<string>(index.Folder);
    public ObservableCollection<string> Files { get; } = new ObservableCollection<string>(index.Files);
    [ObservableProperty] public bool labelenable = false;

    [ObservableProperty] private string filetext;
    [ObservableProperty] private string foldertext;

    [ObservableProperty] private string selectedfolder;
    [ObservableProperty] private string selectedfile;
    [ObservableProperty] private bool focusfolder;
    [ObservableProperty] private bool focusfiles;
    
    [ObservableProperty]
    private string presetname = IOC.Default.GetService<MainViewmodel>().Selectedlistitem;

    [ICommand]
    private async void FileAdd()
    {
        if(String.IsNullOrEmpty(Filetext))
            return;
        Regex regex = new Regex(@"^[^\.]");
        if (regex.IsMatch(filetext))
        {
            var output = "." + filetext; 
            Files.Add(output);
        }
        else
        {
            Files.Add(filetext);
        }
        Filetext = String.Empty;
        OnPropertyChanged(nameof(filetext));
    }

    
    [ICommand]
    private void CallAll()
    {
        FolderAdd();
        FileAdd();
    }
    
    [ICommand]
    private void FolderAdd()
    {
        if(String.IsNullOrEmpty(Foldertext))
            return;
        Folder.Add(foldertext);
        Foldertext=String.Empty;
        OnPropertyChanged(nameof(Foldertext));

    }
    
    [ICommand]
    private void Remove()
    {
        Files.Remove(selectedfile);
    }
    
    [ICommand]
    private void Remove2()
    {
        Folder.Remove(selectedfolder);
    }
}