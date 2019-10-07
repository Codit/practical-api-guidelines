# Practical API Guidelines - Should have

1. [Writing Open API Specifications](#writing-open-api-specifications)

## Writing solid OpenAPI Specifications
Writing OpenAPI specs is easy, writing good OpenAPI specs is a different story.

[Maturity level one covered how you can document & generate OpenAPI specifications](./../maturity-level-one#document-your-apis), however that is only the start.

You should:
- Add your OpenAPI specs to source control given this is part of your application
- Validate changes to your OpenAPI specs to avoid specification violations ([user guide](docs/validating-open-api-specs.md))
- Unit test Open API validation to automatically detect breaking changes

 ## Document your api requests and responses

 You should use the right attributes in order to make sure your API is documented correctly. You can use the [Consumes] or [Produces] attribute (or both, depending on your method) in order to specify what content type your api consumes and produces. You can optionally use these attributes in on your controller class if you'd like to define this attribute for each method of your controller. In order to clarify the response body scheme of a request, you can use the [SwaggerResponse] attribute when you're using Swashbuckle. Make sure you use all parameters in the [SwaggerResponse] attribute when doing an HTTP GET with response status OK, the type argument might not be needed in other cases. 
 Here is an example on how to generate them with Swashbuckle.
```csharp
/// <summary>
/// Get car by Id
/// </summary>
/// <param name="id">car identifier</param>
/// <remarks>Get a car by Id</remarks>
/// <returns>a Car instance</returns>
[HttpGet("{id}", Name = Constants.RouteNames.v1.GetCar)]
[Produces("application/json")]
[SwaggerResponse((int)HttpStatusCode.OK, "Car info", typeof(CarDetailsDto))]
[SwaggerResponse((int)HttpStatusCode.NotFound, "Car id not found")]
[SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
public async Task<IActionResult> GetCar(int id)
{
    var car = await _coditoRepository.GetCarAsync(id,true);

    if (car == null) return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));

    var result = Mapper.Map<CarDetailsDto>(car);
    return Ok(result);
}
```

