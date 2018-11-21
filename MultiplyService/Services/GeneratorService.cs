using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExternalService.Services
{
    public class GeneratorService : IGeneratorService
    {
        private readonly Random _random;
        private readonly ILogger<GeneratorService> _logger;
        private readonly IServerClient<BatchNumbersGenerated> _serverClient;
        private readonly IConfiguration _section;

        private int GenerateRandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public GeneratorService(ILogger<GeneratorService> logger, IServerClient<BatchNumbersGenerated> serverClient, IConfiguration config)
        {
            _random = new Random();
            _logger = logger;
            _serverClient = serverClient;
            _section = config.GetSection("ExternalService");
        }

        public async Task GenerateRandomNumber(CancellationToken token, Guid id, int numberOfBatches)
        {
            _logger.Log(LogLevel.Information,$"Started Generate Simulated work, number of batches:'{numberOfBatches}' the order id is: '{id}'");
            for (var i = 1; i <= numberOfBatches; i++)
            {
                token.ThrowIfCancellationRequested();
                var delay = GenerateRandomInt(5, 10);
                var generatedNumber = GenerateRandomInt(1, 100);

                Thread.Sleep(delay * 1000);
                var batch = new BatchNumbersGenerated(id, i.ToString(), generatedNumber);

                var path = _section["GeneratePath"];
                _logger.Log(LogLevel.Information, $"Generate work completed for batch'{i.ToString()}', generated number is:{generatedNumber} the order id is: '{id}'");
                await _serverClient.PostAsync(token, batch, path: path);
                //TODO: Think if this is real service then we should log this to DB so my TODO to add to DB
            }
        }
    }
}
