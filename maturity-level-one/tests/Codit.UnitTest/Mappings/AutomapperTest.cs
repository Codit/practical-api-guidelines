using AutoMapper;
using Codit.LevelOne;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;

namespace Codit.UnitTest.Mappings
{
    public class AutomapperTest
    {

        [Fact]
        public void Map_PlayerToDto_Test()
        {
            AutoMapperConfig.Initialize();
            var objInstance = new Player
            {
                FirstName = "Mario",
                Description = "He plays for Nice.",
                IsTopPlayer = true,
                TeamId = 1
            };

            var playerDto = new PlayerDto();
            playerDto = Mapper.Map<PlayerDto>(objInstance);

            playerDto.Should().NotBeNull();
            playerDto.Id.Should().Be(objInstance.Id);
            playerDto.FirstName.Should().Be(objInstance.FirstName);
            playerDto.Description.Should().Be(objInstance.Description);
            playerDto.IsTopPlayer.Should().Be(objInstance.IsTopPlayer);
            playerDto.TeamId.Should().Be(objInstance.TeamId);

            playerDto.Should().BeEquivalentTo(objInstance, options => options
                .Including(o => o.Id)
                .Including(o => o.FirstName)
                .Including(o => o.Description)
                .Including(o => o.IsTopPlayer)
                .Including(o => o.TeamId));
        }
    }
}
