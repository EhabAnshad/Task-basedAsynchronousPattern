using System.Threading;

namespace Common.Services
{
    public interface ICancellationService
    {
        CancellationToken GetToken();

        void CancelAll();
    }
}
