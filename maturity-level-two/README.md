# Practical API Guidelines - Should have

1. [Validating OpenAPI Specifications](docs/validating-open-api-specs.md)
1. [Content Negotiation](docs/content-negotiation.md)
1. [APISecurity](docs/api-security.md)


## Validating OpenAPI Specifications
Writing OpenAPI specs is easy, writing good OpenAPI specs is a different story.

[Maturity level one covered how you can document & generate OpenAPI specifications](./../maturity-level-one#document-your-apis), however that is only the start.

You should:
- Add your OpenAPI specs to source control given this is part of your application
- Validate changes to your OpenAPI specs to avoid specification violations ([user guide](docs/validating-open-api-specs.md))
- Unit test Open API validation to automatically detect breaking changes

 ## Content negotiation
With [content negotiation](https://developer.mozilla.org/en-US/docs/Web/HTTP/Content_negotiation) a consumer specifies in which format he/she will communicate (send and receive data) with the server. You can do this by using the following headers in your request:
- `Content-Type` - Specify the format of the payload you send to the server.
- `Accept` - Specify your preferred payload format(s) of the server response. A default format will be used when this header is not specified. Also other accept headers can be used, you can find more info about this [here](https://developer.mozilla.org/en-US/docs/Web/HTTP/Content_negotiation).

When you send a `Content-Type` the server doesn't understand, the server should return an HTTP 415: Unsupported Media Type. If the server cannot respond to your request in a format specified in your `Accept` header, it will return an HTTP 406: Not Acceptable.

When adding Content-Negotiation to your project you should:
* Think whether content negotiation is really necessary. For most of the cases you only need JSON and thus no content negotiation is needed.
* Remove input and output formatters when multi-format (JSON, XML, CSV, ...) is not necessary. 
* Carefully evaluate whether you should use the [Produces] and [Consumes] attributes to further restrict the supported request and response media types for one specific acion or controller.
    * [Produces] and [Consumes] are not meant to document the supported media types.  
    * It is strongly advised to not use these attributes if you are supporting more than one media type (e.g. application/json and application/problem+json)
    
Notes: 
* `Content-Type` is not needed for HTTP GET requests since a GET request has no request body.
* If you want to explicitly specify which content types are produced/consumed in your swagger file, we advise to use a custom attribute (to be checked whether something).                  

## API Security
API security is an essential part when designing the API. All different levels of security are discussed within the API-Security document ([user guide](docs/api-security.md)).

You should:
- When possible, use a token based authenticaton/authorization (OAuth2, MSI, etc..)
- When not, depending on the use case, consider to use a combination of the available security mechanisms (e.g. Client Certificate, api key, ip filtering, etc..)
