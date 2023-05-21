using System;
using System.IO;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1.Presets;

public class GetSetPresetIndex
{
    public PresetIndex getpresetindex()
    {
        var x=  JsonConvert.DeserializeObject<PresetIndex>(File.ReadAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"GnuCopy", IOC.Default.GetService<MainViewmodel>().Selectedlistitem)));
        return x;
    }
}