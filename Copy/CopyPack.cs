using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using DynamicData;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Project1.Viewmodels;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;
using Path = System.IO.Path;

namespace Project1;

public class CopyPack
{
    public static async Task Start(CancellationToken token)
    {
        bool multiple = IOC.Default.GetService<Settings>().MultipleSources;
        await Task.Run(async () =>
        {
            if (multiple)
            {
                IOC.Default.GetService<IProgressBarService>().Progressmax(token,true);
                HashSet<string> folder = IOC.Default.GetService<MainViewmodel>().Expanderpaths.ToHashSet();
                if (IOC.Default.GetService<Settings>().Listingart == false)
                {
                     //ALL
                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    var target1 =target+(IOC.Default.GetService<Settings>().Packageformat == 1 ? ".tar" : ".zip");
                    FileStream fs = new FileStream(target1, FileMode.Create);
                    var zip = new ZipWriter(fs, new ZipWriterOptions(IOC.Default.GetService<Settings>().Packageformat == 1 ? CompressionType.None : CompressionType.Deflate));
                   
                    foreach (var Source in folder) 
                    {
                        if (!Directory.EnumerateFiles(Source).Any())
                        {
                            IOC.Default.GetService<MainViewmodel>().progressmax++;
                            IOC.Default.GetService<MainViewmodel>().progressmax2++;
                            var fss = new FileStream(Path.Combine(Source, @"onionfile.ow"), FileMode.Create);
                            fss.Dispose();
                        }
                        var a = GetLastPartFromPath(Source);
                        var ffiles= Directory.EnumerateFiles(Source, "*", new EnumerationOptions() { AttributesToSkip = FileAttributes.System | FileAttributes.Hidden });
                        foreach (var file in ffiles)
                        {
                            
                            zip.Write(a+"/"+Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        var folders = Directory.EnumerateDirectories(Source, "*", new EnumerationOptions()
                        {
                            RecurseSubdirectories = true, AttributesToSkip = FileAttributes.System | FileAttributes.Hidden 
                        });
                        foreach (var foldeer in folders)
                        {
                            List<string> empty= new();
                            empty.Replace(Directory.EnumerateFiles(foldeer).ToArray());
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fss = new FileStream(Path.Combine(foldeer, @"onionfile.ow"), FileMode.Create);
                                fss.Dispose();
                                empty.Add((Path.Combine(foldeer, @"onionfile.ow")));
                            }

                            string path = foldeer.Replace(Source, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{a}{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                        
                    }
                    zip.Dispose();
                    fs.Dispose();
                }else if (IOC.Default.GetService<Settings>().Listingart == true)
                {
                    //White
                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    var target1 =target+(IOC.Default.GetService<Settings>().Packageformat == 1 ? ".tar" : ".zip");
                    FileStream fs = new FileStream(target1, FileMode.Create);
                    var zip = new ZipWriter(fs, new ZipWriterOptions(IOC.Default.GetService<Settings>().Packageformat == 1 ? CompressionType.None : CompressionType.Deflate));
                   
                    foreach (var Source in folder)
                    {  
                        var ignorfiles =IOC.Default.GetService<MainViewmodel>().ignorefiles;
                        ignorfiles.Add(".ow");
                        var aaa = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Source).ToArray(),
                            ignorfiles.ToArray(), false, token);
                        if (!aaa.Any())
                        {
                            emptyy(Source);
                        }
                        var a = GetLastPartFromPath(Source);
                        var ffiles= await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Source, "*", new EnumerationOptions() { AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray(), ignorfiles.ToArray(), false, token);
                        foreach (var file in ffiles)
                        {
                            zip.Write(a+"/"+Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        var folders = await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(Source, "*", new EnumerationOptions()
                        { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray(), MainViewmodel.Default.ignorefolder.ToArray(), true, token);
                        
                        foreach (var foldeer in folders)
                        {
                            List<string> empty= new();
                            empty.Replace(await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(foldeer).ToArray(),ignorfiles.ToArray(),false,token));
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fss = new FileStream(Path.Combine(foldeer, @"onionfile.ow"), FileMode.Create);
                                fss.Dispose();
                                empty.Add((Path.Combine(foldeer, @"onionfile.ow")));
                            }

                            string path = foldeer.Replace(Source, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{a}{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                        
                    }
                    zip.Dispose();
                    fs.Dispose();
                }
                else
                {
                    //BLACK
                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    var target1 = target + (IOC.Default.GetService<Settings>().Packageformat == 1 ? ".tar" : ".zip");
                    FileStream fs = new FileStream(target1, FileMode.Create);
                    var zip = new ZipWriter(fs,
                        new ZipWriterOptions(IOC.Default.GetService<Settings>().Packageformat == 1
                            ? CompressionType.None
                            : CompressionType.Deflate));

                    foreach (var Source in folder)
                    {
                        IOC.Default.GetService<MainViewmodel>().ignorefiles.Add(".ow");
                        var aaa = await CleanupLoops.CLean(Directory.EnumerateFiles(Source).ToArray(),
                            IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(), false, token);
                        if (!aaa.Any())
                        {
                            IOC.Default.GetService<MainViewmodel>().progressmax++;
                            IOC.Default.GetService<MainViewmodel>().progressmax2++;
                            var fss = new FileStream(Path.Combine(Source, @"onionfile.ow"), FileMode.Create);
                            fss.Dispose();
                        }
                        var a = GetLastPartFromPath(Source);
                        var ffiles = await CleanupLoops.CLean(
                            Directory.EnumerateFiles(Source, "*",
                                new EnumerationOptions()
                                    { AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray(),
                            MainViewmodel.Default.ignorefiles.ToArray(), false, token);
                        foreach (var file in ffiles)
                        {

                            zip.Write(a + "/" + Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        var folders = await CleanupLoops.CLean(Directory.EnumerateDirectories(Source, "*",
                            new EnumerationOptions()
                            {
                                RecurseSubdirectories = true,
                                AttributesToSkip = FileAttributes.System | FileAttributes.Hidden
                            }).ToArray(), MainViewmodel.Default.ignorefolder.ToArray(), true, token);

                        foreach (var foldeer in folders)
                        {
                            List<string> empty = new();
                            empty.Replace(await CleanupLoops.CLean(Directory.EnumerateFiles(foldeer).ToArray(),IOC.Default.GetService<MainViewmodel>().ignorefiles.ToArray(),false,token));
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fss = new FileStream(Path.Combine(foldeer, @"onionfile.ow"), FileMode.Create);
                                fss.Dispose();
                                empty.Add((Path.Combine(foldeer, @"onionfile.ow")));
                            }

                            string path = foldeer.Replace(Source, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{a}{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                    }
                    zip.Dispose();
                    fs.Dispose();
                }
            }
            else
            {
                IOC.Default.GetService<IProgressBarService>().Progressmax(token,true);
                string Source = MainViewmodel.Default.Copyfromtext;
                HashSet<string> folder = new HashSet<string>();

                #region Listing

                if (IOC.Default.GetService<Settings>().Listingart == false)
                {
                    //all
                    #region vorbereitung

                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    byte format = (byte)IOC.Default.GetService<Settings>().Packageformat;
                    var target1 =target+(format == 1 ? ".tar" : ".zip");

                    #endregion
                    #region enumerationen/Clean

                    folder.Replace(Directory.EnumerateDirectories(Source, "*", new EnumerationOptions() { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray());
                    

                    #endregion
                    #region pack

                    using (var fileStream = new FileStream(target1, FileMode.Create))
                    {
                        var zip = new ZipWriter(fileStream, new ZipWriterOptions(format == 1 ? CompressionType.None : CompressionType.Deflate));
                        var a = Directory.EnumerateFiles(Source, "*",new EnumerationOptions() { RecurseSubdirectories = false }).ToArray();
                        foreach (var file in a)
                        {
                            zip.Write(Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        foreach (var folders in folder)
                        {
                            List<string> empty = new();
                            empty.Replace(Directory.EnumerateFiles(folders).ToArray());
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fs = new FileStream(Path.Combine(folders, @"onionfile.ow"), FileMode.Create);
                                fs.Dispose();
                                empty.Add((Path.Combine(folders, @"onionfile.ow")));
                            }

                            string path = folders.Replace(IOC.Default.GetService<MainViewmodel>().Copyfromtext, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                        zip.Dispose();
                    }

                    #endregion
                }
                else if (IOC.Default.GetService<Settings>().Listingart == true)
                {
                    //white

                    #region vorbereitung

                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    byte format = (byte)IOC.Default.GetService<Settings>().Packageformat;
                    var target1 =target+(format == 1 ? ".tar" : ".zip");

                    #endregion
                    #region enumerationen/Clean

                    folder.Replace(await CleanupLoops.CLeanWhite(Directory.EnumerateDirectories(Source, "*", new EnumerationOptions() { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray(), MainViewmodel.Default.ignorefolder.ToArray(), true, token));
                    

                    #endregion
                    #region pack

                    using (var fileStream = new FileStream(target1, FileMode.Create))
                    {
                        var zip = new ZipWriter(fileStream, new ZipWriterOptions(format == 1 ? CompressionType.None : CompressionType.Deflate));
                        var a = await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(Source, "*",new EnumerationOptions() { RecurseSubdirectories = false }).ToArray(), IOC.Default.GetService<MainViewmodel>().Ignorefiles.ToArray(), false, token);
                        foreach (var file in a)
                        {
                            zip.Write(Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        foreach (var folders in folder)
                        {
                           List<string> empty= new(); 
                           empty.Replace(await CleanupLoops.CLeanWhite(Directory.EnumerateFiles(folders).ToArray(),
                                IOC.Default.GetService<MainViewmodel>().Ignorefiles.ToArray(), false, token));
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fs = new FileStream(Path.Combine(folders, @"onionfile.ow"), FileMode.Create);
                                fs.Dispose();
                                empty.Add((Path.Combine(folders, @"onionfile.ow")));
                            }

                            string path = folders.Replace(IOC.Default.GetService<MainViewmodel>().Copyfromtext, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                        zip.Dispose();
                    }

                    #endregion
                }
                else
                {
                    //black

                    #region vorbereitung

                    string target = Path.Combine(MainViewmodel.Default.Copytotext,
                        IOC.Default.GetService<Settings>().DateAsName
                            ? DateTime.Now.ToString("g").Replace(':', '-')
                            : (string.IsNullOrEmpty(IOC.Default.GetService<Settings>().ZipName)
                                ? "CompressedDirection"
                                : IOC.Default.GetService<Settings>().ZipName));
                    byte format = (byte)IOC.Default.GetService<Settings>().Packageformat;
                    var target1 =target+(format == 1 ? ".tar" : ".zip");

                    #endregion
                    #region enumerationen/Clean
                    folder.Replace(await CleanupLoops.CLean(Directory.EnumerateDirectories(Source, "*", new EnumerationOptions() { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.System | FileAttributes.Hidden }).ToArray(), MainViewmodel.Default.ignorefolder.ToArray(), true, token));
                    #endregion
                    #region pack

                    using (var fileStream = new FileStream(target1, FileMode.Create))
                    {
                        var zip = new ZipWriter(fileStream, new ZipWriterOptions(format == 1 ? CompressionType.None : CompressionType.Deflate));
                        var a = await CleanupLoops.CLean(Directory.EnumerateFiles(Source, "*",new EnumerationOptions() { RecurseSubdirectories = false }).ToArray(), IOC.Default.GetService<MainViewmodel>().Ignorefiles.ToArray(), false, token);
                        foreach (var file in a)
                        {
                            zip.Write(Path.GetFileName(file), new FileInfo(file));
                            IOC.Default.GetService<IProgressBarService>().Progress();
                        }

                        foreach (var folders in folder)
                        {
                           List<string> empty= new(); 
                           empty.Replace(await CleanupLoops.CLean(Directory.EnumerateFiles(folders).ToArray(),
                                IOC.Default.GetService<MainViewmodel>().Ignorefiles.ToArray(), false, token));
                            if (!empty.Any())
                            {
                                IOC.Default.GetService<MainViewmodel>().progressmax++;
                                IOC.Default.GetService<MainViewmodel>().progressmax2++;
                                var fs = new FileStream(Path.Combine(folders, @"onionfile.ow"), FileMode.Create);
                                fs.Dispose();
                                empty.Add((Path.Combine(folders, @"onionfile.ow")));
                            }

                            string path = folders.Replace(IOC.Default.GetService<MainViewmodel>().Copyfromtext, String.Empty);
                            foreach (var file in empty)
                            {
                                var sssa = $"{path}/{Path.GetFileName(file)}";
                                var asss = sssa.Replace("\\", "/");
                                zip.Write(asss, file);
                                IOC.Default.GetService<IProgressBarService>().Progress();
                            }
                        }
                        zip.Dispose();
                    }

                    #endregion
                }
                #endregion
                
            }
        }, token);
    }
    static string GetLastPartFromPath(string path)
    {
        string[] parts = path.Split('\\');
        string lastPart = parts[parts.Length - 1];
        return lastPart;
    }

    private static void emptyy(string Source)
    {
        using (var fss = new FileStream(Path.Combine(Source, @"onionfile.ow"), FileMode.Create))
        {
            IOC.Default.GetService<MainViewmodel>().progressmax++;
            IOC.Default.GetService<MainViewmodel>().progressmax2++;
        }
    }
}