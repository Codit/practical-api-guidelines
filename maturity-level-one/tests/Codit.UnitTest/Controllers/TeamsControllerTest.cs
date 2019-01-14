using Codit.LevelOne;
using Codit.LevelOne.Controllers;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
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
            // Act
            var okTeams = _controller.GetTeams().Result as OkObjectResult;
            var teams = Assert.IsType<List<TeamDto>>(okTeams.Value);
            Assert.Equal(2, teams.Count);
        }
    }
}
