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
        IOC.Default.GetService<IProgressBarService>().ProgressMax();
        if (IOC.Default.GetService<Settings>().Packageformat != "none")
        {
            switch (IOC.Default.GetService<Settings>().Packageformat)
            {
                case "7Zip" :
                    await copypackaged.Copy(MainViewmodel.Default.ignorefiles,MainViewmodel.Default.ignorefolder);
                    await StartPackaging(true);
                    MainViewmodel.Default.selectionchaged();
                    break;
                case "Tar":
                    await copypackaged.Copy(MainViewmodel.Default.ignorefiles,MainViewmodel.Default.ignorefolder);
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
    private async Task StartPackaging(bool use7z)
    {
        using (var archive = ZipArchive.Create())
        {
            archive.AddAllFromDirectory(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"));
            archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext,$"Compresseddirection.{(use7z ? "7z" : "tar")}"), use7z ? CompressionType.LZMA : CompressionType.None);
        }
        Directory.Delete(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"),true);
    }
    private async Task CopyUnPackaged()
    {
        bool settingschaged = false;
        if (Path.Exists(MainViewmodel.Default.Copyfromtext) && Path.Exists(MainViewmodel.Default.Copytotext))
        {
            await Copy.Settings();
        }
    }
    
}