using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Project1.Presets;
using Path = Avalonia.Controls.Shapes.Path;
using static Project1.IFileDialogService;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class EditPresetsViewmodel
{
    public static EditPresetsViewmodel Default = new();
    private static PresetIndex index => IOC.Default.GetService<GetSetPresetIndex>().getpresetindex();
    
    public ObservableCollection<string> Folder { get; } = new ObservableCollection<string>(index.Folder);
    public ObservableCollection<string> Files { get; } = new ObservableCollection<string>(index.Files);

    [ObservableProperty] private string filetext;
    [ObservableProperty] private string foldertext;

    [ObservableProperty] private string selectedfolder;
    [ObservableProperty] private string selectedfile;
    
    [ObservableProperty]
    private string presetname = IOC.Default.GetService<MainViewmodel>().Selectedlistitem;

    [ICommand]
    private void FileAdd()
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