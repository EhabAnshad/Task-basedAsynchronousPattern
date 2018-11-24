using System;
using Common.Models;
using Common.Services;
using ExternalService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExternalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MultiplierController : ControllerBase
    {
        private readonly ILogger<MultiplierController> _logger;
        private readonly IMultiplierService _multiplierService;
        private ICancellationService _cancellationService;

        public MultiplierController(ILogger<MultiplierController> logger, IMultiplierService multiplierService, ICancellationService cancellationService)
        {
            _logger = logger;
            _multiplierService = multiplierService;
            _cancellationService = cancellationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Batch model)
        {
            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, $"Error {ModelState}");
                return BadRequest(ModelState);
            }
            _logger.Log(LogLevel.Information, $"Multiplier service received a request. order id: '{model.OrderId}', Batch Id :'{model.BatchId}'.");
            try
            {
                _multiplierService.MultiplyAsync(_cancellationService.GetToken(), model.OrderId, model.Key, model.Value);
            }
            catch (OperationCanceledException)
            {
                _logger.Log(LogLevel.Information, $"All services are canceled.");

            }

            return Accepted();
        }

    }
}
