using Api;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;
using static Xunit.Assert;

namespace API.Test
{
    public class OrdersControllerIntegrationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public OrdersControllerIntegrationTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task TestGettingNotExistingOrderId()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync("api/orders/"+id.ToString());
            Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestDeletingNotExistingOrderId()
        {
            var id = Guid.NewGuid();
            var response = await _client.DeleteAsync("api/orders/" + id.ToString());
            Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestpostingNewOrder()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var response = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task TestDeleteValidOrder()
        {
            //setup
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var response = await _client.DeleteAsync("api/orders/" + jObject["orderId"]);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetValidOrder()
        {
            //setup
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            var body = postResponse.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(body);

            var response = await _client.GetAsync("api/orders/" + jObject["orderId"]);
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync();
            Assert.Contains( "batchCount\":5,\"numberPerBatch\":2", content.Result);
        }


        [Fact]
        public async Task TestInValidOrderMissingBatchCount()
        {
            
            var jsonInString = "{\"NumberPerBatch\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);

        }

        [Fact]
        public async Task TestInValidOrderMissinNumberPerBatch()
        {
            var jsonInString = "{\"BatchCount\": 2}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task TestInValidOrderInvalidNumberPerBatchMoreThanMax()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 11}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task TestInValidOrderInvalidNumberPerBatchLessThanMinimum()
        {
            var jsonInString = "{\"BatchCount\":5,\"NumberPerBatch\": 0}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task TestInValidOrderInvalidBatchCountMoreThanMax()
        {
            var jsonInString = "{\"BatchCount\":11,\"NumberPerBatch\": 5}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task TestInValidOrderInvalidBatchCountLessThanMinimum()
        {
            var jsonInString = "{\"BatchCount\":0,\"NumberPerBatch\": 5}";
            var postResponse = await _client.PostAsync("api/orders/", new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }
    }
}
