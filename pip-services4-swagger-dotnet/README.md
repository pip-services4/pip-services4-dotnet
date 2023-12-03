# <img src="https://uploads-ssl.webflow.com/5ea5d3315186cf5ec60c3ee4/5edf1c94ce4c859f2b188094_logo.svg" alt="Pip.Services Logo" width="200"> <br/> Swagger UI for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit.

The swagger module provides a Swagger UI that can be added into microservices and seamlessly integrated with existing REST and Commandable HTTP services.

The module contains the following packages:
- **Build** - Swagger service factory
- **Services** - Swagger UI service

<a name="links"></a> Quick links:

* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)


## Use

Install the NuGet package as
```bash
dotnet add package PipServices4.Swagger
```

Develop a RESTful service component. For example, it may look the following way.
In the `Register` method we load an Open API specification for the service.
You can also enable swagger by default in the constractor by setting `_swaggerEnable` property.
```csharp
public class MyRestService: RestService 
{
    public MyRestService() 
    {
        _baseRoute = "myservice";
        _swaggerEnable = true;
    }

    private Task GreetingAsync(HttpRequest req, HttpResponse res, ClaimsPrincipal user, RouteData rd) 
    {
        var parameters = GetParameters(request);
        var name = parameters.GetAsNullableString("name");
        var response = "Hello, " + name + "!";
        await SendResultAsync(res, response);
    }
        
    public void Register()
    {
        RegisterRoute(
            "get", "/greeting", 
            new ObjectSchema(true)
                .WithRequiredProperty("name", TypeCode.String),
            GreetingAsync
        );
        
        RegisterOpenApiSpecFromFile("./src/Service/Services/myservice.yml");
    }
}
```

The Open API specification for the service shall be prepared either manually
or using [Swagger Editor](https://editor.swagger.io/)
```yaml
openapi: '3.0.2'
info:
  title: 'MyService'
  description: 'MyService REST API'
  version: '1'
paths:
  /myservice/greeting:
    get:
      tags:
        - myservice
      operationId: 'greeting'
      parameters:
      - name: correlation_id
        in: query
        description: Correlation ID
        required: false
        schema:
          type: string
      - name: name
        in: query
        description: Name of a person
        required: true
        schema:
          type: string
      responses:
        200:
          description: 'Successful response'
          content:
            application/json:
              schema:
                type: 'string'
```

Include Swagger service into `config.yml` file and enable swagger for your REST or Commandable HTTP services.
Also explicitely adding HttpEndpoint allows to share the same port betwee REST services and the Swagger service.
```yaml
---
...
# Shared HTTP Endpoint
- descriptor: "pip-services:endpoint:http:default:1.0"
  connection:
    protocol: http
    host: localhost
    port: 8080

# Swagger Service
- descriptor: "pip-services:swagger-service:http:default:1.0"

# My RESTful Service
- descriptor: "myservice:service:rest:default:1.0"
  swagger:
    enable: true
```

Finally, remember to add factories to your container, to allow it creating required components.
```csharp
...
using PipServices4.Rpc.Build
using PipServices4.Swagger.Build

public class MyProcess: ProcessContainer {
  public MyProcess()
    : base("myservice", "MyService microservice") 
  {    
    _factories.Add(new DefaultRpcFactory());
    _factories.Add(new DefaultSwaggerFactory());
    _factories.Add(new MyServiceFactory());
    ...
  }
}
```

Launch the microservice and open the browser to open the Open API specification at
[http://localhost:8080/greeting/swagger](http://localhost:8080/greeting/swagger)

Then open the Swagger UI using the link [http://localhost:8080/swagger](http://localhost:8080/swagger).
The result shall look similar to the picture below.

<img src="swagger-ui.png"/>

## Develop

For development you shall install the following prerequisites:
* Core .NET SDK 3.1+
* Visual Studio Code or another IDE of your choice
* Docker

Restore dependencies:
```bash
dotnet restore src/src.csproj
```

Compile the code:
```bash
dotnet build src/src/csproj
```

Run automated tests:
```bash
dotnet restore test/test.csproj
dotnet test test/test.csproj
```

Generate API documentation:
```bash
./docgen.ps1
```

Before committing changes run dockerized build and test as:
```bash
./build.ps1
./test.ps1
./clear.ps1
```

## Contacts

The .NET version of Pip.Services is created and maintained by:
- **Sergey Seroukhov**
