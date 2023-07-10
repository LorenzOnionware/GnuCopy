using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    private static string TempFolder = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
    public static async Task MAll(bool zip,CancellationToken token)
    {
        HashSet<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
        string Temp = System.IO.Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
        if (zip)
        {
            Directory.CreateDirectory(Temp);
        }
        foreach (var Folder in Source)
        {
            var path = (!zip ? Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder)) : Path.Combine(Temp,GetLastPartFromPath(Folder)));
            Directory.CreateDirectory(path);

            var First = Task.Run(() =>
            {
                var files = Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System});
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(path,Path.GetFileName(file)));
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            },token);
            List<string> folders = new List<string>();
            folders.Replace(Directory.EnumerateDirectories(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList());
            if (zip)
            {
                HashSet<string> remove = new HashSet<string>();
                foreach (var a in folders)
                {
                    if (a.Contains("OnionwareTemp"))
                    {
                        remove.Add(a);
                    }
                }

                foreach (var a in remove)
                {
                    folders.Remove(a);
                }
            }

            foreach (var folder in folders)
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder,folder,path))));
                List<string> files = Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList();
                foreach (var file in files)
                {
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

    public static async Task MBlack(bool zip, CancellationToken token)
    {
        string[] Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToArray();
        string Temp =
            System.IO.Path.Combine(
                String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)
                    ? MainViewmodel.Default.Copyfromtext
                    : IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
        if (zip)
        {
            Directory.CreateDirectory(Temp);
        }

        foreach (var Folder in Source)
        {
            var path = (!zip
                ? Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder))
                : Path.Combine(Temp, GetLastPartFromPath(Folder)));
            Directory.CreateDirectory(path);

            var First = Task.Run(async () =>
            {
                var files = await CleanupLoops.CLean(Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, false, token);
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(path, Path.GetFileName(file)));
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            }, token);
            List<string> folders = new();
            folders.Replace(await CleanupLoops.CLean(
                Directory.EnumerateDirectories(Folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(), false, true, token));
            if (zip)
            {
                HashSet<string> remove = new HashSet<string>();
                foreach (var a in folders)
                {
                    if (a.Contains("OnionwareTemp"))
                    {
                        remove.Add(a);
                    }
                }

                foreach (var a in remove)
                {
                    folders.Remove(a);
                }
            }

            foreach (var folder in folders)
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder, folder, path))));
                List<string> files = new();
                files.Replace(await CleanupLoops.CLean(
                    Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList().ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, false, token));;
                foreach (var file in files)
                {
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

    public static async Task MWhite(bool zip, CancellationToken token)
    {
        string[] Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToArray();
        string Temp =
            System.IO.Path.Combine(
                String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)
                    ? MainViewmodel.Default.Copyfromtext
                    : IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
        if (zip)
        {
            Directory.CreateDirectory(Temp);
        }

        foreach (var Folder in Source)
        {
            var path = (!zip
                ? Path.Combine(MainViewmodel.Default.Copytotext, GetLastPartFromPath(Folder))
                : Path.Combine(Temp, GetLastPartFromPath(Folder)));
            Directory.CreateDirectory(path);

            var First = Task.Run(async () =>
            {
                var files = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, false, token);
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(path, Path.GetFileName(file)));
                    IOC.Default.GetService<IProgressBarService>().Progress();
                }
            }, token);
            List<string> folders = new();
            folders.Replace(await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(Folder, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(), false, true, token));
            if (zip)
            {
                HashSet<string> remove = new HashSet<string>();
                foreach (var a in folders)
                {
                    if (a.Contains("OnionwareTemp"))
                    {
                        remove.Add(a);
                    }
                }

                foreach (var a in remove)
                {
                    folders.Remove(a);
                }
            }

            foreach (var folder in folders)
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder, folder, path))));
                List<string> files = new();
                files.Replace(await CleanupLoops.CLeanWhite(
                    Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList().ToArray(),
                    IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, false, token));;
                foreach (var file in files)
                {
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