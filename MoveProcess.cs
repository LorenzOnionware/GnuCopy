using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using static Project1.MainWindow;
using System.IO;
using System.Linq;

namespace Project1;

public class MoveProcess
{
    public static async Task MoveToo(string mp, string mtp, List<string> value)
    {
        if (Directory.Exists(mp) || Directory.Exists(mtp))
        {
            if (Directory.GetDirectories(mp) == null)
            {
                string[] files = Directory.GetFiles(mp);
                foreach (string f in files)
                {
                    if (value.Any(f.Contains) == true)
                    {
                        continue;
                    }
                    else
                    {
                        string fileName = System.IO.Path.GetFileName(f);
                        string destFile = System.IO.Path.Combine(mtp, fileName);
                        File.Copy(f, destFile, true);
                    }
                }
            }
            else
            {
                string[] files = Directory.GetFiles(mp);
                foreach (string f in files)
                {
                    if (value.Any(f.Contains) == true)
                    {
                        continue;
                    }
                    else
                    {
                        string fileName = System.IO.Path.GetFileName(f);
                        string destFile = System.IO.Path.Combine(mtp, fileName);
                        File.Copy(f, destFile, true);
                    }
                }


                foreach (var i in Directory.GetDirectories(mp))
                {
                    bool ab = false;
                    string name = "";
                    foreach (char a in i.Reverse())
                    {
                        if (a.ToString().Contains(@"\") != true)
                        {
                            if (ab == false)
                            {
                                name += a;
                            }
                        }
                        else
                        {
                            ab = true;
                        }
                    }

                    string aa = "";
                    foreach (char b in name.Reverse())
                    {
                        aa += b;
                    }

                    string l1 = (mp + @"\" + aa);
                    string l = (mtp + @"\" + aa);
                    Directory.CreateDirectory(l);

                    string[] files2 = Directory.GetFiles(l1);
                    foreach (string f in files2)
                    {
                        if (value.Any(f.Contains) == true)
                        {
                            continue;
                        }
                        else
                        {
                            string fileName = System.IO.Path.GetFileName(f);
                            string destFile = System.IO.Path.Combine(l, fileName);
                            File.Copy(f, destFile, true);
                        }
                    }
                }
            }
        }
    }
}