using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExternalService.Services
{
    public class MultiplierService: IMultiplierService
    {
        private readonly Random _random;
        private readonly ILogger<MultiplierService> _logger;
        private readonly IServerClient<Multiplication> _serverClient;
        private readonly IConfigurationSection _section;

        private int GenerateRandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public MultiplierService(ILogger<MultiplierService> logger, IServerClient<Multiplication> serverClient, IConfiguration config)
        {
            _random = new Random();
            _logger = logger;
            _serverClient = serverClient;
            _section = config.GetSection("ExternalService");
        }

        public async Task MultiplyAsync(CancellationToken token, Guid orderId, string batchId, int generatedNumber)
        {
            token.ThrowIfCancellationRequested();
            _logger.Log(LogLevel.Information,
                $"Started Multiply Simulated work, batch number:'{batchId}', generated number: '{generatedNumber} the order id is: '{orderId}'");
            var factor = GenerateRandomInt(2, 4);
            var delay = GenerateRandomInt(5, 10);
            var multipliedNumber = factor * generatedNumber;
            Thread.Sleep(delay * 1000);
            var model = new Multiplication(orderId, batchId, generatedNumber, multipliedNumber);
            var path = _section["MultiplyPath"];
            _logger.Log(LogLevel.Information, $"Multiply work completed for batch Id '{batchId}', Multiplied number is:{multipliedNumber} the order id is: '{orderId}'");

             _serverClient.PostAsync(token, model, path);
        }
    }
}
