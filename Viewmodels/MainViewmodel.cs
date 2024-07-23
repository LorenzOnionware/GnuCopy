using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using GnuCopy.Interfaces;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Newtonsoft.Json;
using Project1.Presets;
using ReactiveUI;
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
        IOC.Default.GetService<WindowClosingService>().Closed += (o, e) => Close();
        _fileDialogService = FileDialogservice;

        USBMonitor usbMonitor = new USBMonitor();

        usbMonitor.USBInserted += UsbInsertedHandler;
        usbMonitor.USBRemoved += UsbRemovedHandler;

        usbMonitor.StartMonitoring();
    }

    public async void Close()
    {
        Cancel = true;
        Thread.Sleep(500);
        string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
        File.WriteAllText(Path.Combine(SettingsViewmodel.Default.settingspath),ab);
    }
    private IFileDialogService _fileDialogService;
    
    private static void UsbInsertedHandler(object sender, USBEventArgs e)
    {
        
    }

    private async void UsbRemovedHandler(object sender, USBEventArgs e)
    {
        if (!Path.Exists(copytotext) || !Path.Exists(copyfromtext))
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Error";
            dialog.Content = "USB removed during the copying process. The process has been canceled.";
            dialog.PrimaryButtonText = "Ok";
            await dialog.ShowAsync();
            Cancel = true;
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

    private bool isenable = true;
    public  bool Isenable2
    {
        get => isenable ? IOC.Default.GetService<Settings>().MultipleSources ? Expanderpaths.Any()?true:false : String.IsNullOrEmpty(copyfromtext)?false:true: false;
    }

    [ObservableProperty][AlsoNotifyChangeFor(nameof(isenable4))]private bool isenable3 = false;
    
    public bool isenable4 => !isenable3;

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
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Progressmax))][AlsoNotifyChangeFor(nameof(Progress))] public string currentfile = "";
    [ObservableProperty]public int progress;
    [ObservableProperty]public int progressmax;
    public bool Expanderexpand => Expanderpaths.Any();
    private PresetIndex? presetindex => IOC.Default.GetService<GetSetPresetIndex>().getpresetindex();

    public void Actualise()
    {
        Progress++;
        OnPropertyChanged(nameof(Currentfile));
    }
    
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
    public bool Cancel = false;
    public bool Pause=false;

    [ICommand]
    private async Task Cancell()
    {
        Pause = true;
        ContentDialog dlg = new ContentDialog();
        dlg.Title = "Warning!";
        dlg.Content = "If process is cancelled, some of the data copied already may be corrupt!";
        dlg.SecondaryButtonText = "Proceed cancellation";
        dlg.PrimaryButtonText = "Take me back";
        var result2 = await dlg.ShowAsync();
        if (result2 == ContentDialogResult.Secondary)
        {
            Pause = false;
            Cancel = true;
        }

        if (result2 == ContentDialogResult.Primary)
        {
            Cancel = false;
            Pause = false;
        }


        if (Cancel)
        {
            Cancel = true;
            ContentDialog dlg2 = new ContentDialog();
            dlg2.Title = "Cancelled";
            dlg2.Content = "Some of the data copied already may be corrupt!";
            dlg2.PrimaryButtonText = "OK";
            Pause = false;
            await dlg2.ShowAsync();


            Pause = false;
            Isenable3 = false;
            OnPropertyChanged(nameof(Isenable3));
            Optaci = 0;
            OnPropertyChanged(nameof(Optaci));
            isenable = true;
            OnPropertyChanged(nameof(Isenable2));
            var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
            if (IOC.Default.GetService<Settings>().MultipleSources)
            {
                copyfromtext = "";
                OnPropertyChanged(nameof(Copyfromtext));
            }

            if (result2 == ContentDialogResult.Primary)
            {
                Pause = false;
            }
        }
    }

    public string copyto;
    public string target1;
    public string deletat;
    [ICommand]
    private async Task Copybutton()
    {
        Cancel = false;
        isenable = false;
        Isenable3 = true;
        
        foreach (var Folder in Expanderpaths)
        {
            if (!Path.Exists(Folder))
            {
                ContentDialog dlg = new ContentDialog();
                dlg.Title = "Could not find certain paths.";
                dlg.Content = "Please check your source paths.";
                dlg.PrimaryButtonText = "ok";
                if (await dlg.ShowAsync() == ContentDialogResult.Primary)
                {
                    return;
                }
            }
        }
        if (IOC.Default.GetService<Settings>().Listingart != false && string.IsNullOrEmpty(Selectedlistitem))
        {
            ContentDialog dlg1 = new ContentDialog();
            dlg1.Title = "Please select a preset.";
            dlg1.PrimaryButtonText = "Ok";
            await dlg1.ShowAsync();
            Optaci = 0;
            OnPropertyChanged(nameof(Optaci));
            Cancel = false;
            isenable = true;
            OnPropertyChanged(nameof(Isenable2));
            Isenable3 = false;
            OnPropertyChanged(nameof(Isenable3));
            return;
        }

        if (Copyfromtext == copytotext)
        {
            ContentDialog dlg1 = new ContentDialog();
            dlg1.Title = "Error";
            dlg1.Content = "Source and target are equals.";
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
                Optaci = 0;
                OnPropertyChanged(nameof(Optaci));
                Cancel = false;
                isenable = true;
                OnPropertyChanged(nameof(Isenable2));
                Isenable3 = false;
                OnPropertyChanged(nameof(Isenable3));
                return;
            }
            
        }
        OnPropertyChanged(nameof(Isenable2));
        var Pathh = copytotext;
        Optaci = 10;
        OnPropertyChanged(nameof(Optaci));
        copyto = copytotext;
        if (String.IsNullOrEmpty(copyfromtext) && IOC.Default.GetService<Settings>().MultipleSources)
        {
            copyfromtext = Expanderpaths[0];
        }
        if (IOC.Default.GetService<Settings>().Packageformat == 0 && IOC.Default.GetService<Settings>().CreateOwnFolder)
        {
            var name = IOC.Default.GetService<Settings>().OwnFolderDate ? DateTime.Now.ToString("G").Replace(':','-') : !String.IsNullOrEmpty(IOC.Default.GetService<Settings>().OwnFolderName)?IOC.Default.GetService<Settings>().OwnFolderName : "GnuCopyFolder";
            Directory.CreateDirectory(System.IO.Path.Combine(Pathh, name));
            copytotext = System.IO.Path.Combine(Pathh, name);
            deletat = System.IO.Path.Combine(Pathh, name);
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
                });
            }
        }

        ProgressBarService Pc = new ProgressBarService();
        Pc.MaxProgress();
        OnPropertyChanged(nameof(Currentfile));
        await Task.Run( ()=> IOC.Default.GetService<StartCopyService>().Start(cancel,Expanderpaths));
        
        if (Cancel)
        { 
            goto A;
        }
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
            });
        }

        if (!Cancel)
        {
            progress = progressmax;
            OnPropertyChanged(nameof(Progress));
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
        }


        A:
        Optaci = 0;
        OnPropertyChanged(nameof(Optaci));
        Cancel = false;
        isenable = true;
        OnPropertyChanged(nameof(Isenable2));
        Isenable3 = false;
        OnPropertyChanged(nameof(Isenable3));
        copytotext = copyto;
        iscancel = false;
        var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
        taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            copyfromtext = "";
            OnPropertyChanged(nameof(Copyfromtext));
        }

        progress = 0;
        OnPropertyChanged(nameof(Progress));
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
            var a = await _fileDialogService.PickFolder();
            Copyfromtext = String.IsNullOrEmpty(a)?IOC.Default.GetService<Settings>().Pathfrom:a;
            IOC.Default.GetService<Settings>().Pathfrom = copyfromtext;
        }

    }
    
    [ICommand]
    private async Task CopyTargetDialog()
    {
        var a = await _fileDialogService.PickFolder();
        Copytotext = String.IsNullOrEmpty(a)?IOC.Default.GetService<Settings>().Pathto:a;
        if (!String.IsNullOrEmpty(a))
        { 
            IOC.Default.GetService<Settings>().Pathto = copytotext;
        }
   
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
