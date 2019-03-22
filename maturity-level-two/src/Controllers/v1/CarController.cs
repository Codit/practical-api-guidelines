using System.Collections.Generic;
using System.Threading.Tasks;
using Codit.LevelTwo.Extensions;
using Codit.LevelTwo.Services;
using Codit.LevelTwo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Codit.LevelTwo.Entities;

namespace Codit.LevelTwo.Controllers.v1
{
    [Route("codito/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Input validation error.")]
    public class CarController : ControllerBase
    {
        private readonly ICoditoRepository _coditoRepository;

        public CarController(ICoditoRepository coditoRepository)
        {
            _coditoRepository = coditoRepository;
        }

        /// <summary>
        /// Get all cars
        /// </summary>
        /// /// <param name="bodyType">Filter a specific body Type (optional)</param>
        /// <remarks>Get all cars</remarks>
        /// <returns>List of cars</returns>
        [HttpGet(Name = Constants.RouteNames.v1.GetCars)]
        [SwaggerResponse((int)HttpStatusCode.OK, "List of Cars")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetCars([FromQuery] CarBodyType? bodyType)
        {
            var cars = await _coditoRepository.GetCarsAsync(bodyType);
            var results = Mapper.Map<IEnumerable<CarDto>>(cars);

            return Ok(results);
        }

        /// <summary>
        /// Get car by Id
        /// </summary>
        /// <param name="id">car identifier</param>
        /// <remarks>Get a car by Id</remarks>
        /// <returns>a Car instance</returns>
        [HttpGet("{id}", Name = Constants.RouteNames.v1.GetCar)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Car info")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Car id not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _coditoRepository.GetCarAsync(id,true);

            if (car == null) return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));

            var result = Mapper.Map<CarDetailsDto>(car);
            return Ok(result);
        }

    }
}