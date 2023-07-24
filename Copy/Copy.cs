using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using Project1.Viewmodels;

namespace Project1;

public class Copy
{
    public static async Task Settings(bool zip, CancellationToken token)
    {
        if (IOC.Default.GetService<Settings>().MultipleSources)
        {
            await IOC.Default.GetService<IProgressBarService>().Progressmax(token,false);
            if (zip)
            {
                MainViewmodel.Default.Progressmax += 10;
            }

            if (IOC.Default.GetService<Settings>().Listingart == false)
            {
                // await All(zip,token);
                await CopyMultiple.MAll(zip, token);
            }
            if (IOC.Default.GetService<Settings>().Listingart == true)
            {
                // await White(zip,token);
                await CopyMultiple.MWhite(zip, token);
            }
            if (IOC.Default.GetService<Settings>().Listingart == null)
            {
                // await Black(zip,token);
                await CopyMultiple.MBlack(zip, token);
            }
        }
        else
        {
            await IOC.Default.GetService<IProgressBarService>().Progressmax(token,false);

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
        var copytask = Task.Run(async () =>
        {
            List<string> firstfiles = Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext,"*",new EnumerationOptions(){AttributesToSkip = FileAttributes.System|FileAttributes.Hidden}).ToList();
            byte exc = 0;
            foreach (var file in firstfiles)
            {
                A:
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext, GetName(file)));
                    
                }
                catch (IOException e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        },token);
        
        
        List<string> folders = Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList();

        int exc = 1;
        foreach (var folder in folders)
        {
            Directory.CreateDirectory(Path.Combine(FolderPath(folder, zip)));
            List<string> files = Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToList();
            foreach (var file in files)
            {
                A:
                try
                {
                    File.Copy(file, Path.Combine(FolderPath(folder, zip), GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (Exception e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }

           
        }
        await copytask;
    }

    private static async Task Black(bool zip,CancellationToken token)
    { 
        string[] firstfiles = await CleanupLoops.CLean(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,token);

        var copytask = Task.Run(async () =>
        {
            int exc = 0;
            foreach (var file in firstfiles)
            {
                A:
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext, GetName(file)));
                }
                catch (IOException e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        },token);

        string[] folderss = await CleanupLoops.CLean(Directory.GetDirectories(MainViewmodel.Default.Copyfromtext, "*", new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}), IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),true,token);
        List<string> folders = folderss.ToList();
        foreach (var folder in folders)
        {
            int exc = 0;
            var a = FolderPath(folder, zip);
            Directory.CreateDirectory(a);
            string[] files = await CleanupLoops.CLean(Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,token);
            foreach (var file in files)
            {
                A:
                try
                {
                    File.Copy(file, Path.Combine(a, GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
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
        string[] firstfiles = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(), IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,token);
        var copytask = Task.Run(async () =>
        {
            int exc = 0;
            foreach (var file in firstfiles)
            {
                A:
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext, GetName(file)));
           
                }
                catch (IOException e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
                }
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        },token);
        string[] folderss = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext,"*",new EnumerationOptions(){RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray(),true,token);
        List<string> folders = folderss.ToList();
        foreach (var folder in folders)
        {
            int exc = 0;
            Directory.CreateDirectory(Path.Combine(FolderPath(folder,zip)));
            string[] files = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folder,"*",new EnumerationOptions(){RecurseSubdirectories = false, AttributesToSkip = FileAttributes.Hidden|FileAttributes.System}).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,token);
            foreach (var file in files)
            {
                A:
                try
                {
                    File.Copy(file,Path.Combine(FolderPath(folder,zip),GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    exc++;
                    if(exc <=3)
                        goto A;
                    else if (exc > 3)
                    {
                        ContentDialog dlg = new();
                        dlg.Title = "Copy error";
                        dlg.Content = e;
                        dlg.PrimaryButtonText = "Ok";
                        if (await dlg.ShowAsync() is ContentDialogResult.Primary)
                        {
                            MainViewmodel.Default.cancelall();
                        }
                    }
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
        var a = folder.Replace(MainViewmodel.Default.Copyfromtext, MainViewmodel.Default.Copytotext);
        return a;
    }
    
}