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
    public class GeneratorController : ControllerBase
    {
        private readonly ILogger<GeneratorController> _logger;
        private readonly IGeneratorService _generatorService;
        private ICancellationService _cancellationService;


        public GeneratorController(ILogger<GeneratorController> logger, IGeneratorService generatorService, ICancellationService cancellationService)
        {
            _logger = logger;
            _generatorService = generatorService;
            _cancellationService = cancellationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateNumber generateNumberModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, $"Error {ModelState}");
                return BadRequest(ModelState);
            }
            _logger.Log(LogLevel.Information, $"Generator service received a request. order id: '{generateNumberModel.OrderId}', Batch count :'{generateNumberModel.BatchCount}'.");
            try
            {
                await _generatorService.GenerateRandomNumber(_cancellationService.GetToken(), generateNumberModel.OrderId, generateNumberModel.BatchCount);

            }
            catch (OperationCanceledException)
            {
                _logger.Log(LogLevel.Information, $"All services are canceled.");

            }
            return Accepted();
        }

    }
}
