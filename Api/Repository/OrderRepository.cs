using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repository
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApiDbContext _context;

        public OrderRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetAsync(CancellationToken token, Guid id)
        {
            return await _context.Orders.Include(Order => Order.CompletedPerBatch).FirstOrDefaultAsync(o => o.OrderId == id, cancellationToken: token);
        }

        public IEnumerable<Order> BrowseAsync(CancellationToken token) => _context.Orders.AsEnumerable();


        public async Task AddAsync(CancellationToken token, Order order)
        {
            await _context.Orders.AddAsync(order, token);
            await _context.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(CancellationToken token, Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(token);
        }

        public long Delete(CancellationToken token, Guid id)
        {
            var order = GetAsync(token, id).Result;
            if (order == null) return -1;
            _context.Orders.Remove(order);
            return _context.SaveChanges();

        }
    }
}
