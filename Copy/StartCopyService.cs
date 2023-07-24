using System;
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
    public async Task Start(CancellationToken token)
    {
        if (!Path.Exists(IOC.Default.GetService<MainViewmodel>().Copyfromtext) && !Path.Exists(IOC.Default.GetService<MainViewmodel>().Copytotext))
            return;
        if (IOC.Default.GetService<Settings>().Packageformat != 0)
        {
            goto A;
        }
        await Copy.Settings(false, token);
        MainViewmodel.Default.selectionchaged();
        return;
        A:;
        await CopyPack.Start(token);
    }
}