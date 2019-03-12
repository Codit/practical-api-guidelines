using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;

namespace Codit.IntegrationTest
{
    public class TeamsTest
    {
        private readonly HttpClient _httpClient;
        public TeamsTest()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Codit.LevelOne.Startup>());

            _httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task GetTeams_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/world-cup/v1/teams");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleTeam_Ok_TestAsync()
        {
            //Arrange
            int teamId = 1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/teams/{teamId}");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleTeam_NotFound_TestAsync()
        {
            //Arrange
            int teamId = -1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/teams/{teamId}");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.Content.Headers.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
