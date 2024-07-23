using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Project1.Viewmodels;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace Project1;

public class StartCopyService
{
    public async Task Start(CancellationTokenSource token, ObservableCollection<string> paths)
    {
        if (!Path.Exists(IOC.Default.GetService<MainViewmodel>().Copyfromtext) && !Path.Exists(IOC.Default.GetService<MainViewmodel>().Copytotext))
            return;
        if (IOC.Default.GetService<Settings>().Packageformat != 0)
        { 
            await CopyPack.Start(token);
        }
        else
        {
            await CopyMultiple.Start(paths);
        }
        MainViewmodel.Default.selectionchaged();
        return;
        
    }
}