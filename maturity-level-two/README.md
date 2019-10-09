# Practical API Guidelines - Should have

1. [Writing Open API Specifications](#writing-open-api-specifications)

## Writing solid OpenAPI Specifications
Writing OpenAPI specs is easy, writing good OpenAPI specs is a different story.

[Maturity level one covered how you can document & generate OpenAPI specifications](./../maturity-level-one#document-your-apis), however that is only the start.

You should:
- Add your OpenAPI specs to source control given this is part of your application
- Validate changes to your OpenAPI specs to avoid specification violations ([user guide](docs/validating-open-api-specs.md))
- Unit test Open API validation to automatically detect breaking changes

 ## Content negotiation
 When no specific compatibility requirements regarding the rest request and response formats are set, it is recommended to use the JSON format (application/json). But in some situations a client might require the data to be formatted in a certain format in order to interpret the data correctly. Herefore content negotiation may be required, in which the client specifies in which format a request may be send and in which response formats the client can accept. By default ASP.NET Core web applications will expect JSON requests and send JSON responses. In addition to this, ASP.NET Core also uses some additional formatters for special cases, such as the TextOutputFormatter and the HttpNoContentOutputFormatter.

 When you would make sure your api only uses the JSON format, you can specify this in the startup of your ASP.NET Core project. This will make sure you'll refuse all non-JSON 'Accept' headers, including the plain text headers (on these requests you will return response code 406). If no 'Accept' header is specified you can return JSON as it is the only supported type. You can find an example of the changes you have to do to the startup below: 
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
});
```
In case you'd like to add other formatting possibilities, it is possible to add these formatters to your api formatters. In case of xml you can use the XmlSerializerInputFormatter and the XmlSerializerOutputFormatter or add the xml formatters using the Mvc. With this approach the default format is still JSON.
 ```csharp
services.AddMvc()
        .AddNewtonsoftJson()
        .AddXmlSerializerFormatters();
```
Be aware that the formatters you specify in the above section are all the formatters your api will know. Thus if an api call is done towards an action in which an unknown request or response format is used/requested, the api will not answer the call with a success status code but rather with a 406 - Not Acceptable. This means that if you have one action on which the user can request a custom/other response format, you'll have to add a formatter for this type as well - and by default the other actions will support this format too.

You can (not should) further restrict the request and respnse formats for one specific acion or controller by using the [Produces] and [Consumes] attributes. However you should be careful when using these attributes: if you use these attributes your method will not be able to return another response format then format specified in your attribute. If you return another response format the content-type of your response will be overwritten.
```csharp
/// <summary>
/// Get car by Id
/// </summary>
/// <param name="id">car identifier</param>
/// <remarks>Get a car by Id</remarks>
/// <returns>a Car instance</returns>
[HttpPost("{id}", Name = Constants.RouteNames.v1.GetCar)]
[Produces("application/json")]
[Consumes("application/json")]
[SwaggerResponse((int)HttpStatusCode.OK, "Car created", typeof(CarCreatedDto))]
[SwaggerResponse((int)HttpStatusCode.NotFound, "Car id not found")]
[SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
public async Task<IActionResult> CreateCar(int id)
```

## Error response codes
By default error response codes in ASP.NET Core will use the application/xml or application/json content types. These return types will by default work well with the above mentioned way of working: if you remove the xml from the supported formats, your method will return a json content type instead. However, using a custom format for your response code (e.g. application/problem+json) will conflict with the use of the [Produces] attribute: the [Produces] attribute will overwrite the content type from you response and set it to application/json. 


