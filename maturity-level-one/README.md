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
To be added

## Error Handling
To be added

## Document your APIs
To be added

## Use thin controllers
- Put less logic as possible in the controller.
- Testing and dependency injection.
