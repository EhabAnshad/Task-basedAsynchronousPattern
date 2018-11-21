using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;

namespace Api.Services
{
    public interface IOrderService
    {
        Task AddAsync(CancellationToken token, Guid id, int batchCount, int numberPerBatch);

        Task UpdateAsync(CancellationToken token, Order order);

        Task UpdateBatch(CancellationToken token, Guid id, string batchId, int finalNumber, bool batchComplete=false);

        Task<Order> GetAsync(CancellationToken token, Guid id);

        bool Delete(CancellationToken token, Guid id);


    }
}
 