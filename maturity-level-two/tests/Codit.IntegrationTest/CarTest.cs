using Xunit;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;

using Codit.LevelTwo;

namespace Codit.IntegrationTest
{
    [Collection("TestServer")]
    public class CarTest
    {
        TestServerFixture fixture;

        public CarTest(TestServerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetCars_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/codito/v1/car");
            //Act
            var response = await fixture._httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleCar_Ok_TestAsync()
        {
            //Arrange
            int id = 1;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/codito/v1/car/{id}");
            //Act
            var response = await fixture._httpClient.SendAsync(request);
            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleCar_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/codito/v1/car/{id}");
            //Act
            var response = await fixture._httpClient.SendAsync(request);
            //Assert
            response.Content.Headers.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
