using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Codit.LevelTwo.Models;
using Codit.LevelTwo.Extensions;
using Codit.LevelTwo;
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
                .UseStartup<Startup>());

            _httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task ProblemJson_RouteNotExists_404_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/codito/api/v1/car");
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
            var customization = new NewCustomizationDto
            {
                Name = "My customization",
            };
            var request = TestExtensions.GetJsonRequest(customization, "POST", "/codito/v1/customization");
            //Act
            var response = await _httpClient.SendAsync(request);
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
            _httpClient.DefaultRequestHeaders.Add("Accept", "custom/content+type");
            //Act
            var response = await _httpClient.SendAsync(request);
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
            var customization = new NewCustomizationDto
            {
                Name = "My customization",
                CarId = -9999 // "evil" request
            };
            
            var request = TestExtensions.GetJsonRequest(customization, "POST", "/codito/v1/customization");
            //Act
            var response = await _httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
            response.ShouldBeProblemJson();
        }
    }
}
