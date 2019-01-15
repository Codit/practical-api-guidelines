using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Codit.IntegrationTest
{
    public class ProblemJsonTest
    {
        private readonly HttpClient _httpClient;
        public ProblemJsonTest()
        {
            if (_httpClient != null) { return; }
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Codit.LevelOne.Startup>());

            _httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task ProblemJson_RouteNotExists_404_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/world-cup/2018/players");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_Validation_400_Test()
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
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_UnsupportedContentType_415_Test()
        {
            //Arrange
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for MUTD.",
                IsTopPlayer = false,
                TeamId = 1
            };
            var request = TestExtensions.GetJsonRequest(player, "POST", "/world-cup/v1/players", "application/pdf");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_InternalServerError_500_Test()
        {
            //Arrange
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "Evil",
                IsTopPlayer = false,
                TeamId = 1
            };
            var request = TestExtensions.GetJsonRequest(player, "POST", "/world-cup/v1/players");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }
    }
}
