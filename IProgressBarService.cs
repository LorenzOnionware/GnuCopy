using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    public async Task Progressmax()
    {
        string source = MainViewmodel.Default.Copyfromtext;
        switch (IOC.Default.GetService<Settings>().Listingart)
        {
            case false:
                //copy all kontent
                var a = Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories).ToArray();
                MainViewmodel.Default.Progressmax = a.Length;
                break;
            case true:
                //Whitelist
                var b = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(source,"*", SearchOption.AllDirectories).ToArray(), MainViewmodel.Default.Ignorefolder.ToArray(),false,true);
                List<string> Files = new();
                foreach (var Folder in b)
                {
                    var ab = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Folder, "*").ToArray(),MainViewmodel.Default.Ignorefiles.ToArray(),false,false);
                    foreach (var n in ab)
                    {
                        Files.Add(n);
                    }
                }

                MainViewmodel.Default.Progressmax = Files.Count;
                break;
            case null:
                //Blacklist
                var bb = await CleanupLoops.CLean(Directory.EnumerateDirectories(source,"*", SearchOption.AllDirectories).ToArray(), MainViewmodel.Default.Ignorefolder.ToArray(),false,true);
                List<string> Filess = new();
                foreach (var Folder in bb)
                {
                    var ab = await CleanupLoops.CLean(Directory.EnumerateFiles(Folder, "*").ToArray(),MainViewmodel.Default.Ignorefiles.ToArray(),false,false);
                    foreach (var n in ab)
                    {
                        Filess.Add(n);
                    }
                }
                MainViewmodel.Default.Progressmax = Filess.Count;
                break;
        }
    }
}