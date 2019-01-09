using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Codit.LevelOne.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace Codit.IntegrationTest
{
    public class PlayersTest
    {
        private readonly HttpClient _httpClient;
        public PlayersTest()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Codit.LevelOne.Startup>());

            _httpClient = srv.CreateClient();
        }

        [Theory]
        [InlineData("GET")]
        public async Task GetPlayers_Ok_TestAsync(string httpMethod)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        public async Task GetSinglePlayer_Ok_TestAsync(string httpMethod)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players/1");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Theory]
        [InlineData("GET")]
        public async Task GetSinglePlayer_NotFound_TestAsync(string httpMethod)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players/-1");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Theory]
        [InlineData("POST")]
        public async Task CreateNewPlayer_Created_TestAsync(string httpMethod)
        {
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for Codit.",
                IsTopPlayer = false,
                TeamId = 1
            };
            var data = JsonConvert.SerializeObject(player);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [InlineData("POST")]
        public async Task CreateNewPlayer_BadRequest_TestAsync(string httpMethod)
        {
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for Codit.",
                IsTopPlayer = false
            };
            var data = JsonConvert.SerializeObject(player);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
