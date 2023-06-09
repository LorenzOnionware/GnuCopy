using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Avalonia;
using MessageBox.Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Notifications;
using Avalonia.Generics.Controls;
using Avalonia.Threading;
using Avalonia.Generics.Extensions;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Views;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Project1.Presets;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;
using Path = Avalonia.Controls.Shapes.Path;
using static Project1.IFileDialogService;
using MessageBox = Avalonia.Generics.Dialogs.MessageBox;
using ZipArchive = SharpCompress.Archives.Zip.ZipArchive;


namespace Project1.Viewmodels;

[ObservableObject]
public partial class MainViewmodel
{
    
    
    public MainViewmodel(IFileDialogService FileDialogservice)
    {
        _fileDialogService = FileDialogservice;
    }

    private IFileDialogService _fileDialogService;

    public bool canedit => !String.IsNullOrEmpty(Selectedlistitem);
    public ObservableCollection<string>? Folderitems { get; set; } = new();
    public static MainViewmodel Default = IOC.Default.GetService<MainViewmodel>();

    public void ChageProperties()
    {
        OnPropertyChanged(nameof(Isnotempty));
    }
    public bool Presetlistenable
    {
        get => IOC.Default.GetService<Settings>().Listingart == null ||
               IOC.Default.GetService<Settings>().Listingart == true;
    }

    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copytotext;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable))] private string copyfromtext;
    [ObservableProperty] private bool isvisable;   
    [ObservableProperty][AlsoNotifyChangeFor(nameof(canedit))] private int selectedpreset;
    [ObservableProperty]public List<string> ignorefiles =new();
    [ObservableProperty]public List<string> ignorefolder =new();
    [ObservableProperty] private bool? listing = IOC.Default.GetService<Settings>().Listingart;
    [ObservableProperty] private string selectedmultifolder;
    public bool Ismultiplevisable => IOC.Default.GetService<Settings>().MultipleSources;


    public ObservableCollection<string> Expanderpaths { get; set; } = new(); 
    public string Headertext => Expanderpaths?.Any()==true?Expanderpaths[0]:"Paths";
    public bool Isnotempty => Folderitems.Count != 0;
    [ObservableProperty] public int progress;
    [ObservableProperty] public int progressmax;
    private PresetIndex? presetindex => IOC.Default.GetService<GetSetPresetIndex>().getpresetindex();

    [ICommand]
    public void AddPath()
    {
        if (!Expanderpaths.Contains(Copyfromtext) && Directory.Exists(Copyfromtext))
        {
              Expanderpaths.Add(Copyfromtext);
        }
        OnPropertyChanged(nameof(Headertext));
    }

    [ICommand]
    private void MultiListRemove()
    {
        Expanderpaths.Remove(Selectedmultifolder);
        OnPropertyChanged(nameof(Headertext));
        IOC.Default.GetService<Settings>().Sources = Expanderpaths.ToList();
        string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
        File.WriteAllText(System.IO.Path.Combine(SettingsViewmodel.Default.settingspath), ab);
        
    }
    
    [ICommand]
    public async void ListingPropertyChanged()
    {
        Task.Run(() =>
        {
            Task.Delay(50);
            IOC.Default.GetService<Settings>().Listingart = Listing;
            IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
            string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
            File.WriteAllText(System.IO.Path.Combine(SettingsViewmodel.Default.settingspath), ab);
            OnPropertyChanged(nameof(Presetlistenable));
        });
    }

    public bool isenabled2 = true;
    public bool Isenable=> Copyfromtext != "" && Copytotext != "" && isenabled2==true;

    [ObservableProperty] private double opaciprogress = 0.0;
 
    private string selectedlistitem;

    [ObservableProperty] public bool hoverover;
    [ObservableProperty] private double optaci = 0;

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
    public async void AddPreset()
    {
        var window = new Project1.Presets.Addpresets();
        await window.ShowAsync();
        await IOC.Default.GetService<MainWindow>().AddItemsToList();
        OnPropertyChanged(nameof(Folderitems));
        OnPropertyChanged(nameof(Isnotempty));
    }

    [ICommand]
    public async Task Import()
    {
        var a = await _fileDialogService.PickFile();
        var b = a[0];
        var c = Task.Run(() =>
        {
            using (var archive = ZipArchive.Open(b))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(PPresetPath, new ExtractionOptions()
                    {
                        Overwrite = true
                    });
                }
            }
        });

        await c;
        await IOC.Default.GetService<MainWindow>().AddItemsToList();
        OnPropertyChanged(nameof(Folderitems));
        OnPropertyChanged(nameof(Isnotempty));
    }

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
            File.Delete((PPresetPath + @"\" + Selectedlistitem+".json")); 
            Folderitems.Remove(selectedlistitem);
        }

        Selectedpreset = -1;
        OnPropertyChanged(nameof(canedit));
        OnPropertyChanged(nameof(Isnotempty));
        
    }

    [ICommand]
    public async Task Copybutton()
    {
        isenabled2 = false;
        OnPropertyChanged(nameof(Isenable));
        var Pathh = copytotext;
        Optaci = 10;
        OnPropertyChanged(nameof(Optaci));
        if (IOC.Default.GetService<Settings>().Packageformat == 0 && IOC.Default.GetService<Settings>().CreateOwnFolder == true)
        {
            
            var name = IOC.Default.GetService<Settings>().OwnFolderDate ? DateTime.Now.ToString("g").Replace(':','-') : IOC.Default.GetService<Settings>().OwnFolderName;
            Directory.CreateDirectory(System.IO.Path.Combine(Pathh, name));
            copytotext = System.IO.Path.Combine(Pathh, name);
        }
        if (copyfromtext.EndsWith(":"))
        {
            copyfromtext = "Please selct a folder.";
            return;
        }
        if (copyfromtext.EndsWith("\\"))
        {
            string a = copyfromtext.Remove(copyfromtext.Length - 1);
            Copyfromtext = a;
        }

        if (!Copytotext.EndsWith("\\"))
        {
            copytotext += "\\";
        }
        IOC.Default.GetService<Settings>().Pathfrom = copyfromtext;
        IOC.Default.GetService<Settings>().Pathto =  IOC.Default.GetService<Settings>().CreateOwnFolder==true ? Pathh : copytotext;
        await Task.Run( async ()=> await IOC.Default.GetService<StartCopyService>().Start());
        Optaci = 0;
        OnPropertyChanged(nameof(Optaci));

        ContentDialog dlg = new ContentDialog();
        dlg.Title = "Done";
        dlg.Content = "GnuCopy finished operation.";
        dlg.PrimaryButtonText = "Close";
        await dlg.ShowAsync();
        OnPropertyChanged(nameof(Isenable));
        copytotext = Pathh;
        isenabled2 = true;
        OnPropertyChanged(nameof(Isenable));
        var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
        taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
    }
    
    [ICommand]
    private async Task CopySourceDialog()
    {
        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            foreach (var a in await _fileDialogService.PickFolders())
            {
                if (Directory.Exists(a) && !Expanderpaths.Contains(a))
                {
                    Expanderpaths.Add(a);
                }
            }
            OnPropertyChanged(nameof(Headertext));
            IOC.Default.GetService<Settings>().Sources.Replace(Expanderpaths);
            string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
            File.WriteAllText(System.IO.Path.Combine(SettingsViewmodel.Default.settingspath),ab);
            
        }
        else
        {
            Copyfromtext = await _fileDialogService.PickFolder();
            IOC.Default.GetService<Settings>().Pathfrom = copyfromtext;
        }

    }
    
    [ICommand]
    private async Task CopyTargetDialog()
    {
        Copytotext = await _fileDialogService.PickFolder();
        IOC.Default.GetService<Settings>().Pathto = copytotext;
    }
  
    public void selectionchaged()
    {
        OnPropertyChanged(nameof(presetindex));
        OnPropertyChanged(nameof(canedit));
        if (IOC.Default.GetService<Settings>().Listingart != false)
        {
            Ignorefiles = presetindex.Files;
            Ignorefolder = presetindex.Folder;
        }
    }

    [ICommand]
    private async void Settings()
    {
        var window = new Project1.SettingsControl();
        await window.ShowAsync();
        OnPropertyChanged(nameof(Presetlistenable));
        OnPropertyChanged(nameof(Ismultiplevisable));
        if (Ismultiplevisable)
        {
            IOC.Default.GetService<MainWindow>().Addsources();
            
        }
    }

    [ICommand]
    private async void EditPreset()
    {
        EditPresetsViewmodel.Default = new();
        var window = new Project1.Presets.EditPresetsWindow();
        await window.ShowAsync();
        OnPropertyChanged(nameof(Isnotempty));
    }

    [ICommand]
    private async Task Export()
    {
        var window = new Project1.Presets.Export();
        await window.ShowAsync();
    }

    #endregion
}
