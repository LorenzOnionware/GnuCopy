using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class Copy
{
    private static string TempFolder = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");

    public static async Task Settings(bool zip, CancellationToken token)
    {
        if (zip)
        {
            Directory.CreateDirectory(TempFolder);
            if (!Path.Exists(TempFolder))
            {
                throw new TempPathNotExistException("no valid temp path");
            }
        }

        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            if (zip)
            {
                MainViewmodel.Default.Progressmax += 10;
            }

            if (IOC.Default.GetService<Settings>().Listingart == false)
            {
                // await White(zip);
                await CopyMultiple.MAll(zip, token);
            }
        }
        else
        {


            await IOC.Default.GetService<IProgressBarService>().Progressmax(token);

            if (zip)
            {
                MainViewmodel.Default.Progressmax += 10;
            }

            if (IOC.Default.GetService<Settings>().Listingart == true)
            {
                // await White(zip);
                await White(zip, token);
            }
            else if (IOC.Default.GetService<Settings>().Listingart == false)
            {
                // await Black(zip);
                await Without(zip, token);
            }
            else
            {
                // await Without(zip);
                await Black(zip, token);
                IOC.Default.GetService<MainViewmodel>().ignorefolder.Add("OnionwareTemp");
            }
        }
    }

    private static async Task Without(bool zip,CancellationToken token)
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
        },token);
        
        
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
   //     var aa =MainViewmodel.Default.Progressmax % MainViewmodel.Default.progress;
     //   MainViewmodel.Default.progress += aa;
    }

    private static async Task Black(bool zip,CancellationToken token)
    { 
        string[] firstfiles = await CleanupLoops.CLean(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*").ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false,token);

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
        },token);

        string[] folderss = await CleanupLoops.CLean(Directory.GetDirectories(MainViewmodel.Default.Copyfromtext, "*", SearchOption.AllDirectories), IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),false,true,token);
        List<string> folders = folderss.ToList();
        if (zip)
        {
            folders.Remove(TempFolder);
        }
        foreach (var folder in folders)
        {
            var a = FolderPath(folder, zip);
            Directory.CreateDirectory(a);
            string[] files = await CleanupLoops.CLean(Directory.EnumerateFiles(folder).ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false,token);
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
    //    var aa = MainViewmodel.Default.progress % MainViewmodel.Default.Progressmax;
      //  MainViewmodel.Default.progress += aa;
    }

    private static async Task White(bool zip,CancellationToken token)
    { 
        string[] firstfiles = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*").ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false,token);
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
        },token);
        string[] folderss = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext,"*").ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),false,true,token);
        List<string> folders = folderss.ToList();
        if (zip)
        {
            folders.Remove(TempFolder);
        }
        foreach (var folder in folders) 
        {
           
            Directory.CreateDirectory(Path.Combine(FolderPath(folder,zip)));
            string[] files = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folder).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,false,token);
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
   //     var aa = MainViewmodel.Default.progress % MainViewmodel.Default.Progressmax;
     //   MainViewmodel.Default.progress += aa;
    }


    public static string GetName(string path) => File.Exists(path) ? Path.GetFileName(path) : new DirectoryInfo(path).Name;

    public static string FolderPath(string folder, bool zip)
    {
        var a = folder.Replace(MainViewmodel.Default.Copyfromtext, !zip ? MainViewmodel.Default.Copytotext : TempFolder);
        return a;
    }
    
}