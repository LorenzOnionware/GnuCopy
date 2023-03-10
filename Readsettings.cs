using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Project1.Viewmodels;

namespace Project1;

public class Readsettings
{ 
    public static bool Read(byte settingsindex)
    {
        bool index = false;
        string[] Jsonfile = (JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy\Settings\Settings.json")));
        string pattern = "=(.*)$";
        Match match = Regex.Match(Jsonfile[settingsindex], pattern);
        if (match.Success)
        {
            string value = match.Groups[1].Value;
            Console.WriteLine(value);
            switch (value)
            {
                case "true":
                    index = true;
                    break;
                case "false":
                    index = false;
                    break;
            }
        }
        return index;
    }
    
    public static byte Read1(byte index)
    {
        byte index2 = 0;

        string[] Jsonfile2 = (JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy\Settings\Settings.json")));
        string pattern2 = "=(.*)$";
        Match match2 = Regex.Match(Jsonfile2[index], pattern2);
        if (match2.Success)
        {
            string value = match2.Groups[1].Value;
            Console.WriteLine(value);
            switch (value)
            {
                case "0":
                    index2 = 0;
                    break;
                case "1":
                    index2 = 1;
                    break;
                case "2":
                    index2 = 2;
                    break;
            }
        }

        return index2;
    }
}