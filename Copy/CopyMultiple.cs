using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using GnuCopy.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    public static bool? listing = IOC.Default.GetService<Settings>().Listingart;
    private static string dest2 = "";
    private static FileAttributes Skip = FileAttributes.Hidden | FileAttributes.System;
    private static bool Overwrite = IOC.Default.GetService<Settings>().Overrite;
    public static async Task Start(ObservableCollection<string> Paths)
    {
        foreach (var foler in IOC.Default.GetService<MainViewmodel>().Expanderpaths)
        {
            switch (listing)
            {
                case false:
                    CopyDirectory(foler, IOC.Default.GetService<MainViewmodel>().Copytotext);
                    break;
                case true :
                    CopyWhite(foler, IOC.Default.GetService<MainViewmodel>().Copytotext);
                    break;
                default:
                    CopyBlack(foler, IOC.Default.GetService<MainViewmodel>().Copytotext);
                    break;
            }
        }
    }
   
    private static async Task CopyDirectory(string sourcePath,string destPath)
    {
        Debug.WriteLine(sourcePath);
        var subdirs = Directory.EnumerateDirectories(sourcePath, "*",
            new EnumerationOptions()
                { RecurseSubdirectories = true, AttributesToSkip =Skip });
        var dest = Path.Combine(destPath, Path.GetFileName(sourcePath));
        Directory.CreateDirectory(dest);
        var FFiles= Directory.EnumerateFiles(sourcePath, "*",
            new EnumerationOptions()
                { RecurseSubdirectories = false, AttributesToSkip = Skip });
        foreach (var file in FFiles)
        {
            if (MainViewmodel.Default.Pause)
            {
                while (MainViewmodel.Default.Pause)
                {
                    Debug.WriteLine("Pause");
                }
            }

            if (MainViewmodel.Default.Cancel)
            {
                return;
            }
            Debug.WriteLine("FF"+Path.Combine(dest,Path.GetFileName(file)));
            IOC.Default.GetService<MainViewmodel>().currentfile = file;
            IOC.Default.GetService<MainViewmodel>().Actualise();
            try
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite: Overwrite);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
            {
                IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                IOC.Default.GetService<MainViewmodel>().Checking();
            }
        }

        foreach (var sub in subdirs)
        {
            if (MainViewmodel.Default.Pause)
            {
                while (MainViewmodel.Default.Pause)
                {
                    Debug.WriteLine("Pause");
                }
            }
            if (MainViewmodel.Default.Cancel)
            {
                return;
            }
            Debug.WriteLine(sourcePath+"  "+destPath+"  "+sub);
            var de = sub.Replace(sourcePath,dest);
            Debug.WriteLine($"de= {de}");
            var files = Directory.EnumerateFiles(sub, "*",
                new EnumerationOptions() { RecurseSubdirectories = false, AttributesToSkip = Skip });
            Directory.CreateDirectory(de);
            foreach (var file in files)
            {
                if (MainViewmodel.Default.Pause)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Pause");
                    }
                }

                if (MainViewmodel.Default.Cancel)
                {
                    return;
                }
                Debug.WriteLine(Path.Combine(de,Path.GetFileName(file)));
                IOC.Default.GetService<MainViewmodel>().currentfile = file;
                IOC.Default.GetService<MainViewmodel>().Actualise();
                try
                {
                    File.Copy(file,Path.Combine(de,Path.GetFileName(file)), Overwrite);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
                {
                    IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                    IOC.Default.GetService<MainViewmodel>().Checking();
                }
            }
        }
    }
    private static void CopyWhite(string sourcePath, string destPath)
    {
        var splitts0 = sourcePath.Split(Path.DirectorySeparatorChar);
        foreach (var a in splitts0)
        {
            IOC.Default.GetService<MainViewmodel>().ignorefolder.Add(a);
        }
        
         Debug.WriteLine(sourcePath);
        var subdirs = Directory.EnumerateDirectories(sourcePath, "*",new EnumerationOptions(){ RecurseSubdirectories = true, AttributesToSkip =Skip });
        var dest = Path.Combine(destPath, Path.GetFileName(sourcePath));
        Directory.CreateDirectory(dest);
        var FFiles= Directory.EnumerateFiles(sourcePath, "*", new EnumerationOptions(){ RecurseSubdirectories = false, AttributesToSkip = Skip });
        foreach (var file in FFiles)
        {
            Debug.WriteLine("FF" + Path.Combine(dest, Path.GetFileName(file)));
            if (IOC.Default.GetService<MainViewmodel>().ignorefiles.Contains(Path.GetExtension(file)))
            {
                if (MainViewmodel.Default.Pause)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Pause");
                    }
                }

                if (MainViewmodel.Default.Cancel)
                {
                    return;
                }
                IOC.Default.GetService<MainViewmodel>().currentfile = file;
                IOC.Default.GetService<MainViewmodel>().Actualise();
                try
                {
                   File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite: Overwrite);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
                {
                    IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                    IOC.Default.GetService<MainViewmodel>().Checking();
                }
            }
        }

        foreach (var sub in subdirs)
        {
            var splitts = sub.Split(Path.DirectorySeparatorChar);
            bool deny = false;
            foreach (var a in splitts)
            {
                if (!IOC.Default.GetService<MainViewmodel>().ignorefolder.Contains(a))
                {
                    deny = true;
                }
            }
            if (!deny)
            {
                Debug.WriteLine(sourcePath + "  " + destPath + "  " + sub);
                var de = sub.Replace(sourcePath, dest);
                Debug.WriteLine($"de= {de}");
                var files = Directory.EnumerateFiles(sub, "*",
                    new EnumerationOptions() { RecurseSubdirectories = false, AttributesToSkip = Skip });
                Directory.CreateDirectory(de);
                foreach (var file in files)
                {
                    if (MainViewmodel.Default.Pause)
                    {
                        while (MainViewmodel.Default.Pause)
                        {
                            Debug.WriteLine("Pause");
                        }
                    }

                    if (MainViewmodel.Default.Cancel)
                    {
                        return;
                    }
                    Debug.WriteLine(Path.Combine(de, Path.GetFileName(file)));
                    if (IOC.Default.GetService<MainViewmodel>().ignorefiles.Contains(Path.GetExtension(file)))
                    {
                        IOC.Default.GetService<MainViewmodel>().currentfile = file;
                        IOC.Default.GetService<MainViewmodel>().Actualise();
                        try
                        {
                           File.Copy(file, Path.Combine(de, Path.GetFileName(file)),Overwrite);
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                       
                        if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
                        {
                            IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                            IOC.Default.GetService<MainViewmodel>().Checking();
                        }
                    }
                }
            }
        }
        foreach (var a in splitts0)
        {
            IOC.Default.GetService<MainViewmodel>().ignorefolder.Remove(a);
        }
    }
    private static void CopyBlack(string sourcePath, string destPath)
    {
        Debug.WriteLine(sourcePath);
        var subdirs = Directory.EnumerateDirectories(sourcePath, "*",
            new EnumerationOptions()
                { RecurseSubdirectories = true, AttributesToSkip =Skip });
        var dest = Path.Combine(destPath, Path.GetFileName(sourcePath));
        Directory.CreateDirectory(dest);
        var FFiles= Directory.EnumerateFiles(sourcePath, "*",
            new EnumerationOptions()
                { RecurseSubdirectories = false, AttributesToSkip = Skip });
        foreach (var file in FFiles)
        {
            Debug.WriteLine("FF" + Path.Combine(dest, Path.GetFileName(file)));
            if (!IOC.Default.GetService<MainViewmodel>().ignorefiles.Contains(Path.GetExtension(file)))
            {
                if (MainViewmodel.Default.Pause)
                {
                    while (MainViewmodel.Default.Pause)
                    {
                        Debug.WriteLine("Pause");
                    }
                }

                if (MainViewmodel.Default.Cancel)
                {
                    return;
                }
                IOC.Default.GetService<MainViewmodel>().currentfile = file;
                IOC.Default.GetService<MainViewmodel>().Actualise();
                try
                {
                       File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite: Overwrite);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
             
                if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
                {
                    IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                    IOC.Default.GetService<MainViewmodel>().Checking();
                }
            }
        }

        foreach (var sub in subdirs)
        {
            var splitts = sub.Split(Path.DirectorySeparatorChar);
            bool deny = false;
            foreach (var a in splitts)
            {
                if (IOC.Default.GetService<MainViewmodel>().ignorefolder.Contains(a))
                {
                    deny = true;
                }
            }
            if (!deny)
            {
                Debug.WriteLine(sourcePath + "  " + destPath + "  " + sub);
                var de = sub.Replace(sourcePath, dest);
                Debug.WriteLine($"de= {de}");
                var files = Directory.EnumerateFiles(sub, "*",
                    new EnumerationOptions() { RecurseSubdirectories = false, AttributesToSkip = Skip });
                Directory.CreateDirectory(de);
                foreach (var file in files)
                {
                    if (MainViewmodel.Default.Pause)
                    {
                        while (MainViewmodel.Default.Pause)
                        {
                            Debug.WriteLine("Pause");
                        }
                    }

                    if (MainViewmodel.Default.Cancel)
                    {
                        return;
                    }

                    Debug.WriteLine(Path.Combine(de, Path.GetFileName(file)));
                    if (!IOC.Default.GetService<MainViewmodel>().ignorefiles.Contains(Path.GetExtension(file)))
                    {
                        IOC.Default.GetService<MainViewmodel>().currentfile = file;
                        IOC.Default.GetService<MainViewmodel>().Actualise();
                        try
                        {
                            File.Copy(file, Path.Combine(de, Path.GetFileName(file)), Overwrite);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        if (IOC.Default.GetService<MainViewmodel>().Progressmax== IOC.Default.GetService<MainViewmodel>().Progress)
                        {
                            IOC.Default.GetService<MainViewmodel>().currentfile = "Checking data";
                            IOC.Default.GetService<MainViewmodel>().Checking();
                        }
                    }
                }
            }
        }
    }
}