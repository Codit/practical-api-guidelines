# Practical API Guidelines - Should have

1. [Validating OpenAPI Specifications](docs/validating-open-api-specs.md)
1. [APISecurity](docs/api-security.md)


## Validating OpenAPI Specifications
Writing OpenAPI specs is easy, writing good OpenAPI specs is a different story.

[Maturity level one covered how you can document & generate OpenAPI specifications](./../maturity-level-one#document-your-apis), however that is only the start.

You should:
- Add your OpenAPI specs to source control given this is part of your application
- Validate changes to your OpenAPI specs to avoid specification violations ([user guide](docs/validating-open-api-specs.md))
- Unit test Open API validation to automatically detect breaking changes

## API Security
API security is an essential part when designing the API. All different levels of security are discussed within the API-Security document ([user guide](docs/api-security.md)).

You should:
- When possible, use a token based authenticaton/authorization (OAuth2, MSI, etc..)
- When not, depending on the use case, consider to use a combination of the available security mechanisms (e.g. Client Certificate, api key, ip filtering, etc..)
