using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Project1.CreateDirectories;
namespace Project1;
using static Readsettings;

public class FirstFolderFiles
{
    public static async Task FirstFolder(string cf, string ct, List<string> a1, bool? setting)
    {
        foreach (var abc in cf.Reverse())
        {
            string[] MyArray;
            switch (setting)
            {
                case true :
                    MyArray = CleanupLoops.CLeanWhite(Directory.GetFiles(cf),a1.ToArray());
                    break;
                case false:
                    MyArray = CleanupLoops.CLean(Directory.GetFiles(cf),a1.ToArray());
                    break;
                case null:
                    MyArray = Directory.GetFiles(cf);
                    break;
            }
            foreach (var a in MyArray)
            {

                string value1234 = "";
                bool value123 = false;
                foreach (var b in a.Reverse())
                {
                    if (b.ToString() == @"\")
                    {
                        value123 = true;
                    }
                    else if (value123 != true)
                    {
                        value1234 += b;
                    }
                }

                string lel = new string(ct + @"\" + new string(value1234.Reverse().ToArray()));
                File.Copy(a, lel, overwrite: Read(0));
            }
        }
    }
}
