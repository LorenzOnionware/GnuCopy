using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.Core;
using Project1.Viewmodels;
using static Project1.CreateDirectories;
using static Project1.FirstFolderFiles;


namespace Project1;

public class Files : ObservableObject
{
    //Damn after 5 month Im editing this piece of sh**, and i cant belive that i wrote this!!
    public static bool? setting;

    public static List<string> paths = new();
    public static List<string> folders1 = new();
    
    public static string[] array1;
    public static string[] array2;
    public static string[] folders;
    public static async Task CopyFolder(string cf, string ct, List<string> a1, List<string?> ignore)
    {
        if (Readsettings.Read1((3)) >= 2)
        {
            setting = null;
            folders = Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories).ToArray();
        }else if (Readsettings.Read1(3) == 0)
        {
            setting = false;
            folders = CleanupLoops.CLean(Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories).ToArray(),ignore.ToArray());
        }
        else
        {
            setting = true;
            folders = CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories).ToArray(),ignore.ToArray());
        }
        
        foreach (string value in folders)
        {
            string pattern = @"^(.*\\)([^\\]*)$";

            Match match = Regex.Match(value, pattern);

            if (match.Success)
            {
                string file = match.Groups[2].Value;
                await DirectoriesCreate(value, ct);
            }
        }
        if (Readsettings.Read1((3)) >= 2)
        {
            array2 = paths.ToArray();
            array1 = folders1.ToArray();
        }else if (Readsettings.Read1(3) == 0)
        {
            array1 = CleanupLoops.CLean(folders1.ToArray(), ignore.ToArray()); 
            array2 = CleanupLoops.CLean(paths.ToArray(), a1.ToArray());
        }
        else
        {
            array1 = CleanupLoops.CLeanWhite(folders1.ToArray(), ignore.ToArray()); 
            array2 = CleanupLoops.CLeanWhite(paths.ToArray(), ignore.ToArray());
        }

        int i = 0;
        await FirstFolder(cf, ct, a1, setting);
        int count = array1.Length - 1;
        while (count >= i)
        {
            string datapath = array1[i];
            string datapath2 = array2[i];
            string[] currentfiles;
            switch (setting)
            {
                case true :
                    currentfiles = CleanupLoops.CLeanWhite(Directory.GetFiles(datapath),a1.ToArray());
                    break;
                case false:
                    currentfiles = CleanupLoops.CLean(Directory.GetFiles(datapath),a1.ToArray());
                    break;
                case null:
                    currentfiles = Directory.GetFiles(datapath);
                    break;
            }
            foreach (var file in currentfiles)
            {   
          
                    string value2 = "";
                    bool ab2 = false;
                    foreach (var b in file.Reverse())
                    {
                        if (b.ToString() == @"\")
                        {
                            if (ab2 == false)
                            {
                                value2 += b;
                                ab2 = true;

                            }

                        }
                        else
                        {
                            if (ab2 == false)
                            {
                                value2 += b;
                            }
                        }
                    }

                    try
                    {
                        File.Copy(file, datapath2 + new string(value2.Reverse().ToArray()), overwrite: Readsettings.Read(0));
                    }
                    catch (IOException e)
                    {
                        continue;
                    }
            }
            MainViewmodel.Default.Progressvalue++;
            i++;
        }
        MainViewmodel.Default.Progressvalue = 0;
    }
}
