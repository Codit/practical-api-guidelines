using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;

using Codit.LevelTwo;

namespace Codit.IntegrationTest
{
    public class CarTest
    {
        private static readonly HttpClient httpClient;
        static CarTest()
        {
            if (httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task GetCars_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/codito/v1/car");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleCar_Ok_TestAsync()
        {
            //Arrange
            int id = 1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/car/{id}");
            //Act
            var response = await httpClient.SendAsync(request);
            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleCar_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/car/{id}");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.Content.Headers.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
