using Api.Services;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private ICancellationService _cancellationService;


        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger, ICancellationService cancellationService)
        {
            _orderService = orderService;
            _logger = logger;
            _cancellationService = cancellationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = _orderService.GetAsync(_cancellationService.GetToken(), id).Result;
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> PostBatchTask([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, $"Error {ModelState}");
                return BadRequest(ModelState);
            }
            order.OrderId = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            await _orderService.AddAsync(_cancellationService.GetToken(), order.OrderId, order.BatchCount, order.NumberPerBatch);
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            //Very risky because lots of the operations are events need to add lots of validations
            //to confirm not to delete active batch which accepted as this whole process is event based
            if (_orderService.Delete(_cancellationService.GetToken(), id))
            {
                return Ok(id.ToString());
            }
            return NotFound();

        }
    }
}
