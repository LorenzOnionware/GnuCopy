using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using DynamicData;
using GnuCopy.Interfaces;
using Project1.Viewmodels;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;
using Path = System.IO.Path;

namespace Project1;

public class CopyPack
{
    public static List<string> Igfolder = IOC.Default.GetService<MainViewmodel>().ignorefolder;
    public static List<string> Igfiles = IOC.Default.GetService<MainViewmodel>().ignorefiles;
    public static ObservableCollection<string> Source = IOC.Default.GetService<MainViewmodel>().Expanderpaths;

    public static async Task Start(CancellationTokenSource token)
    {
        string target = Path.Combine(MainViewmodel.Default.Copytotext,
            IOC.Default.GetService<Settings>().DateAsName
                ? DateTime.Now.ToString("G").Replace(':', '-')
                : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                    ? "CompressedDirection"
                    : IOC.Default.GetService<Settings>().ZipName));
        string target2 = target + (IOC.Default.GetService<Settings>().Packageformat == 1 ? ".tar" : ".zip");
        string target1 = Regex.Replace(target2, " ", "-");
        MainViewmodel.Default.target1 = target1;
        switch (IOC.Default.GetService<Settings>().Listingart)
        {
            case true:
                White(Regex.Replace(target1, "/", "-"));
                return;
            case false:
                All(Regex.Replace(target1, "/", "-"));
                return;
            default:
                Black(Regex.Replace(target1, "/", "-"));
                return;
        }
    }

    private static void Black(string target)
    {
        Stream stream = File.OpenWrite(target); 
        var writer = WriterFactory.Open(stream,(IOC.Default.GetService<Settings>().Packageformat == 1 ? ArchiveType.Tar : ArchiveType.Zip), IOC.Default.GetService<Settings>().Packageformat == 1 ? CompressionType.None : CompressionType.Deflate);
        foreach (var Folder in Source)
        {
            var subs = Directory.EnumerateDirectories(Folder, "*",
                new EnumerationOptions() { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden });
            if (subs.Any())
            {
                PrepEmptyDirs(subs.ToList(), false);
            }
            if (!Directory.GetFiles(Folder).Any())
            {
                File.Create(Path.Combine(Folder,"onionfile.ow"));
            }
            writer.WriteAll(Folder, "*", fileSearchFuncBlack, SearchOption.AllDirectories);
        }
        writer.Dispose();
        stream.Dispose();
    }

    private static void White(string target)
    {
        Stream stream = File.OpenWrite(target); 
        var writer = WriterFactory.Open(stream,(IOC.Default.GetService<Settings>().Packageformat == 1 ? ArchiveType.Tar : ArchiveType.Zip), IOC.Default.GetService<Settings>().Packageformat == 1 ? CompressionType.None : CompressionType.Deflate);
        foreach (var Folder in Source)
        {
            var splitts = Folder.Split(Path.DirectorySeparatorChar);
            foreach (var a in splitts)
            {
                Igfolder.Add(a);
            }
            Igfiles.Add(".ow");
            List<string> prep = new();
            var d = Directory.EnumerateDirectories(Folder, "*", SearchOption.AllDirectories);
            if (d.Any())
            {
                foreach (var dir in d)
                {
                    if (!Filter.FilterTask(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).ToList(), true).Any())
                    {
                        prep.Add(dir);
                    }
                }
            }
            if (prep.Any())
            {
                PrepEmptyDirs(prep, true);
            }
            writer.WriteAll(Folder, "*", fileSearchFuncWhite, SearchOption.AllDirectories);
            foreach (var a in splitts)
            {
                Igfolder.Remove(a);
            }
        }
        writer.Dispose();
        stream.Dispose();
    }

    private static void All(string target)
    { 
        Stream stream = File.OpenWrite(target); 
        var writer = WriterFactory.Open(stream,(IOC.Default.GetService<Settings>().Packageformat == 1 ? ArchiveType.Tar : ArchiveType.Zip), IOC.Default.GetService<Settings>().Packageformat == 1 ? CompressionType.None : CompressionType.Deflate);
        foreach (var Folder in Source)
        {
            var subs = Directory.EnumerateDirectories(Folder, "*",
                new EnumerationOptions() { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.Hidden });
            if (subs.Any())
            {
                PrepEmptyDirs(subs.ToList(), false);
            }

            if (!Directory.GetFiles(Folder).Any())
            {
                File.Create(Path.Combine(Folder,"onionfile.ow"));
            }
            
            writer.WriteAll(Folder, "*", AllFunc,SearchOption.AllDirectories);
        }
        writer.Dispose();
        stream.Dispose();
    }

    private static void PrepEmptyDirs(List<string> source, bool w)
    {
        foreach (var folder in source)
        {
            if (w)
            {
                File.Create(Path.Combine(folder, "onionfile.ow"));
                IOC.Default.GetService<MainViewmodel>().Progressmax++;
                return;
            }

            var files = Directory.GetFiles(folder);
            if (!files.Any())
            {
                File.Create(Path.Combine(folder, "onionfile.ow"));
                IOC.Default.GetService<MainViewmodel>().Progressmax++;
            }

        }
    }

    private static Func<string, bool> fileSearchFuncBlack = filepath =>
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
            return false;
        }

        bool output = !Igfiles.Contains(Path.GetExtension(filepath));
        string[] parts = filepath.Split(Path.DirectorySeparatorChar);
        foreach (var part in parts)
        {
            if (Igfolder.Contains(part))
            {
                return false;
            }
        }

        if (output)
        {
            IOC.Default.GetService<MainViewmodel>().currentfile = filepath;
            IOC.Default.GetService<MainViewmodel>().Actualise();
        }

        return output;
    };

    private static Func<string, bool> fileSearchFuncWhite = filepath =>
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
            return false;
        }
        
        bool output = Igfiles.Contains(Path.GetExtension(filepath));
        var splitts = filepath.Split(Path.DirectorySeparatorChar).ToList(); 
        splitts.Remove(Path.GetFileName(filepath));
        foreach (var part in splitts)
        {
            if (!Igfolder.Contains(part))
            {
                output = false;
            }   
        }
        if (output)
        {
            IOC.Default.GetService<MainViewmodel>().currentfile = filepath;
            IOC.Default.GetService<MainViewmodel>().Actualise();
        }
        return output;
    };

    private static Func<string, bool> AllFunc =CallerFilePathAttribute=>
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
            return false;
        }
        
        IOC.Default.GetService<MainViewmodel>().currentfile = CallerFilePathAttribute;
        IOC.Default.GetService<MainViewmodel>().Actualise();
        return true;
    };
}