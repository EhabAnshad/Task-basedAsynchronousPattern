using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;

namespace Api.Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(CancellationToken token, Guid id);
        IEnumerable<Order> BrowseAsync(CancellationToken token);
        Task AddAsync(CancellationToken token, Order order);
        Task UpdateAsync(CancellationToken token, Order order);
        long Delete(CancellationToken token, Guid id);
    }
}
