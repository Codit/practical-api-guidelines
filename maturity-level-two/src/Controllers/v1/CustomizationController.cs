using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codit.LevelTwo.Entities;
using Codit.LevelTwo.Extensions;
using Codit.LevelTwo.Models;
using Codit.LevelTwo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper;

using Codit.LevelTwo;

namespace Codit.LevelTwo.Controllers.v1
{
    [Route("codito/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Input validation error.")]
    public class CustomizationController : ControllerBase
    {

        private readonly ICoditoRepository _coditoRepository;

        public CustomizationController(ICoditoRepository coditoRepository)
        {
            _coditoRepository = coditoRepository;
        }

        /// <summary>
        /// Get all customizations
        /// </summary>
        /// <remarks>Get all customizations, ordered by popularity</remarks>
        /// <returns>List of customizations</returns>
        [HttpGet(Name = Constants.RouteNames.v1.GetCustomizations)]
        [SwaggerResponse((int)HttpStatusCode.OK, "List of players")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetCustomizations()
        {
            var customizations = await _coditoRepository.GetAllCustomizationsAsync();
            var results = Mapper.Map<IEnumerable<CustomizationDto>>(customizations);
            return Ok(results);
        }


        /// <summary>
        /// Get customization by Id
        /// </summary>
        /// <param name="id">customization identifier</param>
        /// <remarks>Get a customization by Id</remarks>
        /// <returns>a Customization instance</returns>
        [HttpGet("{id}", Name = Constants.RouteNames.v1.GetCustomization)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Customization info")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Customization not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetCustomization(int id)
        {
            var customization = await _coditoRepository.GetCustomizationAsync(id);
            if (customization == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }
            var result = Mapper.Map<CustomizationDto>(customization);
            return Ok(result);
        }

        /// <summary>
        /// Create customization
        /// </summary>
        /// <param name="customization">Instance of a customization object</param>
        /// <remarks>Add new customization to the database</remarks>
        /// <returns>Acknowledge the object has been created</returns>
        [HttpPost(Name = Constants.RouteNames.v1.CreateCustomization)]
        [SwaggerResponse((int)HttpStatusCode.Created, "New customization added. New object and location are returned.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> Create(NewCustomizationDto newCustomization)
        {           
            if (newCustomization.CarId == -9999)
            {
                throw new ArgumentException("this is evil code");
            }

            var car = await _coditoRepository.GetCarAsync(newCustomization.CarId, false);
            if (car == null)
            {
                return BadRequest(new ProblemDetailsError(StatusCodes.Status400BadRequest, $"There is no car with id {newCustomization.CarId}."));
            }

            var newCustomizationObject = new Customization
            {
                Name = newCustomization.Name,
                CarId = newCustomization.CarId,
                Url = newCustomization.Url,
                InventoryLevel = newCustomization.InventoryLevel,
            };

            await _coditoRepository.CreateCustomizationAsync(newCustomizationObject);
            var result = Mapper.Map<CustomizationDto>(newCustomizationObject);

            return CreatedAtRoute(Constants.RouteNames.v1.GetCustomization, new { id = result.Id }, result);
        }

        /// <summary>
        /// Delete customization by id
        /// </summary>
        /// <param name="customization">Instance of a customization object</param>
        /// <remarks>Remove a customization from the database</remarks>
        /// <returns>Acknowledge the object has been deleted</returns>
        [HttpDelete("{id}", Name = Constants.RouteNames.v1.DeleteCustomization)]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Customization succesfully deleted")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Customization not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> DeleteCustomization(int id)
        {
            int numberOfChanges = await _coditoRepository.DeleteCustomizationAsync(id);
            if(numberOfChanges > 0)
            {
                return NoContent();
            }
            else
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }           
        }

        /// <summary>
        /// Update customization
        /// </summary>
        /// <param name="id">customization identifier</param>
        /// <param name="customizationDto">Update for the customization object</param>
        /// <remarks>Update the customization profile (incremental update)</remarks>
        /// <returns>Acknowledge the object has been updated</returns>
        [HttpPatch("{id}", Name = Constants.RouteNames.v1.UpdateCustomizationIncremental)]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The data has been updated")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Customization not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> UpdateIncremental(int id, [FromBody] CustomizationDto customizationDto)
        {
            var customizationObj = await _coditoRepository.GetCustomizationAsync(id);
            if (customizationObj == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }

            // User passes Id as a parameter. If an Id is passed in the body, it is ignored.
            customizationDto.Id = id;
            var customizationUpdated = Mapper.Map<Customization>(customizationDto);

            await _coditoRepository.ApplyPatchAsync(customizationUpdated);
            return NoContent();

        }

        /// <summary>
        /// Sell a car customization
        /// </summary>
        /// <param name="id">Customization identifier</param>
        /// <returns>Acknowledge that the c</returns>
        [HttpPost("{id}/sale", Name = Constants.RouteNames.v1.SellCustomization)]
        [SwaggerResponse((int)HttpStatusCode.Accepted, "Sale accepted. No response body")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Out of stock")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Car customization not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> SellCustomization(int id)
        {
            SalesRequestResult result = await _coditoRepository.ApplyCustomizationSaleAsync(id);
            switch (result)
            {
                case SalesRequestResult.NotFound:
                    return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
                case SalesRequestResult.OutOfStock:
                    return Conflict(new ProblemDetailsError(StatusCodes.Status409Conflict));
                default:
                    return Accepted();
            }
        }
    }
}