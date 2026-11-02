using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project1.Viewmodels;

namespace Project1;

public static class Filter
{
    /// <summary>
    /// Applies the configured file or folder filter. Format is true for files and false for folders.
    /// </summary>
    public static List<string> FilterTask(List<string> input, bool format)
    {
        ArgumentNullException.ThrowIfNull(input);

        var listing = IOC.Default.GetService<Settings>().Listingart;
        if (listing is null or false)
            return listing == false ? input : FilterItems(input, format, includeMatches: false);

        return FilterItems(input, format, includeMatches: true);
    }

    private static List<string> FilterItems(List<string> input, bool format, bool includeMatches)
    {
        var viewmodel = IOC.Default.GetService<MainViewmodel>();
        var configuredItems = format ? viewmodel.Ignorefiles : viewmodel.Ignorefolder;
        var filter = new HashSet<string>(configuredItems, StringComparer.OrdinalIgnoreCase);

        return input.Where(path =>
        {
            var value = format
                ? Path.GetExtension(path)
                : Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
            var matches = filter.Contains(value);
            return matches == includeMatches;
        }).ToList();
    }
}
