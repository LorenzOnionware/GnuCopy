using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1;

public partial class SettingsControl : ContentDialog, IStyleable
{
    private Settings settings = IOC.Default.GetService<Settings>();
    Type IStyleable.StyleKey => typeof(ContentDialog);
    
    public SettingsControl()
    {
        DataContext = SettingsViewmodel.Default;
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void ContentDialog_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
       string path = Path.Combine(SettingsViewmodel.Default.settingspath);
       settings.Clearaftercopy = SettingsViewmodel.Default.Clearafterchecked;
       settings.Clearforcopy = SettingsViewmodel.Default.Clearforchecked;
       settings.Savelastpaths = SettingsViewmodel.Default.Savepaths;
       settings.Packageformat = SettingsViewmodel.Default.Comboboxselectedindex;
       settings.Overrite = SettingsViewmodel.Default.Overritechecked;
       settings.DateAsName = SettingsViewmodel.Default.Dateasname;
       settings.ZipName = SettingsViewmodel.Default.Zipname;
       settings.TempfolderPath = SettingsViewmodel.Default.Costumepath;
       settings.MultipleSources = SettingsViewmodel.Default.Multiplesources;
       settings.CreateOwnFolder = SettingsViewmodel.Default.Createfolder;
       settings.OwnFolderName = SettingsViewmodel.Default.Createfoldername;
       settings.OwnFolderDate = SettingsViewmodel.Default.Ownfolderdate;
       settings.CustomMica = SettingsViewmodel.Default.Custommica;
       settings.MicaIntensy = SettingsViewmodel.Default.Micaintensy;
       string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
       File.WriteAllText(path,ab);
    }

    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        uri(false);
    }

    private void InputElement_OnTapped2(object? sender, TappedEventArgs e)
    {
        uri(true);
    }

    private async void uri(bool What)
    {
        if (What)
        {
            await Task.Run(() => Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/LorenzOnionware",
                UseShellExecute = true
            }));
        }
        else
        {
            await Task.Run(() => Process.Start(new ProcessStartInfo
            {
                FileName = "mailto:contact.onionware@gmail.com",
                UseShellExecute = true
            }));
        }
    }

    private void InputElement_OnTapped3(object? sender, TappedEventArgs e)
    {
        //Thirdparty
        string filePath = System.IO.Path.Combine(AppContext.BaseDirectory, @"pages\Third-Party.txt");

        ProcessStartInfo psi = new ProcessStartInfo(filePath);
        psi.UseShellExecute = true;

        Process.Start(psi);
    }

    private void InputElement_OnTapped4(object? sender, TappedEventArgs e)
    {
        string filePath = System.IO.Path.Combine(AppContext.BaseDirectory, @"pages\License.txt");

        ProcessStartInfo psi = new ProcessStartInfo(filePath);
        psi.UseShellExecute = true;

        Process.Start(psi);
    }
   
    private async void Thanks_OnTapped(object? sender, TappedEventArgs e)
    {
        var window = new GnuCopy.pages.Thanks();
        await window.ShowAsync();
    }
    private async void Thanks_OnTapped2(object? sender, TappedEventArgs e)
    {
        await Task.Run(() => Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.paypal.com/donate/?hosted_button_id=5TABD3FZYH452",
            UseShellExecute = true
        }));
    }
}
