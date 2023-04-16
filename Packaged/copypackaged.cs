using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynamicData;
using HarfBuzzSharp;
using Project1.Viewmodels;
using static Project1.CleanupLoops;

namespace Project1;

public class copypackaged
{
    private static string TempFolder = Path.Combine(MainViewmodel.Default.Copyfromtext, "OnionwareTemp");
    private static string[] folders = new string[]{};
    public static async Task Copy(List<string> ignorefiles, List<string>ignorefolders)
    {
        Directory.CreateDirectory(TempFolder);
        var Folders = Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext);
        var Firstfiles = Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext);
        bool firstfilesarecopied =false;
        if (IOC.Default.GetService<Settings>().Listingart == null)
        {
            
        }else if (IOC.Default.GetService<Settings>().Listingart == false)
        {
            //blacklist
            ignorefolders.Add(TempFolder);
            folders = CleanupLoops.CLean(Folders.ToArray(),ignorefolders.ToArray());
            Firstfiles = CleanupLoops.CLean(Firstfiles.ToArray(), ignorefiles.ToArray());
            if (!firstfilesarecopied)
            {
                firstfilesarecopied = true;
                foreach (var value in Firstfiles)
                {
                    File.Copy(value,Path.Combine(TempFolder,Path.GetFileName(value)));
                }
            }
            foreach (var folder in folders)
            { 
                var files = CleanupLoops.CLean(Directory.GetFiles(folder).ToArray(),ignorefiles.ToArray()).ToList();
                int ab = MainViewmodel.Default.Copyfromtext.Count(c=> c == '\\');
               int i = 0;
               string path1="";
               foreach (var a in MainViewmodel.Default.Copyfromtext)
               {
                   if (a.ToString() =="\\" && i < ab)
                   {
                       i++;
                   }
                   else if(i==ab)
                   {
                       path1 += a;
                   }
               }
               string path2 = new string(path1.ToString());
               string[] value1 = folder.Split('\\');
               bool yes = false;
               int index = 0;
               string path = "";
               foreach (var va in value1)
               {
                   i++;
                   if (va == path2)
                   {
                       yes = true;
                   }else if (yes)
                   {
                       path += (va);
                   }
               }
               Directory.CreateDirectory(Path.Combine(TempFolder,new string(path.ToArray())));
               if (files.Count != 0)
               {
                   foreach (var value in files)
                   {
                       File.Copy(value,Path.Combine(TempFolder,new string(path.ToArray()),Path.GetFileName(value)));
                   }
               }
            }
        }
        else
        {
            //Whitelist
            ignorefolders.Add(TempFolder);
            folders = CleanupLoops.CLeanWhite(Folders.ToArray(),ignorefolders.ToArray());
            Firstfiles = CleanupLoops.CLeanWhite(Firstfiles.ToArray(), ignorefiles.ToArray());
            if (!firstfilesarecopied)
            {
                firstfilesarecopied = true;
                foreach (var value in Firstfiles)
                {
                    File.Copy(value,Path.Combine(TempFolder,Path.GetFileName(value)));
                }
            }
            foreach (var folder in folders)
            { 
                var files = CleanupLoops.CLeanWhite(Directory.GetFiles(folder).ToArray(),ignorefiles.ToArray()).ToList();
                int ab = MainViewmodel.Default.Copyfromtext.Count(c=> c == '\\');
               int i = 0;
               string path1="";
               foreach (var a in MainViewmodel.Default.Copyfromtext)
               {
                   if (a.ToString() =="\\" && i < ab)
                   {
                       i++;
                   }
                   else if(i==ab)
                   {
                       path1 += a;
                   }
               }
               string path2 = new string(path1.ToString());
               string[] value1 = folder.Split('\\');
               bool yes = false;
               int index = 0;
               string path = "";
               foreach (var va in value1)
               {
                   i++;
                   if (va == path2)
                   {
                       yes = true;
                   }else if (yes)
                   {
                       path += (va);
                   }
               }
               Directory.CreateDirectory(Path.Combine(TempFolder,new string(path.ToArray())));
               if (files.Count != 0)
               {
                   foreach (var value in files)
                   {
                       File.Copy(value,Path.Combine(TempFolder,new string(path.ToArray()),Path.GetFileName(value)));
                   }
               }
            }
        }
    }
}