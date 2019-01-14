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
using Microsoft.AspNetCore.JsonPatch;

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        public async Task GetSinglePlayer_Ok_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Theory]
        [InlineData("GET")]
        public async Task GetSinglePlayer_NotFound_TestAsync(string httpMethod)
        {
            int playerId = -1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");
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
            var request = TestExtensions.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players");
            var response = await _httpClient.SendAsync(request);
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
            var request = TestExtensions.GetJsonRequest(player, httpMethod, "/world-cup/v1/players");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("POST")]
        public async Task VoteAsBestPlayer_Accepted_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}/vote");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Theory]
        [InlineData("POST")]
        public async Task VoteAsBestPlayer_NotFound_TestAsync(string httpMethod)
        {
            int playerId = -1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}/vote");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("PUT")]
        public async Task UpdatePlayer_NoContent_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var player = new PlayerDto
            {
                FirstName = "Hazard",
                Description = "He plays in Chelsea.",
                IsTopPlayer = true,
                TeamId = 2
            };

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            request = TestExtensions.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);
            Assert.Equal(actualDto.Description, updatedDto.Description);
            Assert.Equal(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);
            Assert.Equal(2, updatedDto.TeamId);

        }

        [Theory]
        [InlineData("PATCH")]
        public async Task UpdatePlayerIncremental_NoContent_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var player = new PlayerDto
            {
                Description = "He's still playing for Chelsea."
            };

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            request = TestExtensions.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);
            Assert.NotEqual(actualDto.Description, updatedDto.Description);
            Assert.Equal(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);
            Assert.Equal(actualDto.TeamId, updatedDto.TeamId);

        }

        [Theory]
        [InlineData("PATCH")]
        public async Task UpdatePlayerIncrementalJsonPatch_Ok_TestAsync(string httpMethod)
        {
            int playerId = 1;
            JsonPatchDocument<PlayerDto> player = new JsonPatchDocument<PlayerDto>();
            player.Replace(p => p.Description, "He's still playing for Chelsea.");
            player.Replace(p => p.IsTopPlayer, false);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            request = TestExtensions.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}/update", "application/json-patch+json");
            response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);
            Assert.NotEqual(actualDto.Description, updatedDto.Description);
            Assert.NotEqual(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);
            Assert.Equal(actualDto.TeamId, updatedDto.TeamId);

        }

        [Theory]
        [InlineData("PATCH")]
        public async Task UpdatePlayerIncrementalJsonPatch_BadRequest_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var player = new PlayerDto
            {
                Description = "He's still playing for Chelsea."
            };

            var request = TestExtensions.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}/update");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("DELETE")]
        public async Task DeletePlayer_NotFound_TestAsync(string httpMethod)
        {
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
