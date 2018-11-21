using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common.Services
{
    public class ServerClient<T> : IServerClient<T> where T : new()
    {
        private readonly ILogger<ServerClient<T>> _logger;
        private readonly HttpClient _client;
        private readonly IConfigurationSection _section;

        private string SerializeToBody(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public ServerClient(IConfiguration config, ILogger<ServerClient<T>> logger)
        {
            _logger = logger;
            _section = config.GetSection("ExternalService");
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(_section["BaseUrl"]);
        }

        public async Task<bool> PostAsync(CancellationToken token, string jsonBody = "", string path = "")
        {
            token.ThrowIfCancellationRequested();
            _logger.Log(LogLevel.Information, $"Posting externally for {typeof(T)}, baseUrl'{ _client.BaseAddress}', pathL'{path}'");
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync(path, httpContent, token);
            return postResponse.IsSuccessStatusCode;
        }

        public async Task<bool> PostAsync(CancellationToken token, T model, string path = "")
        {
            var jsonBody = SerializeToBody(model);
            return await PostAsync(token, jsonBody, path);
        }

        public async Task<T> GetAsync(CancellationToken token, string id, string path)
        {
            token.ThrowIfCancellationRequested();
            _logger.Log(LogLevel.Information, $"Getting external resource for {typeof(T)} , baseUrl'{ _client.BaseAddress}', pathL'{path}'");
            var response = await _client.GetAsync($"{path}/{id}", token);
            if (!response.IsSuccessStatusCode) return new T();
            var stringResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }

        public async Task<bool> PutAsync(CancellationToken token, string jsonBody = "", string path = "")
        {
            token.ThrowIfCancellationRequested();
            _logger.Log(LogLevel.Information, $"Put externally for {typeof(T)}, baseUrl'{ _client.BaseAddress}', pathL'{path}'");
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var postResponse = await _client.PutAsync(path, httpContent, token);
            return postResponse.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync(CancellationToken token, T model, string path = "")
        {
            var jsonBody = SerializeToBody(model);
            return await PutAsync(token, jsonBody, path);
        }
    }
}
