# Practical API Guidelines - Maturity level One "Must have" - Code Samples
Code samples for maturity level One "Must have".

These include:
- URL NAMING
	+ api name 
	+ plural for collections
	+ no verbs in URLs
	+ lower case URLs
	+ "-" for better readibility
	+ command as part of the URL 

- VERSIONING in the path & ApiVersion attribute
	+ check how to handle the version parameter in the OpenAPI specification (WIP)

- DATA CONTRACTS
	+ camlCase (dotnetcore >2 has camlCase by default)
	+ dateFormat (WIP ... to be reviewed)

- HTTP METHODS
	+ GET
	+ POST

- HTTP Status Codes (200, 201, 204, 404, 500)

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