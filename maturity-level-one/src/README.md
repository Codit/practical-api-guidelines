# Practical API Guidelines - Maturity level One "Must have" - Code Samples
Code samples for maturity level One "Must have".

****************************************************************
**do not upgrade "Microsoft.AspNetCore.All" 2.1.0 to the 2.1.2**
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
	+ check how to handle the version parameter in the OpenAPI specification (WIP)

- DATA CONTRACTS
	+ camlCase (dotnetcore >2 has camlCase by default)
	+ dateFormat (WIP ... to be reviewed)
	+ enumerations serialized to strings

- HTTP METHODS
	+ GET
	+ POST
	+ PUT
	+ PATCH
	+ PATCH (application/json-patch+json)
	- TODO PATCH (application/merge-patch+json)

- HTTP Status Codes (200, 201, 204, 404, 406-ReturnHttpNotAcceptable, 500)

- Security (TODO)

- Error Handling (WIP)
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

- Document your API (WIP)
	+ TODO controller names should be camlCase

- Use thin controllers (WIP)
	+ Add unit tests (TODO)


Example of the URLs:
http://localhost:48067/world-cup/v1/teams
http://localhost:48067/world-cup/v1/teams/1
http://localhost:48067/world-cup/v1/players/2/vote
http://localhost:48067/world-cup/v1/players?top-players-only=true


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