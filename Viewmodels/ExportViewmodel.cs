using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SharpCompress.Archives;
using SharpCompress.Common;
using ZipArchive = SharpCompress.Archives.Zip.ZipArchive;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class ExportViewmodel
{
    public ExportViewmodel(IFileDialogService FileDialogservice)
    {
        _fileDialogService = FileDialogservice;
    }
    private IFileDialogService _fileDialogService;
    public static ExportViewmodel Default = IOC.Default.GetService<ExportViewmodel>();
    
    public bool enabled => selectedpresets.Any(n => !String.IsNullOrEmpty(n));
    public ObservableCollection<string> presets { get; } = MainViewmodel.Default.Folderitems; 
    public ObservableCollection<string> selectedpresets { get; } = new();
    [ObservableProperty][AlsoNotifyChangeFor(nameof(enabled))] private string selectedpreset;
    [ObservableProperty] private string exportto;

    public string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"GnuCopy");

    [ICommand]
    public void ClosedClick()
    {
        Exportto = String.Empty;
    }

    [ICommand]
    public async Task Lol()
    {
        OnPropertyChanged(nameof(enabled));
    }
    [ICommand]
    public async Task ExportClick()
    {
        string ZipPath = await _fileDialogService.PickFolder();
        var Temp = Directory.CreateTempSubdirectory("temp");
        await Task.Run(() =>
        {
            foreach (var file in ExportViewmodel.Default.selectedpresets)
            {
                var filee = file + ".json";
                File.Copy(Path.Combine(path,filee), Path.Combine(Temp.FullName,filee));
            }
        });
        using (var archive = ZipArchive.Create())
        {
            archive.AddAllFromDirectory(Temp.FullName);
            archive.SaveTo(Path.Combine(ZipPath, "Export.zip"), CompressionType.None);
           
        }
        Directory.Delete(Temp.FullName,true);
        Exportto = String.Empty;
    }

    [ICommand]
    public async Task PickFolder()
    {
       Exportto = await _fileDialogService.PickFolder()+"\\";
       OnPropertyChanged(nameof(Exportto));
    }

}