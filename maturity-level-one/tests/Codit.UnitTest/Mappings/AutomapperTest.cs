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

            var dtoInstance = new PlayerDto();
            dtoInstance = Mapper.Map<PlayerDto>(objInstance);

            dtoInstance.Should().NotBeNull();
            dtoInstance.Id.Should().Be(objInstance.Id);
            dtoInstance.FirstName.Should().Be(objInstance.FirstName);
            dtoInstance.Description.Should().Be(objInstance.Description);
            dtoInstance.IsTopPlayer.Should().Be(objInstance.IsTopPlayer);
            dtoInstance.TeamId.Should().Be(objInstance.TeamId);

            dtoInstance.Should().BeEquivalentTo(objInstance, options => options
                .Including(o => o.Id)
                .Including(o => o.FirstName)
                .Including(o => o.Description)
                .Including(o => o.IsTopPlayer)
                .Including(o => o.TeamId));
        }
    }
}
