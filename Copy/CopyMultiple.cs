using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Project1.Services;
using Project1.Viewmodels;

namespace Project1;

public class CopyMultiple
{
    private static readonly FileAttributes SkippedAttributes = FileAttributes.Hidden | FileAttributes.System;
    private readonly bool _overwrite = IOC.Default.GetService<Settings>().Overrite;

    public Task Start()
    {
        var viewmodel = IOC.Default.GetService<MainViewmodel>();
        var destinationRoot = Path.GetFullPath(viewmodel.Copytotext);

        foreach (var source in viewmodel.Expanderpaths)
        {
            if (viewmodel.Cancel)
                break;

            var mode = viewmodel.Listing;
            CopyTree(source, destinationRoot, mode);
        }

        return Task.CompletedTask;
    }

    private void CopyTree(string sourcePath, string destinationRoot, bool? mode)
    {
        var viewmodel = IOC.Default.GetService<MainViewmodel>();
        var sourceRoot = Path.GetFullPath(sourcePath);
        var rootName = Path.GetFileName(Path.TrimEndingDirectorySeparator(sourceRoot));
        var targetRoot = Path.GetFullPath(Path.Combine(destinationRoot, rootName));
        var allowedFolderNames = new HashSet<string>(viewmodel.ignorefolder, StringComparer.OrdinalIgnoreCase);
        if (mode == true)
        {
            foreach (var segment in sourceRoot.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
                allowedFolderNames.Add(segment);
        }

        if (IsSameOrChildPath(targetRoot, sourceRoot))
        {
            Debug.WriteLine($"Skipping destination inside source: {targetRoot}");
            return;
        }

        var pending = new Stack<(string Source, string Target)>();
        pending.Push((sourceRoot, targetRoot));

        while (pending.Count > 0)
        {
            WaitWhilePaused(viewmodel);
            if (viewmodel.Cancel)
                return;

            var (source, target) = pending.Pop();
            try
            {
                Directory.CreateDirectory(target);
                foreach (var file in Directory.EnumerateFiles(source, "*", new EnumerationOptions
                         { RecurseSubdirectories = false, AttributesToSkip = SkippedAttributes }))
                {
                    WaitWhilePaused(viewmodel);
                    if (viewmodel.Cancel)
                        return;

                    var extension = Path.GetExtension(file);
                    var include = mode switch
                    {
                        true => viewmodel.ignorefiles.Exists(f => string.Equals(f, extension, StringComparison.OrdinalIgnoreCase)),
                        false => true,
                        null => !viewmodel.ignorefiles.Exists(f => string.Equals(f, extension, StringComparison.OrdinalIgnoreCase))
                    };
                    if (!include)
                        continue;

                    try
                    {
                        File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: _overwrite);
                        viewmodel.currentfile = file;
                        IOC.Default.GetService<ProgressBarService>().AddProgress();
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
                    {
                        Debug.WriteLine($"Could not copy '{file}': {ex}");
                    }

                    if (viewmodel.Progressmax == viewmodel.Progress)
                    {
                        viewmodel.currentfile = "Checking data";
                        viewmodel.Checking();
                    }
                }

                foreach (var directory in Directory.EnumerateDirectories(source, "*", new EnumerationOptions
                         { RecurseSubdirectories = false, AttributesToSkip = SkippedAttributes }))
                {
                    var segments = directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                    if (mode == true && Array.Exists(segments, segment => !allowedFolderNames.Contains(segment)))
                        continue;
                    if (mode is null && Array.Exists(segments, segment => allowedFolderNames.Contains(segment)))
                        continue;

                    var relative = Path.GetRelativePath(sourceRoot, directory);
                    var targetDirectory = Path.Combine(targetRoot, relative);
                    pending.Push((directory, targetDirectory));
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                Debug.WriteLine($"Could not read directory '{source}': {ex}");
            }
        }
    }

    private static void WaitWhilePaused(MainViewmodel viewmodel)
    {
        while (viewmodel.Pause && !viewmodel.Cancel)
            Thread.Sleep(100);
    }

    private static bool IsSameOrChildPath(string candidate, string parent)
    {
        var relative = Path.GetRelativePath(parent, candidate);
        return relative == "." || (!Path.IsPathRooted(relative) && relative != ".." &&
                                    !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal));
    }
}
