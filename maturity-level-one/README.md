# Practical API Guidelines - Maturity level One "Must have"

1. [General HTTP Guidelines](#general-http-guidelines)
2. [Security-first](#security-first)
3. [Error Handling](#error-handling)
4. [Document your APIs](#document-your-apis)
5. [Use thin controllers](#document-your-apis)

## General HTTP Guidelines
### URL Naming
Think about your operation URIs and make them as RESTy as possible – What we mean with RESTy?
1. The API name is a singular concept: `https://master-data.contoso.com` or `https://api.contoso.com/master-data`. The API prefix is useless ie `api/v1/users/{id}/profile`.
2. Use plural for collections of resources: `https://api.contoso.com/master-data/items`
3. No verbs in the url: this is NOT OK `https://api.contoso.com/master-data/getItems`
4. All lower case: this is NOT OK `https://api.contoso.com/File-Ingestion/{collection}/{blob}`
5. Use the `-` for better readability : avoid this `https://api.contoso.com/technicalaccount/contracts`
6. Querystring is meant for querying (filtering, paging, etc..) not for actions nor commands: avoid putting commands in the Querystring parameters: this is NOT OK `https://api.contoso.com/master-data/contracts/{contractId}?operation=cancel`
7. Version your endpoint even if you don’t need it (yet). Very simple but so important. a. Path versioning is the advised approach. `https://api.contoso.com/master-data/v1/items/{itemId}/components/{componentId}`
b. Data Contract versioning is normally done with content-negotiation (via accept header) and custom media types (data contract type + version). According to me this is complex to maintain and complex for the consumer. `GET /items/H12652
Accept: application/vnd.mdm.v2+json`
8. Sometimes is necessary to have commands in the path like `http://api.example.com/cart-management/users/{id}/carts/{id}/checkout`. Is this OK for REST purists? No clue, but it makes to me.

### Data Contracts

- Use camelCase for the attributes
- Serialize enumerations to strings

### HTTP Methods

| Method  | Idempotent (\*)    | Safe (\*\*)        | When to use | Notes |
|:--------|:-------------------|:-------------------|:------------|:------|
| GET     | :heavy_check_mark: | :heavy_check_mark: | Getting the current resource or a list of resources | |
| POST    | :x:                | :x:                | Create a new resource, execute a command (e.g. cart checkout) | If you are not a REST extremist: It can also be used to send a complex query and GET a query result. |
| PUT     | :heavy_check_mark: | :x:                | This should be used to update the whole object | This is NOK for incremental updates. |
| PATCH   | :x:                | :x:                | Incremental update | If you want to update a limited number of fields. |
| DELETE  | :heavy_check_mark: | :x:                | Hard/Soft delete | |
| OPTIONS | :heavy_check_mark: | :heavy_check_mark: | Mainly used for CORS | |

*(\*) From a RESTful service standpoint, for an operation (or service call) to be idempotent, clients can make that same call repeatedly while producing the same result. In other words, making multiple identical requests has the same effect as making a single request. Note that while idempotent operations produce the same result on the server (no side effects), the response itself may not be the same (e.g. a resource's state may change between requests).*

*(\*\*) Safe methods are HTTP methods that do not modify resources. For instance, using GET or HEAD on a resource URL, should NEVER change the resource. However, this is not completely true. It means: it won't change the resource representation. It is still possible, that safe methods do change things on a server or resource, but this should not reflect in a different representation.*

## Security-first

- Always use HTTPS
- Do not put security keys and sensitive information in the query string
  - Certain scenarios are exceptional such as exposing webhooks. When this is the case the keys need to be limited in time to live.

## Error Handling

- Use a global exception handler which allows you to trakc & handle unhandled exceptions very easily
- Errors should be propogated in a consistent way
  - Use `application/problem+json` following [RFC 7807](https://tools.ietf.org/html/rfc7807).
    - Every 4XX/5XX should  the same data contract
    - Less details compared to a custom data contract
    - Read [this blog post](https://tech.domain.com.au/2017/11/please-dont-spare-me-the-details/) on how to achieve this
  - If the above suggestion is not possible you should use a custom data contract. See Microsoft example [here](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#7102-error-condition-responses).
- For cloud apis go with AppInsights
- For on-premise apis which Logging library?
- Shouldn't there be a general response for our API's, a general scheme we can use after all? Maybe using trackingcodes etc with map to app insights?

## Document your APIs
Document your API and be as descriptive as possible – New people should get a clear understanding of what they can expect.

Documentation should include the following at least:
- Operation id
- General Description
- Parameters
- Response codes & contracts

Every API should have documentation in the OpenAPI format. If you want to generate those based on your code you can use tools like [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle) & [NSwag](https://github.com/RSuter/NSwag).

Here is an example on how to generate them with Swashbuckle
```csharp
/// <summary>
///     Get Health
/// </summary>
/// <remarks>Gets the current health status of the API</remarks>
[HttpGet]
[Route("health")]
[SwaggerOperation("get-health")]
[SwaggerResponse(HttpStatusCode.OK, "API is up & running")]
[SwaggerResponse(HttpStatusCode.InternalServerError, "API is not available")]
public IHttpActionResult Get()
{
    return Ok();
}
```

## Use thin controllers
- Put less logic as possible in the controller.
- Testing and dependency injection.
