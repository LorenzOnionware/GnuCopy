using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Project1.CreateDirectories;
namespace Project1;
using static Readsettings;

public class FirstFolderFiles
{
    public static async Task FirstFolder(string cf, string ct, List<string> a1, List<string> ignorefolders)
    {

        foreach (var abc in cf.Reverse())
        {
            string[] MyArray = Directory.GetFiles(cf);
            foreach (var a in MyArray)
            {
                if(itemlistfirstfolder.FirstFolderFileList.Contains(Path.GetFileName(a)))
                {continue;}
                else
                {
                    bool abcd = false;
                    foreach (var b in a1)
                    {
                        if (b == Path.GetExtension(a))
                        {
                            abcd = true;
                        }
                    }

                    if (abcd != true)
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
    }
}
