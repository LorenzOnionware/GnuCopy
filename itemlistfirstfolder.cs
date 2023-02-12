using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project1.Viewmodels;

namespace Project1;

public class itemlistfirstfolder:MainWindow
{ 
    private static List<string> lel => Directory.GetFiles(MainViewmodel.Default.Copytotext).ToList();
    public static List<string> FirstFolderFileList => lel.Select(p => Path.GetFileName(p)).ToList();
}