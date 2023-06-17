using System.Collections.Generic;
using System.Linq;
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

    public override async Task<string[]> PickFolders()
    {
        var result = await App.MainWindow.StorageProvider.OpenFolderPickerAsync(new() { AllowMultiple = true });
        return result?.Select(f => f.Path.LocalPath).ToArray();
    }


    public async override Task<string[]> PickFile()
    {
        var dialog = new OpenFileDialog();

        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Zip",
            Extensions = { "zip" }
        });
        var result = await dialog.ShowAsync(App.MainWindow);
        return result?.ToArray();
    }

    public async override Task<string> SaveFile()
    {
        var dialog = new SaveFileDialog();
        dialog.Filters.Add( new FileDialogFilter
        {
            Name ="zip",
            Extensions = {"Zip"}
        });
        var result = await dialog.ShowAsync(App.MainWindow);
        return (result+".zip");
    }
}
