using System;
using System.Diagnostics;
using System.IO;
using Jab;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1;

[ServiceProvider]
[Singleton<MainViewmodel>]
[Singleton<IFileDialogService,Avaloniafiledialogservice>]
[Singleton<StartCopyService>]
[Singleton<IProgressBarService>]
[Singleton<AktualiselSettingsInUI>]
[Singleton<Settings>(Instance = nameof(JsonAppSettings))]
sealed partial class ServiceProvider
{
    public Settings JsonAppSettings { get; }
    
    public ServiceProvider()
    {
       
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuCopy", "Settings", "Settings.json");
        JsonAppSettings = File.Exists(path) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path)) ?? new() : new();
    }
}