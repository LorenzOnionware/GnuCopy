using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using GnuCopy.Interfaces;
using Project1.Viewmodels;

namespace Project1.Services;

public class ProgressBarService : IProgressBarService
{
    public void AddProgress()
    {
        Dispatcher.UIThread.Post(() =>
        {
            IOC.Default.GetService<MainViewmodel>().Progress++;
            IOC.Default.GetService<MainViewmodel>().Actualise();
        });
    }

    public void MaxProgress()
    {
        List<string> files = new();
        foreach (var Folder in IOC.Default.GetService<MainViewmodel>().Expanderpaths)
        {
            var a = Filter.FilterTask(Directory.EnumerateFiles(Folder, "*", new EnumerationOptions() {RecurseSubdirectories = true,AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList(),
                true);
            foreach (var b in a)
            {
             files.Add(b);   
            }
        }
        IOC.Default.GetService<MainViewmodel>().Progressmax = files.Count;
    }
}