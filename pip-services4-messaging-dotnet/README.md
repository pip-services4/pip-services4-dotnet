# <img src="https://uploads-ssl.webflow.com/5ea5d3315186cf5ec60c3ee4/5edf1c94ce4c859f2b188094_logo.svg" alt="Pip.Services Logo" width="200"> <br/> Asynchronous messaging for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit.

The Messaging module contains a set of interfaces and classes for working with message queues, as well as an in-memory message queue implementation. 

The module contains the following packages:

- **Build** - in-memory message queue factory
- **Queues** - contains interfaces for working with message queues, subscriptions for receiving messages from the queue, and an in-memory message queue implementation.

<a name="links"></a> Quick links:

* [Configuration](https://www.pipservices.org/recipies/configuration)
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-messaging-dotnet/)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)

## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Messaging
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
- **Alex Mazur**
- **Volodymyr Tkachenko**

Many thanks to contibutors, who put their time and talant into making this project better:
- **Nick Jimenez**, BootBarn Inc.
