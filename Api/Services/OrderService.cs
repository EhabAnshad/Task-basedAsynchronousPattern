using Api.Repository;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IServerClient<Order> _orderClient;
        private readonly IServerClient<Batch> _batchClient;
        private readonly IConfiguration _section;
        private ObservableCollection<IModel> _orders;
        private ICancellationService _cancellationService;
        private ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IServerClient<Order> serverClient, IServerClient<Batch> batchClient, IConfiguration config, ICancellationService cancellationService, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderClient = serverClient;
            _batchClient = batchClient;
            _section = config.GetSection("ExternalService");
            _orders = new ObservableCollection<IModel>();
            _orders.CollectionChanged += HandleChange;
            _cancellationService = cancellationService;
            _logger = logger;
        }

        private async void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            foreach (IModel model in e.NewItems)
            {
                try
                {
                    switch (model)
                    {

                        case Order order:
                            await _orderClient.PostAsync(_cancellationService.GetToken(), order,
                                _section["GeneratorPath"]);
                            break;
                        case Batch batch:
                            await _batchClient.PostAsync(_cancellationService.GetToken(), batch,
                                _section["MultiplierPath"]);
                            break;

                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.Log(LogLevel.Information, $"All services are canceled.");

                }
            }

        }

        public async Task AddAsync(CancellationToken token, Guid id, int batchCount, int numberPerBatch)
        {

            var batchesInit = new List<Batch>();
            for (var i = 1; i <= batchCount; i++)
            {
                batchesInit.Add(new Batch(i.ToString(), numberPerBatch));
            }

            var order = new Order(id, batchCount, numberPerBatch)
            {
                TotalOrdersCount = batchCount * numberPerBatch,
                CompletedPerBatch = batchesInit,
                CompletedOrdersCount = 0
            };


            await _orderRepository.AddAsync(token, order);
            _orders.Add(order);
        }

        public async Task UpdateAsync(CancellationToken token, Order order) => await _orderRepository.UpdateAsync(token, order);

        public async Task<Order> GetAsync(CancellationToken token, Guid id) => await _orderRepository.GetAsync(token, id);

        public bool Delete(CancellationToken token, Guid id)
        {
            var deleted = _orderRepository.Delete(token, id) > 0;
            if (!deleted) return false;
            var order = _orders.First(x => x.OrderId == id);
            _orders.Remove(order);
            _cancellationService.CancelAll();
            return true;
        }

        public async Task UpdateBatch(CancellationToken token, Guid orderId, string batchId, int finalNumber, bool batchComplete = false)
        {
            token.ThrowIfCancellationRequested();
            var order = GetAsync(token, orderId).Result;
            // could check if order has value 

            if (!batchComplete)
            {
                var numberPerBatch = order.CompletedPerBatch.Where(x => x.Key == batchId).Select(x => x.Value)
                    .FirstOrDefault();
                order.CompletedOrdersCount += numberPerBatch;
            }

            var batch = order.CompletedPerBatch.First(x => x.Key == batchId);
            batch.Value = finalNumber;
            batch.IsCompleted = batchComplete;

            if (!batchComplete)
            {
                _orders.Add(batch);
            }
            await UpdateAsync(token, order);
        }
    }
}
