using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    private static string TempFolder = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");
    public static async Task MAll(bool zip)
    {
        HashSet<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
        foreach (var source in Source)
        {
            var copytask = Task.Run(() =>
            {
                List<string> firstfiles = Directory.EnumerateFiles(source).ToList();
                foreach (var file in firstfiles)
                {
                    try
                    {
                        File.Copy(file, zip == false ? Path.Combine(MainViewmodel.Default.Copytotext, GetName(file)) : Path.Combine(TempFolder, GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                    
                    }
                    catch (IOException e)
                    {
                        continue;
                    }
                }
            });

            var folder = Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories);
            foreach (var Folder in folder)
            {
                var a = FolderPath(Folder, zip);
                Directory.CreateDirectory(a);
                var Files = Directory.EnumerateFiles(Folder,"*");
                foreach (var File in Files)
                {
                    System.IO.File.Copy(File,Path.Combine(a, GetName(File)));
                }
            }

            await copytask;
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
    
    public static string FolderPath(string folder, bool zip)
    {
        var a = folder.Replace(MainViewmodel.Default.Copyfromtext, !zip ? MainViewmodel.Default.Copytotext : TempFolder);
        return a;
    }
}