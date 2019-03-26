using Codit.LevelTwo.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Codit.IntegrationTest
{
    [Collection("TestServer")]
    public class ProblemJsonTest
    {
        private readonly TestServerFixture _fixture;

        public ProblemJsonTest(TestServerFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task ProblemJson_RouteNotExists_404_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/codito/api/v1/car");
            //Act
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_Validation_400_Test()
        {
            //Arrange
            var customization = new NewCustomizationDto
            {
                Name = "My customization",
            };
            var request = TestExtensions.GetJsonRequest(customization, "POST", "/codito/v1/customization");
            //Act
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_ContentNegotiationNotOk_406_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/codito/codito/car");
            request.Headers.Add("Accept", "custom/content+type");
            //Act
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
            response.Content.Headers.Should().BeNullOrEmpty(); // With 406 the body is suppressed
        }

        [Fact]
        public async Task ProblemJson_UnsupportedContentType_415_Test()
        {
            //Arrange
            var customization = new NewCustomizationDto
            {
                Name = "My customization",
                CarId = 1
            };
            var request = TestExtensions.GetJsonRequest(customization, "POST", "/codito/v1/customization/", "application/pdf");
            //Act
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }

        [Fact]
        public async Task ProblemJson_InternalServerError_500_Test()
        {
            //Arrange
            var customization = new NewCustomizationDto
            {
                Name = "My customization",
                CarId = -9999 // "evil" request
            };
            
            var request = TestExtensions.GetJsonRequest(customization, "POST", "/codito/v1/customization");
            //Act
            var response = await _fixture.Http.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }
    }
}
