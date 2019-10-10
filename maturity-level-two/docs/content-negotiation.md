# Content Negotiation

 When no specific compatibility requirements regarding the rest request and response formats are set, it is recommended to use the JSON format (application/json). However in some situations a client might be restricted in the payload formats it can send to and receive from the server. With [content negotiation](https://developer.mozilla.org/en-US/docs/Web/HTTP/Content_negotiation) we determine what format we'd like to use requests & response payloads. For REST API's, the most common formats are JSON and XML.
As an API consumer, you can specify what format you are expecting by adding HTTP headers:
- `Content-Type` - Specify the format of the payload you send to the server.
- `Accept` - Specify your preferred payload format(s) of the server response. A default format will be used when this header is not specified.
When a format is not supported, the API should return an [HTTP 406 Not Acceptable](https://httpstatuses.com/406).
Because of it's lightness and fastness, JSON has become the standard over the last couple of years but in certain situations other formats still have their advantages. In addition to this, ASP.NET Core also uses some additional formatters for special cases, such as the TextOutputFormatter and the HttpNoContentOutputFormatter.

 When you would like to make sure your api only uses the JSON format, you can specify this in the startup of your ASP.NET Core project. This will make sure you'll refuse all non-JSON 'Accept' headers, including the plain text headers (on these requests you will return response code 406). If no 'Accept' header is specified you can return JSON as it is the only supported type. You can find an example of the changes you have to do in the startup example below: 
 ```csharp
services.AddMvc(options =>
{
    var jsonInputFormatters = options.InputFormatters.OfType<JsonInputFormatter>();
    var jsonInputFormatter = jsonInputFormatters.First();
    options.InputFormatters.Clear();
    options.InputFormatters.Add(jsonInputFormatter);
    var jsonOutputFormatters = options.OutputFormatters.OfType<JsonOutputFormatter>();
    var jsonOutputFormatter = jsonOutputFormatters.First();
    options.OutputFormatters.Clear();
    options.OutputFormatters.Add(jsonOutputFormatter);
}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
```
In here we have to add the 'SetCompatibilityVersion'as well to make sure the supported formats are documented correctly in e.g. the swagger. In case you'd like to add other formatting possibilities, it is possible to add these formatters to your api formatters. In case of xml you can use the XmlSerializerInputFormatter and the XmlSerializerOutputFormatter or add the xml formatters using the Mvc. With this approach the default format is still JSON.
 ```csharp
services.AddMvc()
        .AddNewtonsoftJson()
        .AddXmlSerializerFormatters();
```
Be aware that the formatters you specify in the above section are all the formatters your api will know. Thus if an api call is done towards an action in which an unknown request or response format is used/requested, the api will not answer the call with a success status code but rather with an HTTP 406, Not Acceptable, or an HTTP 415, Unsupported.

You can (not should) further restrict the request and respnse formats for one specific acion or controller by using the [Produces] and [Consumes] attributes. However you should be careful when using these attributes: if you use these attributes your method will not be able to return another response format then format specified in your attribute. If you return another response format the content-type of your response will be overwritten.
```csharp
/// <summary>
/// Create a car
/// </summary>
/// <param name="newCarRequest">New car information</param>
/// <remarks>Create a car</remarks>
/// <returns>a Car instance</returns>
[Produces("application/json")]
[Consumes("application/json")]
[HttpPost(Name = Constants.RouteNames.v1.CreateCar)]
[SwaggerResponse((int)HttpStatusCode.OK, "Car created", typeof(CarCreatedDto))]
[SwaggerResponse((int)HttpStatusCode.Conflict, "Car already exists")]
[SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
public async Task<IActionResult> CreateCar([FromBody] NewCarRequest newCarRequest)
```

In case you have an action which returns media type(s) only this action will return, you can use the [Produces] and [Consumes] keywords too. But be aware that in this case your api might not know how it should serialize the response, so you might have to take care of this yourself. In order to do so you can return a ContentResult (e.g. FileContentResult or ContentResult in the Microsoft.AspNetCore.Mvc namespace). An example is given below: 
```csharp
 /// <summary>
/// Get all cars
/// </summary>
/// /// <param name="bodyType">Filter a specific body Type (optional)</param>
/// <remarks>Get all cars</remarks>
/// <returns>List of cars</returns>
[HttpGet(Name = Constants.RouteNames.v1.GetCars)]
[SwaggerResponse((int)HttpStatusCode.OK, "List of Cars")]
[SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
// [Produces("application/json", "application/problem+json")]
public async Task<IActionResult> GetCars([FromQuery] CarBodyType? bodyType)
{
    return File(GetCars(), "application/pdf", "carlist.pdf");
}
```

## Error response codes
By default error response codes in ASP.NET Core will use the application/problem+xml or application/problem+json content types. These return types will usually work well with the above mentioned way of working: if you remove the xml from the supported formats, your method will return a json content type instead. However the use of the content type application/problem+json will conflict with the use of the [Produces] attribute: the [Produces] attribute will overwrite the content type from your error response and set it to application/json. 


