using System.IO;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class DeleteDirectories
{
    public static async Task DeleteDirectory(string path)
    {
        var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
        var value2 = Directory.GetFiles(path);
        foreach (var value in value2)
        {
            File.Delete(value);
        }
        foreach (var value in directories)
        {
            Directory.Delete(value,true);
        }
    }
}