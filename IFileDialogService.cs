using System.Threading.Tasks;

namespace Project1;

public abstract class IFileDialogService
{
    public abstract Task<string> PickFolder();
}