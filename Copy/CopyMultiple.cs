using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    public static async Task MAll(bool zip,CancellationTokenSource token)
    {
        HashSet<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
        foreach (var Folder in Source)
        {
            while (MainViewmodel.Default.Pause)
            {
                Debug.WriteLine("Stopp");       
            }
            if (IOC.Default.GetService<MainViewmodel>().Cancel)
            {
                break;
            }
            var path = (Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder)));
            Directory.CreateDirectory(path);

            var First = Task.Run(() =>
            {
                var files = Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System});
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    File.Copy(file, Path.Combine(path,Path.GetFileName(file)),overwrite:IOC.Default.GetService<Settings>().Overrite);
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            });
            List<string> folders = new List<string>();
            folders.Replace(Directory.EnumerateDirectories(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList());

            foreach (var folder in folders)
            {
                while (MainViewmodel.Default.Pause)
                {
                    Debug.WriteLine("Stopp");
                }
                if (IOC.Default.GetService<MainViewmodel>().Cancel)
                {
                    break;
                }
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder,folder,path))));
                List<string> files = Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList();
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    try
                    {
                        File.Copy(file, Path.Combine(FolderPath(Folder,folder,path), GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                    }
                    catch (IOException e)
                    {
                        continue;
                    }
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            }
            await First;
        }
    }

    public static async Task MBlack(bool zip, CancellationTokenSource token)
    {
        string[] Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToArray();

        foreach (var Folder in Source)
        {
            while (MainViewmodel.Default.Pause)
            {
                Debug.WriteLine("Stopp");  
            }
            if (IOC.Default.GetService<MainViewmodel>().Cancel)
            {
                break;
            }
            var path = (Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder)));

            var First = Task.Run(async () =>
            {
                var files = await CleanupLoops.CLean(Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, token);
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");   
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    File.Copy(file, Path.Combine(path, Path.GetFileName(file)),overwrite:IOC.Default.GetService<Settings>().Overrite);
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            });
            List<string> folders = new();
            folders.Replace(await CleanupLoops.CLean(
                Directory.EnumerateDirectories(Folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),  true, token));

            foreach (var folder in folders)
            {
                while (MainViewmodel.Default.Pause)
                {
                    Debug.WriteLine("Stopp");
                }
                if (IOC.Default.GetService<MainViewmodel>().Cancel)
                {
                    break;
                }
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder, folder, path))));
                List<string> files = new();
                files.Replace(await CleanupLoops.CLean(
                    Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList().ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),  false, token));;
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    try
                    {
                        File.Copy(file, Path.Combine(FolderPath(Folder, folder, path), GetName(file)),
                            overwrite: IOC.Default.GetService<Settings>().Overrite);
                    }
                    catch (IOException e)
                    {
                        continue;
                    }

                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            }
            await First;
        }
    }

    public static async Task MWhite(bool zip, CancellationTokenSource token)
    {
        string[] Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToArray();
        foreach (var Folder in Source)
        {
            while (MainViewmodel.Default.Pause)
            {
                Debug.WriteLine("Stopp");
            }
            if (IOC.Default.GetService<MainViewmodel>().Cancel)
            {
                break;
            }
            var path = (Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder)));
                
            Directory.CreateDirectory(path);

            var First = Task.Run(async () =>
            {
                var files = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, token);
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    File.Copy(file, Path.Combine(path, Path.GetFileName(file)),overwrite:IOC.Default.GetService<Settings>().Overrite);
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            });
            List<string> folders = new();
            folders.Replace(await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(Folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(), true, token));

            foreach (var folder in folders)
            {
                while (MainViewmodel.Default.Pause)
                {
                    Debug.WriteLine("Stopp");
                }
                if (IOC.Default.GetService<MainViewmodel>().Cancel)
                {
                    break;
                }
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder, folder, path))));
                List<string> files = new();
                files.Replace(await CleanupLoops.CLeanWhite(
                    Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList().ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false, token));;
                foreach (var file in files)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Stopp");
                    }
                    if (IOC.Default.GetService<MainViewmodel>().Cancel)
                    {
                        break;
                    }
                    try
                    {
                        File.Copy(file, Path.Combine(FolderPath(Folder, folder, path), GetName(file)),
                            overwrite: IOC.Default.GetService<Settings>().Overrite);
                    }
                    catch (IOException e)
                    {
                        continue;
                    }

                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            }
            await First;
        }
    }

    
    public static string GetName(string path) => File.Exists(path) ? Path.GetFileName(path) : new DirectoryInfo(path).Name;
    
    public static string FolderPath(string replace,string folder,string path)
    {
        var a = folder.Replace(replace, Path.Combine(path));
        return a;
    }
    static string GetLastPartFromPath(string path)
    {
        string[] parts = path.Split('\\');
        string lastPart = parts[parts.Length - 1];
        return lastPart;
    }
}