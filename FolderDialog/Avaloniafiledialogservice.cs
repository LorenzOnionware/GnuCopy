using System.Threading.Tasks;
using Avalonia.Controls;

namespace Project1;

public sealed class Avaloniafiledialogservice: IFileDialogService
{
    public async override Task<string> PickFolder()
    {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(App.MainWindow);
        return result;
    }
}