using Api.Controllers;
using Api.Services;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Services;
using Xunit;

namespace API.Test.Unit
{
    public class OrderControllerTest
    {

        private readonly Mock<ILogger<OrdersController>> _logger;
        private readonly Mock<IOrderService> _orderService;
        private Mock<ICancellationService> _cancellationService;

        public OrderControllerTest()
        {
            _logger = new Mock<ILogger<OrdersController>>();
            _orderService = new Mock<IOrderService>();
            _cancellationService = new Mock<ICancellationService>();
        }

        [Fact]
        public async Task TestGettingOrderId()
        {
            var id = Guid.NewGuid();
            var batchCount = 5;
            var numberPerBatch = 3;
            _orderService.Setup(x => x.GetAsync(_cancellationService.Object.GetToken(), id))
                .ReturnsAsync(new Order(id, batchCount, numberPerBatch));
            var controller = new OrdersController(_orderService.Object, _logger.Object, _cancellationService.Object);

            var result = await controller.Get(id);

            var contentResult = (Order)((ObjectResult) result).Value;
            Assert.Equal(contentResult.OrderId, id);
            Assert.Equal(contentResult.BatchCount, batchCount);
            Assert.Equal(contentResult.NumberPerBatch, numberPerBatch);
        }

        [Fact]
        public  void TestDeleteOrderIdNotFound()
        {
            var id = Guid.NewGuid();
            _orderService.Setup(x => x.Delete(_cancellationService.Object.GetToken(), id))
                .Returns(false);
            var controller = new OrdersController(_orderService.Object, _logger.Object, _cancellationService.Object);

            var result =  controller.Delete(id);

            var contentResult = result as NotFoundResult;
            Assert.Equal(404, contentResult.StatusCode); 
        }

        [Fact]
        public void TestDeleteOrderIdDeleted()
        {
            var id = Guid.NewGuid();
            _orderService.Setup(x => x.Delete(_cancellationService.Object.GetToken(), id))
                .Returns(true);
            var controller = new OrdersController(_orderService.Object, _logger.Object, _cancellationService.Object);

            var result = controller.Delete(id);

            var contentResult = result as OkObjectResult;
            Assert.Equal(200, contentResult.StatusCode);
        }
        
    }
}



