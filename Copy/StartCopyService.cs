using System;
using System.IO;
using System.Threading.Tasks;
using Project1.Viewmodels;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace Project1;

public class StartCopyService
{
    public async Task Start()
    {
        if (!System.IO.Path.Exists(IOC.Default.GetService<MainViewmodel>().Copyfromtext) && !System.IO.Path.Exists(IOC.Default.GetService<MainViewmodel>().Copytotext))
            return;
        if (IOC.Default.GetService<Settings>().Packageformat != 0)
        {
            switch (IOC.Default.GetService<Settings>().Packageformat)
            {
                case 1:
                    //Tar
                    await Project1.Copy.Settings(true);
                    await StartPackaging(true);
                    MainViewmodel.Default.selectionchaged();
                    break;
                case 2:
                    //TZip
                    await Project1.Copy.Settings(true);
                    await StartPackaging(false);
                    MainViewmodel.Default.selectionchaged();
                    break;
            }
        }
        else
        {
            await CopyUnPackaged();
        }
    }
    private async Task StartPackaging(bool useTar)
    {
        var Temp = Path.Combine(String.IsNullOrEmpty(IOC.Default.GetService<Settings>().TempfolderPath)? MainViewmodel.Default.Copyfromtext: IOC.Default.GetService<Settings>().TempfolderPath, "OnionwareTemp");

        using (var archive = ZipArchive.Create())
        {
            archive.AddAllFromDirectory(Temp);
            var a = DateTime.Now.ToString("g").Replace(':','-');
            var b = String.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName) ? "Compresstdirection" : IOC.Default.GetService<Settings>().ZipName;

            if (IOC.Default.GetService<Settings>().DateAsName)
            {
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext, $"{a}.{(useTar ? "tar" : "zip")}"),CompressionType.None);
            }
            else
            {

                archive.SaveTo(
                    Path.Combine(MainViewmodel.Default.Copytotext, $"{b}.{(useTar ? "tar" : "zip")}"),CompressionType.None);
            }
        }

        Directory.Delete(Temp,recursive:true);
        MainViewmodel.Default.Progress += 10;
    }

    private async Task CopyUnPackaged()
    {
        if (Path.Exists(MainViewmodel.Default.Copyfromtext) && Path.Exists(MainViewmodel.Default.Copytotext))
        {
            await Copy.Settings(false);
        }
    }
    
}