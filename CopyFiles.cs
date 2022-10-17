using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;
using System.Linq;
namespace Project1;

public class CopyFiles
{
    public static async Task CopyFile(string destfile, string[] files1, List<string> ignoreformats)
    {
        foreach (var a in files1)
        {
            if (ignoreformats.Contains(Path.GetExtension(a)) != true)
            {
                File.Copy(a, destfile);
            }
        }

    }
}