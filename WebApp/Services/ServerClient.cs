using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class ServerClient : IServerClient<Order> 
    {

        private readonly HttpClient _client;
        private readonly IConfigurationSection _section;

        private string SerializeToBody(Order value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private async Task<Order> DeserializeObject(HttpResponseMessage response)
        {
            var stringResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Order>(stringResult);
        }

        public ServerClient(IConfiguration config)
        {
            _section = config.GetSection("ExternalService");
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(_section["BaseUrl"]);
        }

        public async Task<Order> PostAsync(string jsonBody = "", string path = "")
        {
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync(path, httpContent);
            if (!postResponse.IsSuccessStatusCode) return new Order();
            return await DeserializeObject(postResponse);
        }

        public async Task<Order> PostAsync(Order model, string path = "")
        {
            var jsonBody = SerializeToBody(model);
            return await PostAsync(jsonBody, path);
        }

        public async Task<Order> GetAsync(string id, string path)
        {
            var response = await _client.GetAsync($"{path}/{id}");
            if (!response.IsSuccessStatusCode) return new Order();
            return await DeserializeObject(response);
        }

    }
}
