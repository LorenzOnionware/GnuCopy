using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Project1.Viewmodels;

namespace Project1;

public class IProgressBarService
{
    public async Task ProgressMax()
    {
        var Files = Directory.EnumerateFiles(MainViewmodel.Default.Copyfromtext, "*", SearchOption.AllDirectories).ToArray();
        MainViewmodel.Default.progressmax = Files.Length-1;
    }

    public async Task Progress()
    {
        MainViewmodel.Default.progressvalue++;
    }
}