using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;

using Codit.LevelTwo;

namespace Codit.IntegrationTest
{
    [Collection("TestServer")]
    public class OpenApiTest
    {
        TestServerFixture fixture;

        public OpenApiTest(TestServerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task OpenApi_OperationId_Test()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/swagger/v1/swagger.json");
            //Act
            var response = await fixture._httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var swaggerDoc = JObject.Parse(await response.Content.ReadAsStringAsync());
            var paths = (JObject)swaggerDoc.SelectToken("paths");

            paths.Count.Should().Be(5);

            foreach (var item in paths)
            {
                switch (item.Key)
                {
                    case "/codito/v1/customization":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be(Constants.RouteNames.v1.GetCustomizations);
                        ((string)item.Value.SelectToken("post.operationId")).Should().Be(Constants.RouteNames.v1.CreateCustomization);
                        break;
                    case "/codito/v1/customization/{id}/sale":
                        ((string)item.Value.SelectToken("post.operationId")).Should().Be(Constants.RouteNames.v1.SellCustomization);
                        break;
                    case "/codito/v1/customization/{id}":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be(Constants.RouteNames.v1.GetCustomization);
                        ((string)item.Value.SelectToken("patch.operationId")).Should().Be(Constants.RouteNames.v1.UpdateCustomizationIncremental);
                        ((string)item.Value.SelectToken("delete.operationId")).Should().Be(Constants.RouteNames.v1.DeleteCustomization);
                        break;
                    case "/codito/v1/car":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be(Constants.RouteNames.v1.GetCars); 
                        break;
                    case "/codito/v1/car/{id}":
                        ((string)item.Value.SelectToken("get.operationId")).Should().Be(Constants.RouteNames.v1.GetCar);
                        break;
                    default:
                        Assert.True(false, $"{item.Key} is an unexpected path");
                        break;
                }
            }
        }
    }
}
