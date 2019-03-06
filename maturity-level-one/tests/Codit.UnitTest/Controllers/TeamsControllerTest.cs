using Codit.LevelOne;
using Codit.LevelOne.Controllers.v1;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Codit.UnitTest.Controllers
{
    public class TeamsControllerTest
    {
        private readonly TeamsController _controller;

        public TeamsControllerTest()
        {
            _controller = new TeamsController(new WorldCupRepositoryFake());
            AutoMapperConfig.Initialize();
        }

        [Fact]
        public async Task GetTeams_Test()
        {
            //Act
            var okTeams = (await _controller.GetTeams()) as OkObjectResult;

            //Assert
            Assert.NotNull(okTeams);
            okTeams.Value.Should().BeOfType(typeof(List<TeamDto>));
            ((List<TeamDto>)okTeams.Value).Count.Should().Be(2);
        }
    }
}
