using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Project1;

public abstract class IFileDialogService
{
    public abstract Task<string> PickFolder();
    public abstract Task<string[]> PickFolders();

    public abstract Task<string[]?> PickFile();
    public abstract Task<string> SaveFile();

}