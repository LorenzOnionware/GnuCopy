using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Project1.Viewmodels;
using static Project1.CreateDirectories;
using static Project1.FirstFolderFiles;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;


namespace Project1;

public class Files : ObservableObject
{
    public static List<string> paths = new();
    public static List<string> folders1 = new();
    public static async Task CopyFolder(string cf, string ct, List<string> a1, List<string> ignore)
    {
        var folders = Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories);

        foreach (string value in folders)
        { 
            await DirectoriesCreate(value,ct, ignore);
        }
        int i = 0;
        await FirstFolder(cf, ct, a1);
        int count = folders1.ToArray().Length -1;
        while(count >= i)
        {
            
            var array1 = folders1.ToArray();
            var array2 = paths.ToArray();
            string datapath = array1[i];
            string datapath2 = array2[i];
            string[] currentfiles = Directory.GetFiles(datapath);
            foreach (var file in currentfiles)
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
                        File.Copy(file, datapath2 + new string(value2.Reverse().ToArray()), overwrite: true);
                    }
                    catch(System.IO.IOException ex)
                    {
                        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(
                            new MessageBoxStandardParams
                            {
                                ButtonDefinitions = (ButtonEnum)messageboxbuttons.ok,
                                ContentTitle = "Error",
                                ContentHeader = "Something went wrong by copying files",
                                ContentMessage = ex.ToString() + "Contact: contact.onionware@gmail.com ",
                                Icon = Icon.Error,
                                Style = Style.UbuntuLinux
                            });
                        messageBoxStandardWindow.Show();
                    }
                }
            }
            MainViewmodel.Default.Progressvalue++;
            i++;
        }
        MainViewmodel.Default.Progressvalue = 0;
    }
}
