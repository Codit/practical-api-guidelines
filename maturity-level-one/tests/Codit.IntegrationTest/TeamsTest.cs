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

        [Theory]
        [InlineData("GET")]
        public async Task GetTeams_Ok_TestAsync(string httpMethod)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/teams");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode().ShouldBeNotNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("GET")]
        public async Task GetSingleTeam_Ok_TestAsync(string httpMethod)
        {
            int teamId = 1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/teams/{teamId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode().ShouldBeNotNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Theory]
        [InlineData("GET")]
        public async Task GetSingleTeam_NotFound_TestAsync(string httpMethod)
        {
            int teamId = -1;
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/teams/{teamId}");
            var response = await _httpClient.SendAsync(request);
            response.Content.Headers.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
