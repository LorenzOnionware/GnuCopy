using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Project1.Viewmodels;
using static Project1.Files;
namespace Project1;

public class CreateDirectories
{
    public static async Task DirectoriesCreate(string pathfrom, string pathto, List<string> ignore)
    {
        await FolderCreate(pathto, pathfrom);
    }
    private static async Task FolderCreate(string pathto,string pathfrom)
    {
        string movepath;
        int countofpathfrom = countofpathfrom1(pathfrom);
        int countofpathto = countofpathto1(pathto);
        if (countofpathfrom < countofpathto)
        {
            byte count = 0;
            int differenz = countofpathto - countofpathfrom;
            string peaceofpath = "";
            foreach (var value in pathfrom.Reverse())
            {
                if (value.ToString() == @"\")
                {
                    count++;
                    peaceofpath += value;
                }
                else if (count < differenz)
                    peaceofpath += value;
            }

            movepath = (pathto + new string(peaceofpath.Reverse().ToArray()));
            Directory.CreateDirectory(movepath);
            paths.Add(movepath);
            folders1.Add(pathfrom);
        }
        else if (countofpathfrom > countofpathto)
        {
            byte count = 0;
            int differenz = countofpathfrom - countofpathto;
            string peaceofpath = "";
            foreach (var value in pathfrom.Reverse())
            {
                if (value.ToString() == @"\")
                {
                    count++;
                    if (count <= differenz)
                    {
                        peaceofpath += value;
                    }
                }
                else if (count < differenz)
                    peaceofpath += value;
            }

            movepath = (pathto + new string(peaceofpath.Reverse().ToArray()));
            Directory.CreateDirectory(movepath);
            paths.Add(movepath);
            folders1.Add(pathfrom);
        }
        else
        {
            string peaceofpath = "";
            bool ab = false;
            foreach (var value in pathfrom.Reverse())
            {
                if (value.ToString() == @"\")
                {
                    peaceofpath += value;
                    ab = true;
                }
                else if (ab == false)
                    peaceofpath += value;
            }

            movepath = (pathto + new string(peaceofpath.Reverse().ToArray()));
            Directory.CreateDirectory(movepath);
            paths.Add(movepath);
            folders1.Add(pathfrom);
        }
    }
    
    

    #region ints

    public static int countofpathfrom1(string pathfrom)
    {
        int aa = 0;
        foreach (var a in pathfrom)
        {
            if (a.ToString() == @"\")
            {
                aa++;
            }
        }

        return aa;
    }

    public static int countofpathto1(string pathto)
    {
        int aa = 0;
        foreach (var a in pathto)
        {
            if (a.ToString() == @"\")
            {
                aa++;
            }
        }

        return aa;
    }

    #endregion
}