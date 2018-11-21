using System;
using System.Threading;
using Api.Services;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Common.Services;

namespace Api.Controllers
{
    [Route("api/")]
    public class BatchesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private ICancellationService _cancellationService;

        public BatchesController(IOrderService orderService, ILogger<OrdersController> logger, ICancellationService cancellationService)
        {
            _orderService = orderService;
            _logger = logger;
            _cancellationService = cancellationService;
        }

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> PostBatchTask([FromBody] BatchNumbersGenerated order)
        {

            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, $"Error {ModelState}");
                return BadRequest(ModelState);
            }
            try
            {
                _cancellationService.GetToken().ThrowIfCancellationRequested();
                await _orderService.UpdateBatch(_cancellationService.GetToken(), order.OrderId, order.BatchId, order.MultiplicationNumber);
            }
            catch (OperationCanceledException)
            {
                _logger.Log(LogLevel.Information, $"All services are canceled.");

            }


            return Accepted();
        }

        [HttpPost]
        [Route("Multiply")]
        public async Task<IActionResult> PostBatchMultiply([FromBody] Multiplication order)
        {
            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, $"Error {ModelState}");
                return BadRequest(ModelState);
            }
            try
            {
                _cancellationService.GetToken().ThrowIfCancellationRequested();
                await _orderService.UpdateBatch(_cancellationService.GetToken(), order.OrderId, order.BatchId, order.MultiplicationNumber, true);

            }
            catch (OperationCanceledException)
            {
                _logger.Log(LogLevel.Information, $"All services are canceled.");

            }

            return Accepted();
        }

    }
}
