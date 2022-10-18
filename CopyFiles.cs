using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Project1;

public class CopyFiles
{
    public static async Task CopyFile(string destfile, string[] files1, List<string> ignoreformats)
    {
        
        foreach (var a in files1)
        {
            bool go = false;
            string ab = Path.GetExtension(a);
            foreach (var b in ignoreformats)
            {
                if (b == ab)
                {
                    go = true;
                }
            }
            if (go == false)
            {
                string abc = "";
                byte io = 0;
                foreach (var ac in a.Reverse())
                {
                    if (ac.ToString() == @"\")
                    {
                        io++;
                        break;
                    }
                    if (ac.ToString() != @"\" && io == 0) 
                    {
                        abc += ac;
                    }

                }
                File.Copy(a, destfile+@"\"+new string(abc.Reverse().ToArray()));
            }
        }
    }
}