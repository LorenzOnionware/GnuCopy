using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;
using SharpCompress.Archives.Zip;

namespace Project1;

public class IProgressBarService
{
    private bool TaskBar => Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported;
    public void Progress()
    {
        MainViewmodel.Default.Progress++;
        MainViewmodel.Default.Progress2 = MainViewmodel.Default.Progress2 = Math.Max(MainViewmodel.Default.Progress - 10, 0);;
        if(TaskBar)
        {           
            var taskbarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal);
            taskbarInstance.SetProgressValue(MainViewmodel.Default.Progress, MainViewmodel.Default.Progressmax);
            
        }
    }
    

    public async Task Progressmax(CancellationToken token, bool Zip)
    {
        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            MainViewmodel.Default.Progressmax = 0;
            MainViewmodel.Default.Progress = 0;

            switch (IOC.Default.GetService<Settings>().Listingart)
            {
                case false:
                //copy all kontent
                int files = 0;
                foreach (var folder in MainViewmodel.Default.Expanderpaths)
                {
                    var a = Directory.EnumerateFiles(folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray();
                    files += a.Length;
                }
                MainViewmodel.Default.evaluating = true;
                MainViewmodel.Default.Progressmax = files;
                MainViewmodel.Default.Progressmax2 = files;
                break;
                case true:
                    //Whitelist
                    int filess = 0;
                    foreach (var folder in MainViewmodel.Default.Expanderpaths)
                    {
                        var a = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(), MainViewmodel.Default.ignorefiles.ToArray(),false,false,token);
                        filess += a.Length;
                    }
                    MainViewmodel.Default.evaluating = true;
                    MainViewmodel.Default.Progressmax = filess;
                    MainViewmodel.Default.Progressmax2 = filess;
                    break;
                case null:
                    //Blacklist
                    int filesss = 0;
                    foreach (var folder in MainViewmodel.Default.Expanderpaths)
                    {
                        var a = await CleanupLoops.CLean(Directory.EnumerateFiles(folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(), MainViewmodel.Default.ignorefiles.ToArray(),false,false,token);
                        filesss += a.Length;
                    }
                    MainViewmodel.Default.evaluating = true;
                    MainViewmodel.Default.Progressmax = filesss;
                    MainViewmodel.Default.Progressmax2 = filesss;
                    break;
            }
        }
        else
        {
            MainViewmodel.Default.Progressmax = 0;
            MainViewmodel.Default.Progress = 0;
            string source = MainViewmodel.Default.Copyfromtext;
            switch (IOC.Default.GetService<Settings>().Listingart)
            {
                case false:
                    //copy all kontent
                    var a = Directory.EnumerateFiles(source, "*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray();
                    MainViewmodel.Default.evaluating = true;
                    MainViewmodel.Default.Progressmax = a.Length;
                    MainViewmodel.Default.Progressmax2 = a.Length;
                    break;
                case true:
                    //Whitelist
                    var b = await CleanupLoops.CLeanWhite(
                        Directory.EnumerateDirectories(source, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                        MainViewmodel.Default.Ignorefolder.ToArray(), false, true, token);
                    List<string> Files = new();
                    foreach (var Folder in b)
                    {
                        var ab = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Folder, "*").ToArray(),
                            MainViewmodel.Default.Ignorefiles.ToArray(), false, false, token);
                        foreach (var n in ab)
                        {
                            Files.Add(n);
                        }
                    }
                    MainViewmodel.Default.evaluating = true;
                    MainViewmodel.Default.Progressmax = Files.Count;
                    MainViewmodel.Default.Progressmax2 = Files.Count;
                    break;
                case null:
                    //Blacklist
                    var bb = await CleanupLoops.CLean(
                        Directory.EnumerateDirectories(source, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                        MainViewmodel.Default.Ignorefolder.ToArray(), false, true, token);
                    List<string> Filess = new();
                    foreach (var Folder in bb)
                    {
                        var ab = await CleanupLoops.CLean(Directory.EnumerateFiles(Folder, "*").ToArray(),
                            MainViewmodel.Default.Ignorefiles.ToArray(), false, false, token);
                        foreach (var n in ab)
                        {
                            Filess.Add(n);
                        }
                    }

                    MainViewmodel.Default.evaluating = true;
                    MainViewmodel.Default.Progressmax = Filess.Count;
                    MainViewmodel.Default.Progressmax2 = Filess.Count;
                    break;
            }
        }
    }
}