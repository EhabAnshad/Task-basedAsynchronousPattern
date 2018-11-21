using Api.Repository;
using Api.Services;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace API.Test.Unit
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private Mock<IConfiguration> _configMock;
        private readonly Mock<IServerClient<Order>> _serverClientMock;
        private readonly Mock<IServerClient<Batch>> _batchClientMock;
        private Mock<ICancellationService> _cancellationServiceMock;
        private Mock<ILogger<OrderService>> _loggerMock;

        public OrderServiceTest()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _serverClientMock = new Mock<IServerClient<Order>>();
            _batchClientMock = new Mock<IServerClient<Batch>>();
            _configMock = new Mock<IConfiguration>();
            _cancellationServiceMock = new Mock<ICancellationService>();
            _loggerMock = new Mock<ILogger<OrderService>>();
        }

        [Fact]
        public async Task TestUpdateOrderId()
        {
            var id = Guid.NewGuid();
            var batchCount = 2;
            var numberPerBatch = 2;
            var order = new Order(id, batchCount, numberPerBatch);
            var completedPerBatch = new List<Batch> {new Batch("1", 1), new Batch("2", 2)};
            order.TotalOrdersCount = batchCount * numberPerBatch;
            order.CompletedPerBatch = completedPerBatch;

            _repositoryMock.Setup(x => x.GetAsync(_cancellationServiceMock.Object.GetToken(), id)).ReturnsAsync(order);
            var service = new OrderService(_repositoryMock.Object,_serverClientMock.Object,_batchClientMock.Object,_configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5);

            _repositoryMock.Verify(x=>x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), It.IsAny<Order>()), Times.Once);

        }

        [Fact]
        public async Task TestAddOrder()
        {
            var id = Guid.NewGuid();
            var service = new OrderService(_repositoryMock.Object, _serverClientMock.Object, _batchClientMock.Object, _configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.AddAsync(_cancellationServiceMock.Object.GetToken(), id, 5, 5);
           _repositoryMock.Verify(x => x.AddAsync(_cancellationServiceMock.Object.GetToken(), It.IsAny<Order>()), Times.Once);
        }
       

        [Fact]
        public async Task TestUpdateOrderIdCompletionStatus()
        {
            var id = Guid.NewGuid();
            var batchCount = 4;
            var numberPerBatch = 5;
            var updatedCount = 5;
            var order = new Order(id, batchCount, numberPerBatch);
            var completedPerBatch = new List<Batch>
            {
                new Batch("1", updatedCount),
                new Batch("2", 2)
            };
            order.TotalOrdersCount = batchCount * numberPerBatch;
            order.CompletedPerBatch = completedPerBatch;

            _repositoryMock.Setup(x => x.GetAsync(_cancellationServiceMock.Object.GetToken(), id)).ReturnsAsync(order);
            _repositoryMock.Setup(x => x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), order)).Returns(Task.FromResult(order));
            var service = new OrderService(_repositoryMock.Object, _serverClientMock.Object, _batchClientMock.Object, _configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5, true);

            _repositoryMock.Verify(x => x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), It.Is<Order>(p=>p.CompletedOrdersCount == updatedCount)));
        }

        [Fact]
        public async Task TestUpdateOrderIdCompletionStatusAccumulate()
        {
            var id = Guid.NewGuid();
            var batchCount = 4;
            var numberPerBatch = 5;
            var updatedCount = 5;
            var order = new Order(id, batchCount, numberPerBatch);
            var completedPerBatch = new List<Batch>
            {
                new Batch("1", updatedCount),
                new Batch("2",updatedCount)
            };
            order.TotalOrdersCount = batchCount * numberPerBatch;
            order.CompletedPerBatch = completedPerBatch;

            _repositoryMock.Setup(x => x.GetAsync(_cancellationServiceMock.Object.GetToken(), id)).ReturnsAsync(order);
            _repositoryMock.Setup(x => x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), order)).Returns(Task.FromResult(order));
            var service = new OrderService(_repositoryMock.Object, _serverClientMock.Object, _batchClientMock.Object, _configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5, true);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "2", 5);
            _repositoryMock.Verify(x => x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), It.Is<Order>(p => p.CompletedOrdersCount == updatedCount*2)));
        }

        [Fact]
        public async Task TestPostingABackMultiplierToExternalServices()
        {
            //setup valid order with valid batch
            var id = Guid.NewGuid();
            var batchCount = 4;
            var numberPerBatch = 5;
            var updatedCount = 5;
            var order = new Order(id, batchCount, numberPerBatch);
            var completedPerBatch = new List<Batch>
            {
                new Batch("1", updatedCount),
                new Batch("2",updatedCount)
            };
            order.TotalOrdersCount = batchCount * numberPerBatch;
            order.CompletedPerBatch = completedPerBatch;
            var batch = new Batch("1", 5)
            {
                OrderId = id
            };
            //mock the configuration and client services
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _configMock.Setup(x => x.GetSection("ExternalService")).Returns(config.GetSection("ExternalService"));

            _repositoryMock.Setup(x => x.GetAsync(_cancellationServiceMock.Object.GetToken(), id)).ReturnsAsync(order);
            _repositoryMock.Setup(x => x.UpdateAsync(_cancellationServiceMock.Object.GetToken(), order)).Returns(Task.FromResult(order));
            _batchClientMock.Setup(x => x.PostAsync(_cancellationServiceMock.Object.GetToken(), batch, "Multiplier")).ReturnsAsync(true);



            var service = new OrderService(_repositoryMock.Object, _serverClientMock.Object, _batchClientMock.Object, _configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5);

            //If will call Multiplier service if batch not complete and will not call if it is complete
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "2", 5);
            _batchClientMock.Verify(x => x.PostAsync(_cancellationServiceMock.Object.GetToken(), It.IsAny<Batch>(), "Multiplier"), Times.Exactly(2));
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5,true);
            _batchClientMock.Verify(x => x.PostAsync(_cancellationServiceMock.Object.GetToken(), It.IsAny<Batch>(), "Multiplier"), Times.Exactly(2));
        }

        [Fact]
        public async Task TestPostingABackGeneratorToExternalServices()
        {
            //setup valid order with valid batch
            var id = Guid.NewGuid();
            var batchCount = 4;
            var numberPerBatch = 5;
            var updatedCount = 5;
            var order = new Order(id, batchCount, numberPerBatch);
            var completedPerBatch = new List<Batch>
            {
                new Batch("1", updatedCount),
                new Batch("2", updatedCount)
            };
            order.TotalOrdersCount = batchCount * numberPerBatch;
            order.CompletedPerBatch = completedPerBatch;

            //mock the configuration and client services
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _configMock.Setup(x => x.GetSection("ExternalService")).Returns(config.GetSection("ExternalService"));

            _repositoryMock.Setup(x => x.GetAsync(_cancellationServiceMock.Object.GetToken(), id)).ReturnsAsync(order);
            _repositoryMock.Setup(x => x.AddAsync(_cancellationServiceMock.Object.GetToken(), order)).Returns(Task.FromResult(order));
            _serverClientMock.Setup(x => x.PostAsync(_cancellationServiceMock.Object.GetToken(), order, "Generator")).ReturnsAsync(true);



            var service = new OrderService(_repositoryMock.Object, _serverClientMock.Object, _batchClientMock.Object,
                _configMock.Object, _cancellationServiceMock.Object, _loggerMock.Object);
            await service.UpdateBatch(_cancellationServiceMock.Object.GetToken(), id, "1", 5);

            //If will call Multiplier service if batch not complete and will not call if it is complete
            await service.AddAsync(_cancellationServiceMock.Object.GetToken(), order.OrderId, batchCount,numberPerBatch);
            _serverClientMock.Verify(x => x.PostAsync(_cancellationServiceMock.Object.GetToken(), It.IsAny<Order>(), "Generator"), Times.Once);
        }
    }
}



