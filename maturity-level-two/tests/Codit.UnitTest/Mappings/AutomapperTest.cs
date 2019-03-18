using AutoMapper;
using Codit.LevelTwo;
using Codit.LevelTwo.Entities;
using Codit.LevelTwo.Models;
using Xunit;
using FluentAssertions;

namespace Codit.UnitTest.Mappings
{
    public class AutomapperTest
    {
        public AutomapperTest()
        {
            AutoMapperConfig.Initialize();
        }

        [Fact]
        public void Map_CustomizationToDto_Test()
        {
            //Arrange
            var customization = new Customization
            {
                //Id = 4,
                Name = "My customization",
                Url = "https://fake-url.com",
                NumberSold = 5,
                InventoryLevel = 3,
                CarId = 1
            };

            //Act
            var customizationDto = Mapper.Map<CustomizationDto>(customization);

            //Assert
            customizationDto.Should().NotBeNull();
            customizationDto.Id.Should().Be(customization.Id);
            customizationDto.Name.Should().Be(customization.Name);
            customizationDto.Url.Should().Be(customization.Url);
            customizationDto.NumberSold.Should().Be(customization.NumberSold);
            customizationDto.InventoryLevel.Should().Be(customization.InventoryLevel);
            customizationDto.CarId.Should().Be(customization.CarId);

        }

        [Fact]
        public void Map_CarToDto_Test()
        {
            //Arrange
            var car = new Car
            {
                Brand = "Skoda",
                Model = "Octavia Combi",
                BodyType = CarBodyType.Break,
                Description = "Skoda's most popular break.",
            };

            //Act
            var carDto = Mapper.Map<CarDto>(car);

            //Assert
            carDto.Should().NotBeNull();
            carDto.Id.Should().Be(car.Id);           
            carDto.Brand.Should().Be(car.Brand);
            carDto.Model.Should().Be(car.Model);
            carDto.BodyType.Should().Be(car.BodyType);
            carDto.Description.Should().Be(car.Description);

        }

    }
}
