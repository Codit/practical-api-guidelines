# Practical API Guidelines - Maturity level One "Must have" - Code Samples
****************************************************************
Code samples for maturity level One "Must have".
****************************************************************

These include:
- URL NAMING
	+ api name 
	+ plural for collections
	+ no verbs in URLs
	+ lower case URLs
	+ "-" for better readibility
	+ command as part of the URL 
	+ querystring for filters

- VERSIONING in the path & ApiVersion attribute

- DATA CONTRACTS
	+ camlCase (dotnetcore >2 has camlCase by default)
	+ enumerations serialized to strings

- HTTP METHODS
	+ GET
	+ POST
	+ PUT
	+ PATCH
	+ PATCH (application/json-patch+json)
	
- HTTP Status Codes (200, 201, 204, 400, 404, 415, 500)

- Error Handling (problem+json is returned for 4XX and 5XX)
	+ problem+json (when model is not valid)
		```json
		{
		  "errors": {
			"FirstName": [
			  "The FirstName field is required."
			]
		  },
		  "type": "https://asp.net/core",
		  "title": "One or more validation errors occurred.",
		  "status": 400,
		  "detail": "Please refer to the errors property for additional details.",
		  "instance": "/world-cup/v1/players"
		}
		```
	+ 5XX by the generic exception handler
	+ 400 for invalid model (controller decorated with [ValidateModel])
	+ 4XX by passing ProblemDetailsError to the ActionResult (this is not an elegant solution but I preferred to limit the usage of external libraries like this https://github.com/khellang/Middleware )
	+ unmatched routes via UseStatusCodePagesWithReExecute + error controller (e.g. 404)

- Document your API

- Unit and integration tests 


Example of the URLs:
- http://localhost:48067/world-cup/v1/teams
- http://localhost:48067/world-cup/v1/teams/1
- http://localhost:48067/world-cup/v1/players/2/vote
- http://localhost:48067/world-cup/v1/players?top-players-only=true


Example of

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
