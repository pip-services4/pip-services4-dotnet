# <img src="https://github.com/pip-services/pip-services/raw/master/design/Logo.png" alt="Pip.Services Logo" style="max-width:30%"> <br/> IoC container for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit. It provides an inversion-of-control (IoC) container to facilitate the development of services and applications composed of loosely coupled components.

The module containes a basic in-memory container that can be embedded inside a service or application, or can be run by itself.
The second container type can run as a system level process and can be configured via command line arguments.
Also it can be used to create docker containers.

The containers can read configuration from JSON or YAML files use it as a recipe for instantiating and configuring components.
Component factories are used to create components based on their locators (descriptor) defined in the container configuration.
The factories shall be registered in containers or dynamically in the container configuration file.

The module contains the following packages:
- **Core** - Component container and container as a system process
- **Build** - Container default factory
- **Config** - Container configuration
- **Refer** - Container references


<a name="links"></a> Quick links:

* [Configuration](https://www.pipservices.org/recipies/configuration) 
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-container-dotnet)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)

## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Container
```

Create a factory to create components based on their locators (descriptors).

```cs
using PipServices4.Commons.Refer;
using PipServices4.Components.Build;


class MyFactory : Factory
{
    public static Descriptor myComponentDescriptor = new Descritor("myservice", "mycomponent", "default", "*", "1.0");

    public MyFactory() : base()
    {
        this.RegisterAsType(MyFactory.myComponentDescriptor, myComponent);
    }
}
```

Then create a process container and register the factory there. You can also register factories defined in other
modules if you plan to include external components into your container.

```cs
using PipServices4.Container;
using PipServices4.Rpc.Build;

class MyProcess : ProcessContainer
{
    public MyProcess() : base("myservice", "My service running as a process")
    {
        this._factories.Add(new DefaultRpcFactory());
        this._factories.Add(new MyFactory());
    }
}
```

Define YAML configuration file with components and their descriptors.
The values for the templating engine are defined via process command line arguments or via environment variables.
Support for environment variables works well in docker or other containers like AWS Lambda functions.

```yaml
---
# Context information
- descriptor: "pip-services:context-info:default:default:1.0"
  name: myservice
  description: My service running in a process container

# Console logger
- descriptor: "pip-services:logger:console:default:1.0"
  level: {{LOG_LEVEL}}{{^LOG_LEVEL}}info{{/LOG_LEVEL}}

# Performance counters that posts values to log
- descriptor: "pip-services:counters:log:default:1.0"
  
# My component
- descriptor: "myservice:mycomponent:default:default:1.0"
  param1: XYZ
  param2: 987
  
{{#if HTTP_ENABLED}}
# HTTP endpoint version 1.0
- descriptor: "pip-services:endpoint:http:default:1.0"
  connection:
    protocol: "http"
    host: "0.0.0.0"
    port: {{HTTP_PORT}}{{^HTTP_PORT}}8080{{/HTTP_PORT}}

 # Default Status
- descriptor: "pip-services:status-service:http:default:1.0"

# Default Heartbeat
- descriptor: "pip-services:heartbeat-service:http:default:1.0"
{{/if}}
```

To instantiate and run the container we need a simple process launcher.

```cs
try
{
    MyProcess proc = new MyProcess();

    ConfigParams param = ConfigParams.FromValue(Environment.GetEnvironmentVariables());

    proc.ReadConfigFromFile("123", "./config/config.yml", param);
    proc.RunAsync(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
```

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
dotnet build src/src.csproj
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
- **Volodymyr Tkachenko**
- **Alex Mazur**
