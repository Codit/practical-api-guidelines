using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Codit.IntegrationTest
{
    [Collection("TestServer")]
    public class CarTest
    {
        private readonly TestServerFixture _fixture;

        public CarTest(TestServerFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task GetCars_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/codito/v1/car");
            //Act
            var response = await _fixture.Http.SendAsync(request);
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
            var response = await _fixture.Http.SendAsync(request);
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
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.Content.Headers.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
