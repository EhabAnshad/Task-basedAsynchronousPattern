using Api;
using Common.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.Test
{
    public class BatchesControllerIntegrationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BatchesControllerIntegrationTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }
       
        [Fact]
        public async Task TestPostingNewBatch()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var batch = new BatchNumbersGenerated((Guid)jObject["orderId"], "1", 123);
            var jsonBody = JsonConvert.SerializeObject(batch);
            var response = await _client.PostAsync("api/Generate", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPostingInvalidBatchId()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var batch = new BatchNumbersGenerated((Guid)jObject["orderId"], "33", 123);
            var jsonBody = JsonConvert.SerializeObject(batch);
            var response = await _client.PostAsync("api/Generate", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var resultMessage = response.Content.ReadAsStringAsync().Result;
            Assert.True(resultMessage.Contains("Invalid Range, Values must be between 1-10"),$"Expected message changed, current response message is: '{resultMessage}'");
        }

        [Fact]
        public async Task TestPostingNewMultiplyBatch()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var batch = new Multiplication((Guid)jObject["orderId"], "1", 123, 222);
            var jsonBody = JsonConvert.SerializeObject(batch);
            var response = await _client.PostAsync("api/Multiply", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPostingInvalidMultiplyBatchId()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var batch = new Multiplication((Guid)jObject["orderId"], "33", 123, 222);
            var jsonBody = JsonConvert.SerializeObject(batch);
            var response = await _client.PostAsync("api/Multiply", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var resultMessage = response.Content.ReadAsStringAsync().Result;
            Assert.True(resultMessage.Contains("Invalid Range, Values must be between 1-10"), $"Expected message changed, current response message is: '{resultMessage}'");
        }

    }
}
