using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1;

public class ReadPresets
{
    public static List<string> IndexObject(string e)
    {
        List<string> jsonfile = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(e));
        return jsonfile;
    }

    //true=files false=folder
    public static List<string> Splitt(bool b,string path)
    {
        var presetindex = IndexObject(path);
        List<string> returindex = new();
        if (b)
        {
            foreach (var a in presetindex)
            {
                if (!a.Contains('#'))
                {
                    returindex.Add(a);
                }
            }
        }
        else
        {
            foreach (var a in presetindex)
            {
                if (a.Contains('#'))
                {
                    returindex.Add(Regex.Replace(a,"#", ""));
                }
            }
        }
        return returindex;
    }
}