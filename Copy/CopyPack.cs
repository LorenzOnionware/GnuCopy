using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using DynamicData;
using FluentAvalonia.UI.Data;
using GnuCopy.Interfaces;
using Project1.Services;
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
    public static HashSet<string> Copied = new(StringComparer.OrdinalIgnoreCase);
    public Task Start()
    {
        Copied.Clear();
        var settings = IOC.Default.GetService<Settings>();
        var name = settings.DateAsName
            ? DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture)
            : (string.IsNullOrWhiteSpace(settings.ZipName) ? "CompressedDirection" : settings.ZipName);
        name = SanitizeFileName(name);
        Directory.CreateDirectory(MainViewmodel.Default.Copytotext);
        string target = Path.Combine(MainViewmodel.Default.Copytotext, name) + (settings.Packageformat == 1 ? ".tar" : ".zip");
        MainViewmodel.Default.target1 = target;
        switch (IOC.Default.GetService<Settings>().Listingart)
        {
            case true:
                White(target);
                break;
            case false:
                All(target);
                break;
            default:
                Black(target);
                break;
        }
        return Task.CompletedTask;
    }

    private static string SanitizeFileName(string name)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder(name.Length);
        foreach (var character in name)
            sanitized.Append(Array.IndexOf(invalidCharacters, character) >= 0 ? '-' : character);

        var result = sanitized.ToString().Trim().TrimEnd('.');
        return string.IsNullOrWhiteSpace(result) ? "CompressedDirection" : result;
    }

    private static string current="";
    private static void Black(string target)
    {
        using Stream stream = File.Create(target);
        using var writer = CreateWriter(stream, IOC.Default.GetService<Settings>().Packageformat);
        foreach (var Folder0 in Source)
        {
            if (MainViewmodel.Default.Cancel) break;
            current = Folder0;
            var Folder = Path.GetDirectoryName(Folder0);
            if (string.IsNullOrEmpty(Folder) || !Directory.Exists(Folder)) continue;
            writer.WriteAll(Folder, "*", fileSearchFuncBlack, SearchOption.AllDirectories);
        }
    }

    private static void White(string target)
    {
        using Stream stream = File.Create(target);
        using var writer = CreateWriter(stream, IOC.Default.GetService<Settings>().Packageformat);
        var addedFolderNames = new List<string>();
        try
        {
            foreach (var Folder0 in Source)
            {
                if (MainViewmodel.Default.Cancel) break;
                current = Folder0;
                var Folder = Path.GetDirectoryName(Folder0);
                if (string.IsNullOrEmpty(Folder) || !Directory.Exists(Folder)) continue;

                var selectedName = Path.GetFileName(Path.TrimEndingDirectorySeparator(Folder0));
                if (!Igfolder.Contains(selectedName))
                {
                    Igfolder.Add(selectedName);
                    addedFolderNames.Add(selectedName);
                }
                var splitts = Folder.Split(Path.DirectorySeparatorChar);
                foreach (var a in splitts)
                {
                    if (!Igfolder.Contains(a))
                    {
                        Igfolder.Add(a);
                        addedFolderNames.Add(a);
                    }
                }
                writer.WriteAll(Folder, "*", fileSearchFuncWhite, SearchOption.AllDirectories);
            }
        }
        finally
        {
            foreach (var folderName in addedFolderNames) Igfolder.Remove(folderName);
        }
    }

    private static void All(string target)
    { 
        using (Stream stream = File.Create(target))
        {
            var settings = IOC.Default.GetService<Settings>();
            using var writer = CreateWriter(stream, settings.Packageformat);

            foreach (var Folder0 in Source)
            {
                current = Folder0;
                var Folder = Path.GetDirectoryName(Folder0);
                if (string.IsNullOrEmpty(Folder) || !Directory.Exists(Folder)) continue;
            
                writer.WriteAll(Folder, "*", AllFunc, SearchOption.AllDirectories);
            }
        }
    }

    private static IWriter CreateWriter(Stream stream, int packageFormat)
    {
        if (packageFormat == 1)
        {
            return WriterFactory.OpenWriter(stream, ArchiveType.Tar, new WriterOptions(CompressionType.None));
        }

        // Large ZIP entries need ZIP64; without it a single file over 4 GiB cannot be written.
        var options = new ZipWriterOptions(CompressionType.Deflate) { UseZip64 = true };
        return WriterFactory.OpenWriter(stream, ArchiveType.Zip, options);
    }


    private static Func<string, bool> fileSearchFuncBlack = filepath =>
    {
        if (Copied.Contains(filepath))
        {
            return false;
        }
        if (!IsWithinCurrentSource(filepath))
        {
            return false;
        }
        if (MainViewmodel.Default.Pause)
        {
            while (MainViewmodel.Default.Pause)
            {
                if (MainViewmodel.Default.Cancel) return false;
                Thread.Sleep(100);
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
            IOC.Default.GetService<ProgressBarService>().ReportFile(filepath);
            Copied.Add(filepath);
        }

        return output;
    };

    private static Func<string, bool> fileSearchFuncWhite = filepath =>
    {
        if (Copied.Contains(filepath))
        {
            return false;
        }
        if (!IsWithinCurrentSource(filepath))
        {
            return false;
        }
        if (MainViewmodel.Default.Pause)
        {
            while (MainViewmodel.Default.Pause)
            {
                if (MainViewmodel.Default.Cancel) return false;
                Thread.Sleep(100);
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
            Copied.Add(filepath);
            IOC.Default.GetService<ProgressBarService>().ReportFile(filepath);
        }
        return output;
    };

    private static Func<string, bool> AllFunc =CallerFilePathAttribute=>
    {
        if (Copied.Contains(CallerFilePathAttribute))
        {
            return false;
        }
        if (!IsWithinCurrentSource(CallerFilePathAttribute))
        {
            return false;
        }
        if (MainViewmodel.Default.Pause)
        {
            while (MainViewmodel.Default.Pause)
            {
                if (MainViewmodel.Default.Cancel) return false;
                Thread.Sleep(100);
            }
        }
        if (MainViewmodel.Default.Cancel)
        {
            return false;
        }
        IOC.Default.GetService<ProgressBarService>().ReportFile(CallerFilePathAttribute);
        Copied.Add(CallerFilePathAttribute);
        return true;
    };

    private static bool IsWithinCurrentSource(string path)
    {
        try
        {
            var root = Path.GetFullPath(current);
            var candidate = Path.GetFullPath(path);
            var relative = Path.GetRelativePath(root, candidate);
            return relative == "." || (!Path.IsPathRooted(relative) && relative != ".." &&
                                        !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal));
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
