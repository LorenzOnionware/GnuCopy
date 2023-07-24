using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project1;

public class CleanupLoops
{
    public static async Task<string[]> CLean(string[] arraytoclean, string[] cleanfromat,bool folder,CancellationToken token)
    {
        int chunksize = 3;
        List<string> output = new List<string>();

        var chunks = arraytoclean.Chunk(chunksize).ToList();
        await Task.Run(() =>
        {
            Parallel.ForEach(chunks, chunk =>
            {
                foreach (var element in chunk)
                {
                    bool allow = false;
                    if (folder)
                    {
                        foreach (var value in cleanfromat)
                        {
                            if (GetName(element) == value)
                            {
                                allow = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var value in cleanfromat)
                        {
                            if (element.Contains(value))
                            {
                                allow = true;
                            }
                        }
                    }

                    if (!allow)
                    {
                        output.Add(element);
                    }
                }
            });
        },token);

        return output.ToArray();
    }

    public static async Task<string[]> CLeanWhite(string[] arraytoclean, string[] cleanfromat,bool folder,CancellationToken token)
    {
        int chunksize = 3;
        List<string> output = new List<string>();

        var chunks = arraytoclean.Chunk(chunksize).ToList();
        await Task.Run(() =>
        {
            Parallel.ForEach(chunks, chunk =>
            {
                foreach (var element in chunk)
                {
                    bool allow = false;
                    allow = false;
                    foreach (var value in cleanfromat)
                    {
                        if (folder)
                        {
                            if (GetName(element) == value)
                            {
                                allow = true;
                            }
                        }
                        else
                        {
                            if (element.Contains(value))
                            {
                                allow = true;
                            }
                        }
                    }

                    if (allow)
                    {
                        output.Add(element);
                    }
                }

            });
        },token);
        return output.ToArray();
    }

    private static string GetName(string element)
    {
        string output = string.Empty;
        int lastIndex = element.LastIndexOf("\\");
        if (lastIndex >= 0 && lastIndex < element.Length - 1)
        {
            output = element.Substring(lastIndex + 1);
        }

        return output;
    }
}