using System.Collections.Generic;
using System.Threading.Tasks;
using Codit.LevelTwo.Models;
using Codit.LevelTwo.Controllers.v1;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Xunit;

using Codit.LevelTwo;



namespace Codit.UnitTest.Controllers
{
    [Collection("AutoMapper")]
    public class CustomizationControllerTest
    {

        private readonly CustomizationController _controller;

        public CustomizationControllerTest()
        {
            _controller = new CustomizationController(new CoditoRepositoryFake());
        }

        [Fact]
        public async Task GetCustomizations_Test()
        {
            var customizations = (await _controller.GetCustomizations()) as OkObjectResult;
            Assert.NotNull(customizations);
            ((List<CustomizationDto>)customizations.Value).Count.Should().Be(2);

        }
    }
}
