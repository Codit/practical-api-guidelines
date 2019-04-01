# Validating OpenAPI Specifications

It is crucial to only ship OpenAPI specifications that are according the [official specification](https://swagger.io/specification/).

In order to achieve this, the generated OpenAPI specs for your application should be automatically validated during pull requests.

This can be achieved by using [swagger-cli NPM package](https://www.npmjs.com/package/swagger-cli) which you can call via a CLI:
```shell
# Install the package
npm install -g swagger-cli

# Validate the OpenAPI specs located in 'deploy/openapi/specs.json'
swagger-cli validate deploy/openapi/specs.json
```

More features and how to use the tool can be found in the [documentation](https://www.npmjs.com/package/swagger-cli#usage).

## Build YAML definition
Here is a YAML representation of the build that you can use.
```yaml
pool:
  name: Hosted VS2017
steps:
- powershell: 'npm install -g swagger-cli'
  displayName: 'Install swagger-cli'

- powershell: 'swagger-cli validate deploy/openapi/specs.json'
  displayName: 'Validate OpenAPI specs'
```