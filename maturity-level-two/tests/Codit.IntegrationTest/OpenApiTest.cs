using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Codit.IntegrationTest
{
    public class OpenApiTest
    {
        private readonly HttpClient _httpClient;
        public OpenApiTest()
        {
            if (_httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Codit.LevelOne.Startup>());

            _httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task OpenApi_OperationId_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/swagger/v1/swagger.json");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var swaggerDoc = JObject.Parse(await response.Content.ReadAsStringAsync());
            var paths = (JObject)swaggerDoc.SelectToken("paths");
            paths.Count.Should().Be(6);

            foreach (var item in paths)
            {
                switch (item.Key)
                {
                    case "/world-cup/v1/players":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be("Players_GetPlayers");
                        ((string)item.Value.SelectToken("post.operationId")).Should().Be("Players_Create");
                        break;
                    case "/world-cup/v1/players/{id}/vote":
                        ((string)item.Value.SelectToken("post.operationId")).Should().Be("Players_VoteAsBestPlayer");
                        break;
                    case "/world-cup/v1/players/{id}":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be("Players_GetPlayer");
                        ((string)item.Value.SelectToken("put.operationId")).Should().Be("Players_UpdateFull");
                        ((string)item.Value.SelectToken("patch.operationId")).Should().Be("Players_UpdateIncremental");
                        break;
                    case "/world-cup/v1/players/{id}/update":
                        ((string)item.Value.SelectToken("patch.operationId")).Should().Be("Players_UpdateIncrementalJsonPatch");
                        break;
                    case "/world-cup/v1/teams":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be("Teams_GetTeams");
                        break;
                    case "/world-cup/v1/teams/{id}":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be("Teams_GetTeam");
                        break;
                    default:
                        Assert.True(false, $"{item.Key} is an unexpected path");
                        break;
                }
            }
        }
    }
}
