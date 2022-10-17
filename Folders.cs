using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Project1.MainWindow;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static Project1.Search;
using static Project1.CopyFiles;
namespace Project1;

public class Folders
{
    public static async Task CopyFolder(string cf, string ct, List<string> a1)
    {
        var folders = Directory.EnumerateDirectories(cf, "*", SearchOption.AllDirectories);
        int counter = 0;
        while(counter <= folders.Count())
        {
            
            var array1 = folders.ToArray();
            string datapath = array1[counter];
            string[] currentfiles = Directory.GetFiles(datapath); 
            int foldercount = 0;
            int foldercountofct = 0;
            foreach (var a in datapath)
            {
                if (a.ToString() == @"\")
                {
                    foldercount++;
                }
            }
            foreach (var b in ct)
            {
                if (b.ToString() == @"\")
                {
                    foldercountofct++;
                }
            }

            if (foldercount > foldercountofct)
            {
                int forpath = foldercount - foldercountofct;
                int value1 = 0;
                string value2 = "";
                foreach (char value in datapath.Reverse())
                {
                    if (value.ToString() == @"\")
                    {
                        if (value1 < forpath)
                        {
                            value1++;
                            value2 += value;
                        }
                    }
                    else
                    {
                        if (value1 < forpath)
                        {
                            value2 += value;
                        }
                    }
                }
                string movepath = new string (datapath +@"\" + (value2.Reverse()));
                Directory.CreateDirectory(movepath);
                await CopyFile(movepath, currentfiles, a1);
            }else if (foldercount < foldercountofct)
            {
                
            }else if (foldercount == foldercountofct)
            {

            }

            counter++;
        }
    }
}