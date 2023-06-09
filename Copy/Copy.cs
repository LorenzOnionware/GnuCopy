using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using Project1.Viewmodels;
using static Project1.MainWindow;
using static Project1.CopyMultiple;

namespace Project1;

public class Copy
{
    private static string TempFolder = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");

    public static async Task Settings(bool zip)
    {
        if (zip)
        {
            Directory.CreateDirectory(TempFolder);
            if (!Path.Exists(TempFolder))
            {
                throw new TempPathNotExistException("no valid temp path");
            }
        }

        await IOC.Default.GetService<IProgressBarService>().Progressmax();

        if (zip)
        {
            MainViewmodel.Default.Progressmax += 10;
        }
        
        if (IOC.Default.GetService<Settings>().Listingart == true)
        {
            // await White(zip);
            await White(zip);
        }
        else if (IOC.Default.GetService<Settings>().Listingart == false)
        {
            // await Black(zip);
            await Without(zip);
        }
        else
        {
            // await Without(zip);
            await Black(zip);
            IOC.Default.GetService<MainViewmodel>().ignorefolder.Add("OnionwareTemp");
        }
    }

    private static async Task Without(bool zip)
    { 
        var copytask = Task.Run(() =>
        {
            List<string> firstfiles = Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext).ToList();
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
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        });


        List<string> folders = Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext,"*",SearchOption.AllDirectories).ToList();
        if (zip)
        {
            folders.Remove(TempFolder);
        }

        foreach (var folder in folders)
        {
            Directory.CreateDirectory(Path.Combine(FolderPath(folder, zip)));
            List<string> files = Directory.EnumerateFiles(folder).ToList();
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file, Path.Combine(FolderPath(folder, zip), GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }

           
        }
        await copytask;
    }

    private static async Task Black(bool zip)
    { 
        string[] firstfiles = await CleanupLoops.CLean(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*").ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false);

        var copytask = Task.Run(() =>
        {
            foreach (var file in firstfiles)
            {
                try
                {
                    File.Copy(file, !zip ? Path.Combine(MainViewmodel.Default.Copytotext, GetName(file)) : Path.Combine(TempFolder, GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        });

        string[] folderss = await CleanupLoops.CLean(Directory.GetDirectories(MainViewmodel.Default.Copyfromtext, "*", SearchOption.AllDirectories), IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),false,true);
        List<string> folders = folderss.ToList();
        if (zip)
        {
            folders.Remove(TempFolder);
        }
        foreach (var folder in folders)
        {
            var a = FolderPath(folder, zip);
            Directory.CreateDirectory(a);
            string[] files = await CleanupLoops.CLean(Directory.EnumerateFiles(folder).ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false);
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file, Path.Combine(a, GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
            
        }
        await copytask;
    }

    private static async Task White(bool zip)
    { 
        string[] firstfiles = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*").ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false);
        var copytask = Task.Run(() =>
        { 
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
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        });
        string[] folderss = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext,"*").ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),false,true);
        List<string> folders = folderss.ToList();
        if (zip)
        {
            folders.Remove(TempFolder);
        }
        foreach (var folder in folders) 
        {
           
            Directory.CreateDirectory(Path.Combine(FolderPath(folder,zip)));
            string[] files = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folder).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false);
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file,Path.Combine(FolderPath(folder,zip),GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
           
        }
        await copytask;
    }


    public static string GetName(string path) => File.Exists(path) ? Path.GetFileName(path) : new DirectoryInfo(path).Name;

    public static string FolderPath(string folder, bool zip)
    {
        var a = folder.Replace(MainViewmodel.Default.Copyfromtext, !zip ? MainViewmodel.Default.Copytotext : TempFolder);
        return a;
    }
    
}