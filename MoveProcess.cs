using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using static Project1.MainWindow;
using System.IO;
using System.Linq;
using static Project1.CopyFiles;
using static Project1.Folders;

namespace Project1;

public class MoveProcess
{
    public static async Task MoveToo(string cf, string ct, List<string> ignoreformats)
    {
        string[] files1 = Directory.GetFiles(cf);
        await CopyFolder(cf, ct, ignoreformats);
        await CopyFile(ct, files1, ignoreformats);
    }
}