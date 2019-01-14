using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Codit.LevelOne.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using FluentAssertions;

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

        [Fact]
        public async Task GetPlayers_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/world-cup/v1/players");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSinglePlayer_Ok_TestAsync()
        {
            //Arrange
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task GetSinglePlayer_NotFound_TestAsync()
        {
            //Arrange
            int playerId = -1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task CreateNewPlayer_Created_TestAsync()
        {
            //Arrange
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for Codit.",
                IsTopPlayer = false,
                TeamId = 1
            };
            var request = TestExtensions.GetJsonRequest(player, "POST", $"/world-cup/v1/players");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateNewPlayer_BadRequest_TestAsync()
        {
            //Arrange
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for Codit.",
                IsTopPlayer = false
            };
            var request = TestExtensions.GetJsonRequest(player, "POST", "/world-cup/v1/players");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task VoteAsBestPlayer_Accepted_TestAsync()
        {
            //Arrange
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/world-cup/v1/players/{playerId}/vote");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Fact]
        public async Task VoteAsBestPlayer_NotFound_TestAsync()
        {
            //Arrange
            int playerId = -1;
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/world-cup/v1/players/{playerId}/vote");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePlayer_NoContent_TestAsync()
        {
            //Arrange
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
            request = TestExtensions.GetJsonRequest(player, "PUT", $"/world-cup/v1/players/{playerId}");

            //Act
            response = await _httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            updatedDto.FirstName.Should().Be(actualDto.FirstName);
            updatedDto.Description.Should().Be(actualDto.Description);
            updatedDto.IsTopPlayer.Should().Be(actualDto.IsTopPlayer);
            updatedDto.TeamId.Should().Be(2);
        }

        [Fact]
        public async Task UpdatePlayerIncremental_NoContent_TestAsync()
        {
            //Arrange
            int playerId = 1;
            var player = new PlayerDto
            {
                Description = "He's still playing for Chelsea."
            };

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);
            request = TestExtensions.GetJsonRequest(player, "PATCH", $"/world-cup/v1/players/{playerId}");
            
            // Act
            response = await _httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            response = await _httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);
            updatedDto.FirstName.Should().Be(actualDto.FirstName);
            updatedDto.Description.Should().NotBe(actualDto.Description);
            updatedDto.IsTopPlayer.Should().Be(actualDto.IsTopPlayer);
            updatedDto.TeamId.Should().Be(actualDto.TeamId);
        }

        [Fact]
        public async Task UpdatePlayerIncrementalJsonPatch_Ok_TestAsync()
        {
            //Arrange
            int playerId = 1;
            JsonPatchDocument<PlayerDto> player = new JsonPatchDocument<PlayerDto>();
            player.Replace(p => p.Description, "He's still playing for Chelsea.");
            player.Replace(p => p.IsTopPlayer, false);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");
            var response = await _httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);
            request = TestExtensions.GetJsonRequest(player, "PATCH", $"/world-cup/v1/players/{playerId}/update", "application/json-patch+json");

            //Act
            response = await _httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);

            updatedDto.FirstName.Should().Be(actualDto.FirstName);
            updatedDto.Description.Should().NotBe(actualDto.Description);
            updatedDto.IsTopPlayer.Should().Be(!actualDto.IsTopPlayer);
            updatedDto.TeamId.Should().Be(actualDto.TeamId);

        }

        [Fact]
        public async Task UpdatePlayerIncrementalJsonPatch_BadRequest_TestAsync()
        {
            //Arrange
            int playerId = 1;
            var player = new PlayerDto
            {
                Description = "He's still playing for Chelsea."
            };

            var request = TestExtensions.GetJsonRequest(player, "PATCH", $"/world-cup/v1/players/{playerId}/update");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeletePlayer_NotFound_TestAsync()
        {
            //Arrange
            int playerId = 1;
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/world-cup/v1/players/{playerId}");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
