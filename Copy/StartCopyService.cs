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
        if (!System.IO.Path.Exists(IOC.Default.GetService<MainViewmodel>().Copyfromtext) && !System.IO.Path.Exists(IOC.Default.GetService<MainViewmodel>().Copytotext))
            return;
        if (IOC.Default.GetService<Settings>().Packageformat != 0)
        {
            switch (IOC.Default.GetService<Settings>().Packageformat)
            {
                case 1:
                    //Tar
                    await Project1.Copy.Settings(true,token);
                    await StartPackaging(true,token);
                    MainViewmodel.Default.selectionchaged();
                    break;
                case 2:
                    //TZip
                    await Project1.Copy.Settings(true,token);
                    await StartPackaging(false,token);
                    MainViewmodel.Default.selectionchaged();
                    break;
            }
        }
        else
        {
            await CopyUnPackaged(token);
        }
    }
    private async Task StartPackaging(bool useTar,CancellationToken token)
    {
        var Temp = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");

        using (var archive = ZipArchive.Create())
        {
            var namee = IOC.Default.GetService<MainViewmodel>().Copyfromtext;
            var abb = Path.GetDirectoryName(namee);
            archive.AddAllFromDirectory(Temp);
            var a = DateTime.Now.ToString("g").Replace(':','-');
            var b = String.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName) ? abb : IOC.Default.GetService<Settings>().ZipName;

            if (IOC.Default.GetService<Settings>().DateAsName)
            {
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext, $"{a}.{(useTar ? "tar" : "zip")}"), useTar?CompressionType.None: CompressionType.Deflate);
            }
            else
            {

                archive.SaveTo(
                    Path.Combine(MainViewmodel.Default.Copytotext, $"{b}.{(useTar ? "tar" : "zip")}"), useTar?CompressionType.None: CompressionType.Deflate);
            }
        }

        Directory.Delete(Temp,recursive:true);
        MainViewmodel.Default.Progress += 10;
    }

    private async Task CopyUnPackaged(CancellationToken token)
    {
        if (Path.Exists(MainViewmodel.Default.Copyfromtext) && Path.Exists(MainViewmodel.Default.Copytotext))
        {
            await Copy.Settings(false,token);
        }
    }
    
}