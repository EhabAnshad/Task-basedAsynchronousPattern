using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalService.Services
{
    public interface IGeneratorService
    {
        Task GenerateRandomNumber(CancellationToken token, Guid id, int numberOfBatches);
    }
}
