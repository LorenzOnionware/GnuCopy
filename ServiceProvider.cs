using System;
using System.IO;
using System.Threading.Tasks;
using Jab;
using Newtonsoft.Json;
using Project1.Presets;
using Project1.Viewmodels;

namespace Project1;

[ServiceProvider]
[Singleton<MainViewmodel>]
[Singleton<SettingsViewmodel>]
[Singleton<IFileDialogService,Avaloniafiledialogservice>]
[Singleton<StartCopyService>]
[Singleton<AktualiselSettingsInUI>]
[Singleton<Settings>(Instance = nameof(JsonAppSettings))]
[Singleton<PresetIndex>]
[Singleton<GetSetPresetIndex>]
[Singleton<ExportViewmodel>]
[Singleton<MainWindow>]
[Singleton<ProgressBarService>]
[Singleton<WindowClosingService>]
[Singleton<EditPresetsWindow>]
sealed partial class ServiceProvider
{
    public Settings JsonAppSettings { get; }
   
    public ServiceProvider()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuCopy", "Settings", "Settings.json");
        Task.Run(() =>
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuCopy"));
                Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuCopy", "Settings"));
            }
        });
        
        JsonAppSettings = File.Exists(path) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path)) ?? new() : new();
    }
}