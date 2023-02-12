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
}