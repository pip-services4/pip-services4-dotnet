# <img src="https://github.com/pip-services/pip-services/raw/master/design/Logo.png" alt="Pip.Services Logo" style="max-width:30%"> <br/> GRPC Calls for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit.

The grpc module is used to organize synchronous data exchange using calls through the gRPC protocol. It has implementations of both the server and client parts.

The module contains the following packages:

- **Build** - factories for creating gRPC services
- **Clients** - basic client components that use the gRPC protocol and Commandable pattern through gRPC
- **Services** - basic service implementations for connecting via the gRPC protocol and using the Commandable pattern via gRPC

<a name="links"></a> Quick links:

* [Configuration](https://www.pipservices.org/recipies/configuration)
* [Protocol buffer](https://github.com/pip-services4-dotnet/pip-services4-grpc-dotnet/blob/main/src/Protos/commandable.proto)
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-grpc-dotnet/globals.html)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)


## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Grpc
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

The dotnet.js version of Pip.Services is created and maintained by:
- **Sergey Seroukhov**

The documentation is written by:
- **Mark Makarychev**
