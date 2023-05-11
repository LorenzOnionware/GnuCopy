using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Project1.Viewmodels;
using static Project1.MainWindow;

namespace Project1;

public class Copy
{
    public static async Task Settings()
    {
        if (IOC.Default.GetService<Settings>().Listingart ==true)
        {await White();
            
        }else if (IOC.Default.GetService<Settings>().Listingart == false)
        {
            await Black();
        }
        else
        {
            await Without();
        }
    }

    private static async Task Without()
    {
        List<string> firstfiles = Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext).ToList();
        foreach (var file in firstfiles)
        {
            try
            {
                File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
            catch (IOException e)
            {
                continue;
            }
        }

        List<string> folders = Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext).ToList();
        foreach (var folder in folders)
        {
            Directory.CreateDirectory(Path.Combine(MainViewmodel.Default.Copytotext, GetName(folder)));
            List<string> files = Directory.EnumerateFiles(folder).ToList();
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(folder),GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }

                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        }
    }
    private static async Task Black()
    {
        List<string> firstfiles = CleanupLoops.CLean(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray()).ToList();
        foreach (var file in firstfiles)
        {
            try
            {
                File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
            catch (IOException e)
            {
                continue;
            }
        }

        List<string> folders = CleanupLoops.CLean(Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray()).ToList();
        foreach (var folder in folders)
        {
            Directory.CreateDirectory(Path.Combine(MainViewmodel.Default.Copytotext, GetName(folder)));
            List<string> files = CleanupLoops.CLean(Directory.EnumerateFiles(folder).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray()).ToList();
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(folder),GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }

                IOC.Default.GetService<IProgressBarService>().Progress();
            }
        }
    }
    private static async Task White()
    {
        List<string> firstfiles = CleanupLoops.CLeanWhite(Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray()).ToList();
        foreach (var file in firstfiles)
        {
            try
            {
                File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                IOC.Default.GetService<IProgressBarService>().Progress();
            }
            catch (IOException e)
            {
                continue;
            }
        }

        List<string> folders = CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(MainViewmodel.Default.Copyfromtext).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefolder.ToArray()).ToList();
        foreach (var folder in folders)
        {
            Directory.CreateDirectory(Path.Combine(MainViewmodel.Default.Copytotext, GetName(folder)));
            List<string> files = CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folder).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray()).ToList();
            foreach (var file in files)
            {
                try
                {
                    File.Copy(file, Path.Combine(MainViewmodel.Default.Copytotext,GetName(folder),GetName(file)), overwrite: IOC.Default.GetService<Settings>().Overrite);
                }
                catch (IOException e)
                {
                    continue;
                }
            }
            IOC.Default.GetService<IProgressBarService>().Progress();
        }
    }


    private static string GetName(string s)
    {
        bool ab = false;
        string abc = "";
        foreach (var a in s.Reverse())
        {
            if (a.ToString() == "\\")
            {
                ab = true;
            }else if (ab == false)
            {
                abc += a;
            }
            
        }

        string result = new string(abc.Reverse().ToArray());
        return result;
    }
    
}