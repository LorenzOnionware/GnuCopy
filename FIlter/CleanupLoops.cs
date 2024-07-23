using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class Filter
{
    public static List<string>FilterTask(List<string> Input, bool Format)
    {
        // Format = true; means that files get filtert
        bool? listing = IOC.Default.GetService<Settings>().Listingart;
        switch(listing)
        {
            case true:
                return White(Input, Format);
            case false:
                return Input; 
            default:
                return Black(Input, Format);
        }
    }  
    private static List<string> Black(List<string> Input, bool Format)
    {
        var ignore = Format ? IOC.Default.GetService<MainViewmodel>().Ignorefiles : IOC.Default.GetService<MainViewmodel>().Ignorefolder;
        List<string> Output = new();
        if(Format == true)
        {
            foreach(var file in Input)
            {
                if(!ignore.Contains(Path.GetExtension(file)))
                {
                    Output.Add(file);
                }
            }
        }
        else
        {
            foreach(var folder in Input)
            {
                if(!ignore.Contains(Path.GetFileName(Path.GetFileName(folder))))
                {
                    Output.Add(folder);
                }
            }
        }
        return Output;
    }
    private static List<string> White(List<string> Input, bool Format)
    {
        var ignore = Format ? IOC.Default.GetService<MainViewmodel>().Ignorefiles : IOC.Default.GetService<MainViewmodel>().Ignorefolder;
        List<string> Output = new();
        if(Format == true)
        {
            foreach(var file in Input)
            {
                if(ignore.Contains(Path.GetExtension(file)))
                {
                    Output.Add(file);
                }
            }
        }
        else
        {
            foreach(var folder in Input)
            {
                if(ignore.Contains(Path.GetFileName(Path.GetFileName(folder))))
                {
                    Output.Add(folder);
                }
            }
        }
        return Output;
    }
}