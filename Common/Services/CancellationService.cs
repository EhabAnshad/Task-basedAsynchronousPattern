using System.Threading;

namespace Common.Services
{
    public class CancellationService: ICancellationService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public CancellationService()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken GetToken()
        {
            return _cancellationTokenSource.Token;
        }

        public void CancelAll()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
