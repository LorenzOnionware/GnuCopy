using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class AddpresetsViewmodel
{
    public static AddpresetsViewmodel Default = new();

    public ObservableCollection<string> Folder { get; } = new();
    public ObservableCollection<string> Files { get; } = new();

    [ObservableProperty] private string filetext;
    [ObservableProperty] private string foldertext;
    public bool primary => !Labelenable;

    [ObservableProperty] private string selectedfolder;
    [ObservableProperty] private string selectedfile;

    [ObservableProperty] private string presetname;

    [ICommand]
    private void FileAdd()
    {
        if (String.IsNullOrEmpty(Filetext))
            return;
        Regex regex = new Regex(@"^[^\.]");
        if (regex.IsMatch(filetext))
        {
            var output = "." + filetext; 
            if (!Files.Contains(output))
            { 
                Files.Add(output);
            }
        }
        else
        {
            if (!Files.Contains("."+Filetext))
            {
                Files.Add(filetext);
            }
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
        if (!Folder.Contains(Foldertext))
        {
                    Folder.Add(foldertext);
        }
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

    //Save
    [ICommand]
    public void PrimaryButtonClick()
    {
        if (!b)
        {
            if (String.IsNullOrEmpty(AddpresetsViewmodel.Default.presetname))
                AddpresetsViewmodel.Default.presetname = "Please write presetname";
            else
            {
                PresetIndex preset = new PresetIndex
                {
                    Folder = AddpresetsViewmodel.Default.Folder.ToList(),
                    Files = AddpresetsViewmodel.Default.Files.ToList()
                };
                var index = JsonConvert.SerializeObject(preset);
                File.WriteAllText(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuCopy",
                        AddpresetsViewmodel.Default.presetname + ".json"), index);
            }

            Folder.Clear();
            Files.Clear();
            Presetname = String.Empty;
        }

    }

    //Cancel
    [ICommand]
    public void SecondaryButtonClick()
    {
        Folder.Clear();
        Files.Clear();
        Presetname = String.Empty;
    }


    [ObservableProperty][AlsoNotifyChangeFor(nameof(primary))]private bool labelenable = false;
    public bool b = false;
    
}