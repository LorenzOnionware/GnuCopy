using System.Threading.Tasks;

namespace GnuCopy.Interfaces;

public interface IProgressBarService
{
    void AddProgress();
    Task MaxProgressAsync();
}
