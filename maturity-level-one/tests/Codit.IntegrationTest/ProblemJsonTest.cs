using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Codit.LevelOne.Models;

namespace Codit.IntegrationTest
{
    public class ProblemJsonTest
    {
        private readonly HttpClient _httpClient;
        public ProblemJsonTest()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Codit.LevelOne.Startup>());

            _httpClient = srv.CreateClient();
        }

        [Theory]
        [InlineData("GET")]
        public async Task ProblemJson_RouteNotExists_404_Test(string httpMethod)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/2018/players");
            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            // TODO validate content
        }

        [Theory]
        [InlineData("POST")]
        public async Task ProblemJson_Validation_400_Test(string httpMethod)
        {
            var player = new NewPlayerDto
            {
                FirstName = "Test Player",
                Description = "He plays for Codit.",
                IsTopPlayer = false
            };
            var request = TestExtensions.GetJsonRequest(player, httpMethod, "/world-cup/v1/players");
            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
        }

        // TODO content type not provided .. 415

    }
}
