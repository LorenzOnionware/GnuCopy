using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    private static string TempFolder = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
    public static async Task MAll(bool zip,CancellationToken token)
    {
        zip = false;
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
                var files = Directory.EnumerateFiles(Folder);
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(path,Path.GetFileName(file)));
                }
            },token);
            
            List<string> folders = Directory.EnumerateDirectories(Folder,"*",SearchOption.AllDirectories).ToList();
            if (zip)
            {
                folders.Remove(TempFolder);
            }

            foreach (var folder in folders)
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(FolderPath(Folder,folder,path))));
                List<string> files = Directory.EnumerateFiles(folder).ToList();
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

    public static async Task MBlack(bool zip)
    {
        HashSet<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
    }

    public static async Task MWhite(bool zip)
    {
        HashSet<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
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