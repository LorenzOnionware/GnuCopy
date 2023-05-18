using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        if (IOC.Default.GetService<Settings>().Packageformat != 0)
        {
            switch (IOC.Default.GetService<Settings>().Packageformat)
            {
                case 1 :
                    //7Zip
                    await Project1.Copy.Settings(true);
                    await StartPackaging(true);
                    MainViewmodel.Default.selectionchaged();
                    break;
                case 2:
                    //Tar
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
    private async Task StartPackaging(bool use7z)
    {
        using (var archive = ZipArchive.Create())
        {
            archive.AddAllFromDirectory(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"));
            var a = DateTime.Now.ToString("g").Replace(':','-');
            var b = String.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName) ? "Compresstdirection" : IOC.Default.GetService<Settings>().ZipName;
            if (IOC.Default.GetService<Settings>().DateAsName)
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext,$"{a}.{(use7z ? "7z" : "tar")}"), use7z ? CompressionType.LZMA : CompressionType.None);
            else
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext,$"{b}.{(use7z ? "7z" : "tar")}"), use7z ? CompressionType.LZMA : CompressionType.None);
            
            
        }
        Directory.Delete(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"),true);
    }
    private async Task CopyUnPackaged()
    {
        bool settingschaged = false;
        if (Path.Exists(MainViewmodel.Default.Copyfromtext) && Path.Exists(MainViewmodel.Default.Copytotext))
        {
            await Copy.Settings(false);
        }
    }
    
}