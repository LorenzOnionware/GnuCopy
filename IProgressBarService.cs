using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class IProgressBarService
{
    private bool TaskBar => Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported;
    public void Progress()
    {
        MainViewmodel.Default.Progress++;
        if(TaskBar)
        {           
            var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal);
            taskbarInstance.SetProgressValue(MainViewmodel.Default.Progress, MainViewmodel.Default.Progressmax);
            
        }
    }
    

    public async Task Progressmax(CancellationToken token)
    {
        MainViewmodel.Default.Progressmax = 0;
        MainViewmodel.Default.Progress = 0;
        string source = MainViewmodel.Default.Copyfromtext;
        switch (IOC.Default.GetService<Settings>().Listingart)
        {
            case false:
                //copy all kontent
                var a = Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories).ToArray();
                MainViewmodel.Default.Progressmax = a.Length;
                MainViewmodel.Default.Progressmax2 = a.Length;
                break;
            case true:
                //Whitelist
                var b = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(source,"*", SearchOption.AllDirectories).ToArray(), MainViewmodel.Default.Ignorefolder.ToArray(),false,true,token);
                List<string> Files = new();
                foreach (var Folder in b)
                {
                    var ab = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Folder, "*").ToArray(),MainViewmodel.Default.Ignorefiles.ToArray(),false,false,token);
                    foreach (var n in ab)
                    {
                        Files.Add(n);
                    }
                }

                MainViewmodel.Default.Progressmax = Files.Count;
                MainViewmodel.Default.Progressmax2 = Files.Count;
                break;
            case null:
                //Blacklist
                var bb = await CleanupLoops.CLean(Directory.EnumerateDirectories(source,"*", SearchOption.AllDirectories).ToArray(), MainViewmodel.Default.Ignorefolder.ToArray(),false,true,token);
                List<string> Filess = new();
                foreach (var Folder in bb)
                {
                    var ab = await CleanupLoops.CLean(Directory.EnumerateFiles(Folder, "*").ToArray(),MainViewmodel.Default.Ignorefiles.ToArray(),false,false,token);
                    foreach (var n in ab)
                    {
                        Filess.Add(n);
                    }
                }
                MainViewmodel.Default.Progressmax = Filess.Count;
                MainViewmodel.Default.Progressmax2 = Filess.Count;
                break;
        }
    }
}