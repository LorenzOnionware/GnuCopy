using System.Collections.Generic;
using System.Linq;
using DynamicData;
using FluentAvalonia.Core;

namespace Project1;

public class CleanupLoops
{
    public static string[] CLean(string[] arraytoclean, string[] cleanfromat)
    { 
        bool allow = false;
        List<string> output = new();
        foreach (var element in arraytoclean)
        {
            allow = false;
            foreach (var value in cleanfromat)
            {
                if (element.Contains(value))
                {
                    allow = true;
                }
            }
            if (allow == false)
            {
                output.Add(element);
            }
        }

        return output.ToArray();
    }
}