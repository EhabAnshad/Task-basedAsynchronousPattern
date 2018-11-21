using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalService.Services
{
    public interface IMultiplierService
    {
        Task MultiplyAsync(CancellationToken token, Guid orderId, string batchId, int generatedNumber);
    }
}
