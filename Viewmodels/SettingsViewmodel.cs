using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

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
    [ObservableProperty] private int comboboxselectedindex = IOC.Default.GetService<Settings>().Packageformat;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(nameenabled))] private bool dateasname = IOC.Default.GetService<Settings>().DateAsName;
    [ObservableProperty] private string? zipname = IOC.Default.GetService<Settings>().ZipName;
    [ObservableProperty] private string? costumepath = IOC.Default.GetService<Settings>().TempfolderPath;
    [ObservableProperty] private bool multiplesources = IOC.Default.GetService<Settings>().MultipleSources;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(ownfolderviseble))] private bool? createfolder = IOC.Default.GetService<Settings>().CreateOwnFolder;
    [ObservableProperty] private string createfoldername = IOC.Default.GetService<Settings>().OwnFolderName;
    [ObservableProperty][AlsoNotifyChangeFor(nameof(owndateenabled))] private bool ownfolderdate = IOC.Default.GetService<Settings>().OwnFolderDate;
     public bool owndateenabled => !ownfolderdate;
     public bool? ownfolderviseble => createfolder;
    public string License => File.ReadAllText(@"pages\License.txt");

    [ICommand]
    private async Task PickFolderAsync()
    {
        Costumepath = await _fileDialogService.PickFolder();
        OnPropertyChanged(nameof(Costumepath));
    }
    
    public bool nameenabled => !Dateasname;
}