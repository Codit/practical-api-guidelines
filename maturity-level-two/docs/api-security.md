# API Security
API-security is an important topic on design & development time of the API.

The document below is explaining the general security guidelines followed by some Azure-specific guidelines in the second half of the document.

1. [General Guidelines](#general-http-guidelines)
    - [HTTPS](#HTTPS)
    - [IP-Filtering](#IP-Filtering)
    - [API Key (Shared Access Key)](#API-Key-(Shared-Access-Key))
    - [TLS Mutual Authentication](#TLS-Mutual-Authentication)
    - [OAuth2 Token (From Identity Provider)](#OAuth2-Token-(From-Identity-Provider))
   
2. [Azure Specifics](#Azure-specifics)
   - [API Management](#API-Management)
   - [Web App](#Web-App)
   - [Managed Service Identity](#Managed-Service-Identity)
   - [Azure Functions](#Azure-Functions)


## General Guidelines
Security can be provided in the different layers of our application infrastructure which have their own pros & cons.  
This guideline discusses first the least secure solution & goes up based on that. Please take this into consideration when choosing your security mechanism.  
You can also combine all of these guidelines, you do not need to stick to one.

Most of the security mechanisms explained below can be handled on network & application level. Keep this in mind when choosing a security role. It is always best to handle it as soon as possible (so network is the preferred level).

### HTTPS
HTTPS and only HTTPS should be allowed on production API's. This is mandatory security measurement to take when designing and creating your API.

Please make sure that you have a valid server certificate and that it remains valid.
It is possible to bypass server certificate validation on the client side, but this should never be done in any case!

### IP-Filtering
IP-Filtering is a system where you validate the incoming IP-address on the server. This can be one item or a list. Preferably, this list is as small as possible. 

Note that this does require a static IP of all the users, and by this validation, still ALL machines behind that public-IP can access the resource.

IP-filtering is done ideally on firewall level/API-Management.

### API Key (Shared Access Key)
The API-Key is a custom HTTP Header with a specific name. Usually this is called `x-API-key`, but can also be called otherwise if required.

The value of this key should be shared between the user and the server.
The server / API will validate the incoming request via an `Authentication Filter` and verify the value matches between the user and the stored one on the API.

To increase security you can here also use `Rolling API keys` where you allow 2 keys at one time, and you require all the users to update to the new key periodically. This way, if a APIKey is intercepted, it will only be valid for a certain amount of time. Also, having just the mechanism in place and being able to force the rolling at a specific time can be beneficial to fix unwanted resource access.

If you want to implement this in .NET Core, you can speed up things by using the Arcus NuGet package as described [here](https://webAPI.arcus-azure.net/features/security/auth/shared-access-key).

### Basic Authentication
Basic authentication is the first level of authentication you can apply to distinct users. How this works is standardized.

The users needs to send an `Authorization` header with value
```
Basic {username:password}
```
where the token `{username:password}` string is a base64 encoding of the actual value.

Again, the server validates the incoming request with an `Authentication Filter` and defines if the user has access or not.

Note that this solution is still very vulnerable to man in the middle attacks as the key the user sends is every time exactly the same.
So having a policy to frequently change the password might be a good added value in this system.

### TLS Mutual Authentication
Having a system with Client certificates is a very good way to authenticate the user and by so make your API more secure.

The TLS Mutual authentication does require the user to have a specific certificate given by preferabbly a specific authority.

When the user sends its request including the client certificate the server can then decrypt the certificate on it's side and validate various properties decided on design phase of the API again in an `Authentication Filter`.

Please keep in mind to use validation properties based on things that cannot be changed in a certificate. By this I think about:
- Certificate Thumprint
- Certificate Chain
- Certificate Serial
- Validity (Expiration time)

Next to that, with this, it is important to have a good framework to update all the certificate you have in your collection as a certificate is only valid for usually a year. It is also a recommendation to not extend this as this might end up in a security risk if the certificate is received by an unwanted party.

The advantage over the previously explained options is that this can give you a certainty over a user, and if the certificates are securely passed from the one party to the other, you have a very secure system.

If you want to implement this in .NET Core, you can speed up things by using the Arcus NuGet package as described [here](https://webAPI.arcus-azure.net/features/security/auth/certificate).

### OAuth2
OAuth2 has various implementations. From IdentityServer to manage the users/secrets by yourself or Facebook, Google or Microsoft (with Azure AD) where the whole user management is done by the OAuth2 provider itself.

This setup might require you the most effort but is also the most secure (if you choose only one).

The principle is that the user will authenticate to the OAuth2 Authentication server. The server will validate if your credentials are valid credentials for this certain application. If that is the case, the authentication server will return a token that gives access to an API.  Consumers of the API must add this (JWT) token in an `Authorization`-header to the request. The API can then validate if:
- The user is authentication with the right Authorization Server
- What the email & basic information of the user is.
- The roles the user has (based on claims)

And do the appropriate action custom made for that user.

Bearer tokens can only be generated by the authentication server so there is no security risk there.
Usually a token is only valid for x amount of time and will expire. Every time a user wants to start a new session he needs to generate a new bearer token via the Authentication server.

Usually within the Authentication Server you can easily add and remove users for your application.

## Azure Specifics
Azure can give you some out of the box features to improve the security of your system. In the list below you can see some examples how this can help.

### API Management
- HTTPS (TLS 1.2) is by default there on API management
- [IP-Filtering can be enabled via policy](https://docs.microsoft.com/en-us/azure/API-management/API-management-access-restriction-policies#RestrictCallerIPs)
- [Client certificate validation can be done via policies](https://docs.microsoft.com/en-us/azure/API-management/API-management-howto-mutual-certificates-for-clients)
- [API-Keys can be added via a policy](https://docs.microsoft.com/en-us/azure/API-management/API-management-access-restriction-policies#CheckHTTPHeader)
- [Basic authentication validation can be added via a policy](https://github.com/Azure/api-management-policy-snippets/blob/master/examples/Perform%20basic%20authentication.policy.xml)
- [API management also has a system of users and subscribers based on API-Keys which allows you to rotate keys, and handle creation and disabling of users by a user interface/powershell scripts.](https://docs.microsoft.com/en-us/azure/API-management/API-management-subscriptions)

:warning: API Management will secure the access between the user and API Management. This does not secure the communication between API Management and your upstream backend services. If for some reason a user has access to the **direct** url of the API, security added over here is bypassed!

### Web App
- [Https only can be enabled on Web Apps as a settings](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-custom-ssl#enforce-https)
- [Client certificate validation can be required as a setting](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-custom-ssl#enforce-tls-versions)
    - NOTE that this only requires a client certificate, and this does NOT validate if the certificate is the certificate you want to have access. This is something to be implemented within the API/API-M
- [Azure AD validation can be done by a tick of a box](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-auth-aad)
    - NOTE, that also this does not validate if that user is allowed, this just makes sure that you can identify the user in your code based on the accesstoken in the `Authentication`-header in your API.

### Managed Service Identity 
Service to service communication can be done via Managed Service Identity.
This allows to verify with Azure AD the ID of the service connecting to the other service. This is a complete password free setup as these things are purely handled by Azure itself

### Azure Functions
- [Https only can be enabled on Web Apps as a settings](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-custom-ssl#enforce-https)
- [Client certificate validation can be required as a setting](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-custom-ssl#enforce-tls-versions)
    - NOTE that this only requires a client certificate, and this does NOT validate if the certificate is the certificate you want to have access. This is something to be implemented within the API/API-M
- [Azure AD validation can be done by a tick of a box](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-auth-aad)
    - NOTE, that also this does not validate if that user is allowed, this just makes sure that you can identify the user in your code based on the accesstoken in the `Authentication`-header in your API.
