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

    public static List<string> paths = new();
    public static List<string> folders1 = new();

    public static async Task CopyFolder(string cf, string ct, List<string> a1, List<string> ignore)
    {
        string[] array1 = CleanupLoops.CLean(folders1.ToArray(), ignore.ToArray());
        string[] array2 = CleanupLoops.CLean(paths.ToArray(), a1.ToArray());
        var folders = CleanupLoops.CLean(Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories).ToArray(),ignore.ToArray());
        foreach (string value in folders)
        {
            string pattern = @"^(.*\\)([^\\]*)$";

            Match match = Regex.Match(value, pattern);

            if (match.Success)
            {
                string file = match.Groups[2].Value;
                if (!ignore.Contains(file))
                {
                    if (!value.Contains(ignore.Any()))
                    {
                          await DirectoriesCreate(value, ct, ignore);
                    }
                }
            }
        }

        int i = 0;
        await FirstFolder(cf, ct, a1, ignore);
        int count = array1.Length - 1;
        while (count >= i)
        {
            string datapath = array1[i];
            string datapath2 = array2[i];
            string[] currentfiles = Directory.GetFiles(datapath);
            foreach (var file in currentfiles)
            {   
                if(itemlistfirstfolder.FirstFolderFileList.Contains(Path.GetFileName(file)))
                {continue;}
                else
                {


                    bool ab = false;
                    foreach (var a in a1)
                    {
                        if (Path.GetExtension(file) == a)
                        {
                            ab = true;
                        }
                    }

                    if (ab == true)
                    {
                        ab = false;
                    }
                    else
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
                            File.Copy(file, datapath2 + new string(value2.Reverse().ToArray()),overwrite: Readsettings.Read(0));
                        }
                        catch (IOException e)
                        {
                            continue;
                        }
                    }
                }
            }
            MainViewmodel.Default.Progressvalue++;
            i++;
        }
        MainViewmodel.Default.Progressvalue = 0;
    }
}
