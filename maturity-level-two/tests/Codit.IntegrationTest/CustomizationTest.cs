using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Codit.LevelTwo.Models;
using Newtonsoft.Json;
using FluentAssertions;

using Codit.LevelTwo;

namespace Codit.IntegrationTest
{
    public class CustomizationTest
    {
        private static readonly HttpClient httpClient;
        static CustomizationTest()
        {
            if (httpClient != null) return;
            var srv = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            httpClient = srv.CreateClient();
        }

        [Fact]
        public async Task GetCustomizations_Ok_TestAsync()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/codito/v1/customization");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetSingleCustomization_Ok_TestAsync()
        {
            //Arrange
            int id = 1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task GetSingleCustomiation_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task CreateNewCustomization_Created_TestAsync()
        {
            //Arrange
            var customization = new NewCustomizationDto
            {
                Name = "My Customization",
                CarId = 1,
                InventoryLevel = 1
            };
            var request = TestExtensions.GetJsonRequest(customization, "POST", $"/codito/v1/customization/");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateNewCustomization_BadRequest_TestAsync()
        {
            //Arrange
            var newCustomization = new NewCustomizationDto
            {
                Name = "My Customization",
                InventoryLevel = 1,
                CarId = 200
            };
            var request = TestExtensions.GetJsonRequest(newCustomization, "POST", "/codito/v1/customization/");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
           
        }

        [Fact]
        public async Task DeleteCustomization_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/codito/v1/customization/{id}");
            //Act
            var response = await httpClient.SendAsync(request);           
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task DeleteCustomization_NoContent_TestAsync()
        {
            //Arrange
            int id = 2;
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/codito/v1/customization/{id}");
            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            response = await httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SellCustomization_Accepted_TestAsync()
        {
            //Arrange
            int id = 1;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            var response = await httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<CustomizationDto>(await response.Content.ReadAsStringAsync());

            request = new HttpRequestMessage(new HttpMethod("POST"), $"/codito/v1/customization/{id}/sale");
            //Act
            response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            // (Inventory must be decremented, number of sales incremented.
            request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            response = await httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<CustomizationDto>(await response.Content.ReadAsStringAsync());

            updatedDto.InventoryLevel.Should().Be(actualDto.InventoryLevel - 1);
            updatedDto.NumberSold.Should().Be(actualDto.NumberSold + 1);

        }

        [Fact]
        public async Task SellCustomization_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/codito/v1/customization/{id}/sale");
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SellCustomization_SoldOut409_TestAsync()
        {
            //Arrange
            //(Create new customization. InventoryLevel is not set, so will be zero.)
            var newCustomization = new NewCustomizationDto
            {
                Name = "My Soldout Customization",
                CarId = 1
            };

            var request = TestExtensions.GetJsonRequest(newCustomization, "POST", "/codito/v1/customization/");
            var response = await httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            //(Get Id of this new customization)
            var newDto = JsonConvert.DeserializeObject<CustomizationDto>(await response.Content.ReadAsStringAsync());
            int id = newDto.Id;
            //(Try to sell this "sold out" customization)          
            request = new HttpRequestMessage(new HttpMethod("POST"), $"/codito/v1/customization/{id}/sale");

            //Act
            response = await httpClient.SendAsync(request);
            
            //Assert
            response.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task UpdateCustomizationIncremental_NotFound_TestAsync()
        {
            //Arrange
            int id = -1;
            var customization = new CustomizationDto
            {
                InventoryLevel = 100
            };


            var request = TestExtensions.GetJsonRequest(customization, "PATCH", $"/codito/v1/customization/{id}");

            // Act
            var response = await httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCustomizationIncremental_NoContent_TestAsync()
        {
            //Arrange
            int id = 1;
            var customization = new CustomizationDto
            {
                InventoryLevel = 100
            };

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            var response = await httpClient.SendAsync(request);
            var actualDto = JsonConvert.DeserializeObject<CustomizationDto>(await response.Content.ReadAsStringAsync());
            request = TestExtensions.GetJsonRequest(customization, "PATCH", $"/codito/v1/customization/{id}");

            //actualDto.Should().BeNull();

            // Act
            response = await httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            request = new HttpRequestMessage(new HttpMethod("GET"), $"/codito/v1/customization/{id}");
            response = await httpClient.SendAsync(request);
            var updatedDto = JsonConvert.DeserializeObject<CustomizationDto>(await response.Content.ReadAsStringAsync());
            updatedDto.Id.Should().Be(actualDto.Id);

            //these should stay the same
            updatedDto.Name.Should().Be(actualDto.Name);
            updatedDto.NumberSold.Should().Be(actualDto.NumberSold);
            updatedDto.CarId.Should().Be(actualDto.CarId);

            // this one is updated
            updatedDto.InventoryLevel.Should().Be(customization.InventoryLevel);
            updatedDto.InventoryLevel.Should().NotBe(actualDto.InventoryLevel);


        }



        //[Fact]
        //public async Task DeletePlayer_NotFound_TestAsync()
        //{
        //    //Arrange
        //    int playerId = 1;
        //    var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/world-cup/v1/players/{playerId}");
        //    //Act
        //    var response = await _httpClient.SendAsync(request);
        //    //Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //}
    }
}
