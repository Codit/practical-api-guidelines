using Codit.LevelOne;
using Codit.LevelOne.Controllers.v1;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace Codit.UnitTest.Controllers
{
    public class TeamsControllerTest
    {
        TeamsController _controller;
        IWorldCupRepository _service;

        public TeamsControllerTest()
        {
            _service = new WorldCupRepositoryFake();
            _controller = new TeamsController(_service);
            AutoMapperConfig.Initialize();
        }

        [Fact]
        public void GetTeams_Test()
        {
            //Arrange
            //Act
            var okTeams = _controller.GetTeams().Result as OkObjectResult;
            //Assert
            okTeams.Value.Should().BeOfType(typeof(List<TeamDto>));
            ((List<TeamDto>)okTeams.Value).Count.Should().Be(2);
        }
    }
}
