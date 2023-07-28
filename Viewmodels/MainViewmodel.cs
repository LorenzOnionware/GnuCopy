using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Newtonsoft.Json;
using Project1.Presets;
using SharpCompress.Archives;
using SharpCompress.Common;
using ZipArchive = SharpCompress.Archives.Zip.ZipArchive;
using static Project1.USBMonitor;
using static Project1.USBEventArgs;


namespace Project1.Viewmodels;

[ObservableObject]
public partial class MainViewmodel
{
    
    
    public MainViewmodel(IFileDialogService FileDialogservice)
    {
        IOC.Default.GetService<WindowClosingService>().Closed += (o, e) => cancelall();
        _fileDialogService = FileDialogservice;

        USBMonitor usbMonitor = new USBMonitor();

        usbMonitor.USBInserted += UsbInsertedHandler;
        usbMonitor.USBRemoved += UsbRemovedHandler;

        usbMonitor.StartMonitoring();
    }

    private IFileDialogService _fileDialogService;
    
    private static void UsbInsertedHandler(object sender, USBEventArgs e)
    {
        
    }

    private void UsbRemovedHandler(object sender, USBEventArgs e)
    {
        if (!Path.Exists(copytotext) || !Path.Exists(copyfromtext))
        {
            cancel.Dispose();
            Optaci = 0;
            OnPropertyChanged(nameof(Optaci));
            OnPropertyChanged(nameof(Isenable2));
            var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
        }
    
    }
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

    private bool isenable =true;
    public  bool Isenable2
    {
        get => isenable ? IOC.Default.GetService<Settings>().MultipleSources ? Expanderpaths.Any()?true:false : String.IsNullOrEmpty(copyfromtext)?false:true: false;
    }

    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable2))] private string copytotext;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Isenable2))] private string copyfromtext;
    [ObservableProperty] private bool isvisable;   
    [ObservableProperty][AlsoNotifyChangeFor(nameof(canedit))] private int selectedpreset;
    [ObservableProperty]public List<string> ignorefiles =new();
    [ObservableProperty]public List<string> ignorefolder =new();
    [ObservableProperty] private bool? listing = IOC.Default.GetService<Settings>().Listingart;
    [ObservableProperty] private string selectedmultifolder;
    public bool Ismultiplevisable => IOC.Default.GetService<Settings>().MultipleSources; 
    public bool Copyfrom => !Ismultiplevisable;
    public ObservableCollection<string> Expanderpaths { get; set; } = new();

    public bool Isnotempty => Folderitems.Count != 0;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(progresstext))] public int progress;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(progresstext))] public int progress2;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(progresstext))] public int progressmax;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(progresstext))] public int progressmax2;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(progresstext))] public bool evaluating;
    public bool Expanderexpand => Expanderpaths.Any();
    
    public string progresstext => !evaluating ? "Evaluating" : progress>=progressmax?"Done":progress.ToString() + " of " + progressmax2.ToString();
    private PresetIndex? presetindex => IOC.Default.GetService<GetSetPresetIndex>().getpresetindex();
    
    [ICommand]
    public void AddPath()
    {
        if (!Expanderpaths.Contains(Copyfromtext) && Directory.Exists(Copyfromtext))
        {
              Expanderpaths.Add(Copyfromtext);
              OnPropertyChanged(nameof(Expanderexpand));
              Expanderpaths.Replace((from path in Expanderpaths orderby path select path).ToArray());
        }

        if (Expanderpaths.Any())
        {
            copyfromtext = "";
        }
        
        OnPropertyChanged(nameof(Copyfromtext));
    }

    [ICommand]
    private void MultiListRemove()
    {
        Expanderpaths.Remove(Selectedmultifolder);
        Expanderpaths.Replace((from path in Expanderpaths orderby path select path).ToArray());
        //OnPropertyChanged(nameof(Headertext));
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
    CancellationTokenSource cancel = new CancellationTokenSource(); 
    CancellationToken token = default;

    public void cancelall()
    {
        cancel.Dispose();
    }
    [ICommand]
    private async Task Copybutton()
    {
        isenable = false;
        if (IOC.Default.GetService<Settings>().Listingart != false && string.IsNullOrEmpty(Selectedlistitem))
        {
            ContentDialog dlg1 = new ContentDialog();
            dlg1.Title = "Please select a preset.";
            dlg1.PrimaryButtonText = "Ok";
            await dlg1.ShowAsync();
            return;
        }
        if (!System.IO.Path.Exists(copyfromtext) && !IOC.Default.GetService<Settings>().MultipleSources)
        {
            ContentDialog dlg1 = new ContentDialog();
            dlg1.Title = "Error";
            dlg1.Content = "Source path doesnt exist";
            dlg1.PrimaryButtonText = "Ok";
            await dlg1.ShowAsync();
            return;
        }

        if (!System.IO.Path.Exists(copytotext))
        {
            try
            {
                Directory.CreateDirectory(copytotext);
            }
            catch (Exception e)
            {
                ContentDialog dlg1 = new ContentDialog();
                dlg1.Title = "Error";
                dlg1.Content = "Couldnt find or create path";
                dlg1.PrimaryButtonText = "Ok";
                await dlg1.ShowAsync();
                return;
            }
            
        }
        OnPropertyChanged(nameof(Isenable2));
        var Pathh = copytotext;
        Optaci = 10;
        OnPropertyChanged(nameof(Optaci));
        if (String.IsNullOrEmpty(copyfromtext) && IOC.Default.GetService<Settings>().MultipleSources)
        {
            copyfromtext = Expanderpaths[0];
        }
        if (IOC.Default.GetService<Settings>().Packageformat == 0 && IOC.Default.GetService<Settings>().CreateOwnFolder == true)
        {
            var name = IOC.Default.GetService<Settings>().OwnFolderDate ? DateTime.Now.ToString("g").Replace(':','-') : IOC.Default.GetService<Settings>().OwnFolderName;
            Directory.CreateDirectory(System.IO.Path.Combine(Pathh, name));
            copytotext = System.IO.Path.Combine(Pathh, name);
        }

        if (copyfromtext.EndsWith("\\"))
        {
            string a = copyfromtext.Remove(copyfromtext.Length - 1);
            Copyfromtext = a;
        }

        if (copyfromtext.EndsWith(":"))
        {
            copyfromtext += "\\";
        }


        if (!Copytotext.EndsWith("\\"))
        {
            copytotext += "\\";
        }
        OnPropertyChanged(nameof(Copyfromtext));
        IOC.Default.GetService<Settings>().Pathfrom = copyfromtext;
        IOC.Default.GetService<Settings>().Pathto =  IOC.Default.GetService<Settings>().CreateOwnFolder==true ? Pathh : copytotext;
        bool iscancel = false;
        if (IOC.Default.GetService<Settings>().Clearaftercopy)
        {
            ContentDialog dlg1 = new ContentDialog();
            dlg1.Title = "Clear";
            dlg1.Content = "GnuCopy Deletes everything in the source folder. After copying.";
            dlg1.PrimaryButtonText = "Ok";
            dlg1.SecondaryButtonText = "Cancel";
            ContentDialogResult result = await dlg1.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                iscancel = true;
            }
        }
        
        if (IOC.Default.GetService<Settings>().Clearforcopy)
        {
            bool denied = false;
            var a = Directory.EnumerateDirectories(Copytotext,"*");
            var b = Directory.EnumerateFiles(copytotext, "*");
            if (a.Any() || b.Any())
            {
                ContentDialog dlg1 = new ContentDialog();
                dlg1.Title = "Clear";
                dlg1.Content = "GnuCopy Deletes everything in the selected target folder.";
                dlg1.PrimaryButtonText = "Ok";
                dlg1.SecondaryButtonText = "Cancel";
                ContentDialogResult result = await dlg1.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    denied = true;
                }
            }

            if (!denied)
            {
                await Task.Run(() =>
                {
                    if (a.Any())
                    {
                        foreach (var folder in a)
                        {
                            Directory.Delete(folder,true);
                        }
                    }

                    if (b.Any())
                    {
                        foreach (var file in b)
                        {
                            File.Delete(file);
                        }
                    }
                },token);
            }
        }
        
        await Task.Run( async ()=> await IOC.Default.GetService<StartCopyService>().Start(token),token);

        if (iscancel)
        {
            await Task.Run(() =>
            {
                var a = Directory.EnumerateDirectories(Copyfromtext,"*");
                var b = Directory.EnumerateFiles(copyfromtext, "*");
                if (a.Any())
                {
                    foreach (var folder in a)
                    {
                        Directory.Delete(folder, true);
                    }
                }

                if (b.Any())
                {
                    foreach (var file in b)
                    {
                        File.Delete(file);
                    }
                }
            },token);   
        }

        ContentDialog dlg = new ContentDialog();
        dlg.Title = "Done";
        dlg.Content = "GnuCopy finished operation.";
        dlg.PrimaryButtonText = "Close";
        dlg.SecondaryButtonText = "Go to target";
        if (await dlg.ShowAsync() is ContentDialogResult.Secondary)
        {
            ProcessStartInfo psi = new ProcessStartInfo(copytotext);
            psi.UseShellExecute = true;

            Process.Start(psi);
        }
        A:
        Optaci = 0;
        OnPropertyChanged(nameof(Optaci));
        isenable = true;
        OnPropertyChanged(nameof(Isenable2));
        copytotext = Pathh;
        iscancel = false;
        var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
        taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            copyfromtext = "";
            OnPropertyChanged(nameof(Copyfromtext));
        }
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
                    Expanderpaths.Replace((from path in Expanderpaths orderby path select path).ToArray());
                }
            }
           // OnPropertyChanged(nameof(Headertext));
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
        OnPropertyChanged(nameof(Copyfrom));
        if (Ismultiplevisable)
        {
            IOC.Default.GetService<MainWindow>().Addsources();
        }
    }

    [ICommand]
    private async void EditPreset()
    {
        if(selectedpreset == -1)
            return;
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
