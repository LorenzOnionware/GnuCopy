using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Resources;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Project1.Viewmodels;

[ObservableObject]
public partial class SettingsViewmodel
{
    public SettingsViewmodel(IFileDialogService FileDialogservice)
    {
        _fileDialogService = FileDialogservice;
    }
    private IFileDialogService _fileDialogService;
    
    public static SettingsViewmodel Default = IOC.Default.GetService<SettingsViewmodel>();
    public string settingspath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy\Settings\Settings.json";

    [ObservableProperty] private bool overritechecked = IOC.Default.GetService<Settings>().Overrite;
    [ObservableProperty] private bool clearforchecked = IOC.Default.GetService<Settings>().Clearforcopy;
    [ObservableProperty] private bool clearafterchecked = IOC.Default.GetService<Settings>().Clearaftercopy;
    [ObservableProperty] private bool savepaths = IOC.Default.GetService<Settings>().Savelastpaths;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(Overwriteenable))] private int comboboxselectedindex = IOC.Default.GetService<Settings>().Packageformat;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(nameenabled))] private bool dateasname = IOC.Default.GetService<Settings>().DateAsName;
    [ObservableProperty] private string? zipname = IOC.Default.GetService<Settings>().ZipName;
    [ObservableProperty] private string? costumepath = IOC.Default.GetService<Settings>().TempfolderPath;
    [ObservableProperty] private bool multiplesources = IOC.Default.GetService<Settings>().MultipleSources;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(ownfolderviseble))][AlsoNotifyChangeFor(nameof(Overwriteenable))] private bool createfolder = IOC.Default.GetService<Settings>().CreateOwnFolder;
    [ObservableProperty] private string createfoldername = IOC.Default.GetService<Settings>().OwnFolderName;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(owndateenabled))] private bool ownfolderdate = IOC.Default.GetService<Settings>().OwnFolderDate;
    [ObservableProperty] private bool custommica = IOC.Default.GetService<Settings>().CustomMica;
    [ObservableProperty] private byte micaintensy = IOC.Default.GetService<Settings>().MicaIntensy;
    public bool Overwriteenable
    {
        get {
            if (comboboxselectedindex != 0 || createfolder)
            {
                overritechecked = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public bool win => Environment.OSVersion.Version.Build >= 22000;
    public bool owndateenabled => !ownfolderdate;
     public bool? ownfolderviseble => createfolder;
    public string License => File.ReadAllText(@"pages\License.txt");


    [ICommand]
    private async Task PickFolderAsync()
    {
        Costumepath = await _fileDialogService.PickFolder();
        OnPropertyChanged(nameof(Costumepath));
    }

    [ICommand]
    public void LicenseExt()
    {

    }
    
    public bool nameenabled => !Dateasname;
}