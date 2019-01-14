using AutoMapper;
using Codit.LevelOne;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Models;
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
        public void Map_PlayerToDto_Test()
        {
            //Arrange
            var objInstance = new Player
            {
                FirstName = "Mario",
                Description = "He plays for Nice.",
                IsTopPlayer = true,
                TeamId = 1
            };

            //Act
            var playerDto = Mapper.Map<PlayerDto>(objInstance);

            //Assert
            playerDto.Should().NotBeNull();
            playerDto.Id.Should().Be(objInstance.Id);
            playerDto.FirstName.Should().Be(objInstance.FirstName);
            playerDto.Description.Should().Be(objInstance.Description);
            playerDto.IsTopPlayer.Should().Be(objInstance.IsTopPlayer);
            playerDto.TeamId.Should().Be(objInstance.TeamId);

        }
    }
}
