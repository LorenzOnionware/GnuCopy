using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using GnuCopy.Interfaces;
using Project1.Viewmodels;

namespace Project1.Services;

public class ProgressBarService : IProgressBarService
{
    private static int _pendingProgress;
    private static int _updateScheduled;
    private static string? _latestFile;

    public void AddProgress()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var viewmodel = IOC.Default.GetService<MainViewmodel>();
            viewmodel.Progress++;
            viewmodel.Actualise();
        });
    }

    public void ReportFile(string filePath)
    {
        Interlocked.Increment(ref _pendingProgress);
        Interlocked.Exchange(ref _latestFile, filePath);
        ScheduleFileUpdate();
    }

    public static void ResetProgressReports()
    {
        Interlocked.Exchange(ref _pendingProgress, 0);
        Interlocked.Exchange(ref _latestFile, null);
    }

    private static void ScheduleFileUpdate()
    {
        if (Interlocked.Exchange(ref _updateScheduled, 1) != 0)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            var completedFiles = Interlocked.Exchange(ref _pendingProgress, 0);
            var latestFile = Interlocked.Exchange(ref _latestFile, null);
            var viewmodel = IOC.Default.GetService<MainViewmodel>();
            viewmodel.Progress += completedFiles;
            if (latestFile is not null)
                viewmodel.currentfile = latestFile;
            viewmodel.Actualise();

            Volatile.Write(ref _updateScheduled, 0);
            if (Volatile.Read(ref _pendingProgress) > 0)
                ScheduleFileUpdate();
        });
    }

    public async Task MaxProgressAsync()
    {
        var viewmodel = IOC.Default.GetService<MainViewmodel>();
        ResetProgressReports();
        var sources = viewmodel.Expanderpaths.ToArray();
        var count = await Task.Run(() =>
        {
            var total = 0;
            foreach (var folder in sources)
            {
                if (!Directory.Exists(folder))
                    continue;

                var files = Directory.EnumerateFiles(folder, "*", new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                    IgnoreInaccessible = true
                }).ToList();
                total += Filter.FilterTask(files, format: true).Count;
            }

            return total;
        });

        await Dispatcher.UIThread.InvokeAsync(() => viewmodel.Progressmax = count);
    }
}
