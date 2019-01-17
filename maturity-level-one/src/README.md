# Practical API Guidelines - Maturity level One "Must have" - Code Samples
****************************************************************
This implementation includes:
## URL conventions 
	* api name 
	* plural for collections
	* no verbs in URLs
	+ lower case URLs
	* "-" for better readibility
	* command as part of the URL 
	* querystring for filters

## VERSIONING 
    There is no 'right way' to version your APIs.
	    * the version number is in the PATH. 
    	* this is the most used and simplest approach to versioning, on the other hand is less flexible compared to the QS/ContentType versioning.

## DATA CONTRACTS
	* camlCase (dotnetcore >2 has camlCase by default)
	* enumerations serialized to strings

## HTTP METHODS
	* GET
	* POST
	* PUT
	* PATCH
	* PATCH (application/json-patch+json)
	
## HTTP Status Codes 
	* 200 OK
	* 201 Created (e.g. Create a new Player)
	* 202 Accepted (e.g. Vote for a Best player)
	* 204 No Content
	* 400 Bad Request (e.g. Client sent invalid data)
	* 404 Not Found (e.g. Not existing resource has been requested)
	* 406 Not Acceptable (e.g. Client requested a not supported  Media Type via Accept header)
	* 415 Unsupported Media Type (e.g. Client sent a not supported Media Type via Content-TYpe header)
	* 500 Internal Server Error (e.g. An error occured in the API)

## Error Handling 
    * problem+json is returned for 4XX and 5XX). In this first implementation we preferred to not use an external library to manage the problem json.
    * 5XX by the generic exception handler
    * 4XX by passing ProblemDetailsError to the ActionResult (this is not an elegant solution but I preferred to limit the usage of external libraries like this https://github.com/khellang/Middleware )
    * unmatched routes via UseStatusCodePagesWithReExecute + error controller (e.g. 404)
    * 400 for invalid model (controller decorated with [ValidateModel])

## Document your API
	* Swagger generation with OperationId, Xml comments, Swagger attributes
		```csharp
        /// <summary>
        /// Get the profiles of the players
        /// </summary>
        /// <param name="topPlayersOnly">Indicates whether to return the top players only</param>
        /// <remarks>Operation description here</remarks>
        /// <returns>Return a list of Players</returns>
        [HttpGet(Name = "Players_GetPlayers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "List of players")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
		```
	+ [ApiExplorerSettings(IgnoreApi = true)] used to exclude operations from OpenAPI output
    
## Unit and integration tests 
	* controllers unit test
	* contract mapping unuit test
	* api operations integration test
	* problem+json integration test
	* open api generation integration test

## Examples
Example of the response in case of 400 BadRequest
```json
	{
	  "errors": {
		"FirstName": [
		  "The FirstName field is required."
		],
		"Description": [
		  "The Description field is required."
		]
	  },
	  "type": "/world-cup/v1/players",
	  "title": "Validation error",
	  "status": 400,
	  "detail": "Please refer to the errors property for additional details.",
	  "instance": "urn:codit.eu:client-error:11ebf6c0-c80d-416c-8c0d-010cbd5a2fa1"
	}
```

Example of the URLs:
- http://localhost:48067/world-cup/v1/teams
- http://localhost:48067/world-cup/v1/teams/1
- http://localhost:48067/world-cup/v1/players/2/vote
- http://localhost:48067/world-cup/v1/players?top-players-only=true

Example of incremental PATCH

PATCH http://localhost:48067/world-cup/v1/players/update/4
Content-Type: application/json-patch+json
```json
[
	{ 
	"op": "replace", 
	"path": "/description", 
	"value": "He plays for Manchester United (UK). " 
	},
	{ 
	"op": "replace", 
	"path": "/isTopPlayer", 
	"value": "false" 
	}	
]
```
