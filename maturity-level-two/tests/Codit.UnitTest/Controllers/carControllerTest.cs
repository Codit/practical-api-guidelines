using Codit.LevelTwo;
using Codit.LevelTwo.Controllers.v1;
using Codit.LevelTwo.Models;
using Codit.LevelTwo.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using Codit.UnitTest;

namespace Codit.UnitTest.Controllers
{
    [Collection("AutoMapper")]
    public class CarControllerTest
    {

        private readonly CarController _controller;

        public CarControllerTest()
        {
            _controller = new CarController(new CoditoRepositoryFake());
        }

        [Fact]
        public async Task GetCars_test()
        {
            //act
            var cars = (await _controller.GetCars(null)) as OkObjectResult;

            //assert
            Assert.NotNull(cars);
            cars.Value.Should().BeOfType(typeof(List<CarDto>));
            ((List<CarDto>)cars.Value).Count.Should().Be(2);
        }

        [Fact]
        public async Task GetCar_test()
        {
            //act
            int id = 0;
            var car = (await _controller.GetCar(id)) as OkObjectResult;

            Assert.NotNull(car);
            ((CarDetailsDto)car.Value).Id.Should().Be(0);           
        }
    }
        
}
