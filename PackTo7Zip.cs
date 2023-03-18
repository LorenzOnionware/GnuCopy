using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Writers;


namespace Project1;

public class PackTo7Zip
{
    //format bool gives the package format false=7zip true=tar.
    public static async Task PackTo(bool format, string source, string target)
    {
        if (format == false)
        {
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(source);
                archive.SaveTo(target, new WriterOptions(CompressionType.Deflate)
                {
                    LeaveStreamOpen = true
                });
            }
        }
    }
}