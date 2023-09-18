# <img src="https://github.com/pip-services/pip-services/raw/master/design/Logo.png" alt="Pip.Services Logo" style="max-width:30%"> <br/> Component Definitions for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit.

The Components module contains standard component definitions that can be used to build applications and services.

The module contains the following packages:
- **Auth** - credentials parameters and credential stores
- **Build** - component factories framework
- **Config** - configuration readers
- **Connect** - connection parameters and discovery services
- **Count** - performance counters
- **Info** - context information
- **Lock** - distributed locks
- **Log** - logging components

<a name="links"></a> Quick links:

* [Logging](https://www.pipservices.org/recipies/logging)
* [Configuration](https://www.pipservices.org/recipies/configuration) 
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)

## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Components
```

Example how to use Logging and Performance counters.
Here we are going to use CompositeLogger and CompositeCounters components.
They will pass through calls to loggers and counters that are set in references.

```cs
using PipServices4.Commons.Config;
using PipServices4.Commons.Refer;
using PipServices4.Components.Log;
using PipServices4.Components.Count;

class MyComponent : IConfigurable, IReferenceable
{
  private CompositeLogger _logger = new CompositeLogger();
  private CompositeCounters _counters = new CompositeCounters();

  public void Configure(ConfigParams config)
  {
    this._logger.Configure(config);
  }

  public void SetReferences(IReferences references)
  {
    this._logger.SetReferences(references);
    this._counters.SetReferences(references);
  }

  public void MyMethod(string correlationId)
  {
    try
      {
        this._logger.Trace(correlationId, "Executed method mycomponent.mymethod");
        this._counters.Increment("mycomponent.mymethod.exec_count", 1);
        Timing timing = this._counters.BeginTiming("mycomponent.mymethod.exec_time");
        ...
        timing.EndTiming();
      }
    catch (Exception ex)
    {
      this._logger.Error(correlationId, ex, "Failed to execute mycomponent.mymethod");
      this._counters.Increment("mycomponent.mymethod.error_count", 1);
      ...
    }
  }
}
```

Example how to get connection parameters and credentials using resolvers.
The resolvers support "discovery_key" and "store_key" configuration parameters
to retrieve configuration from discovery services and credential stores respectively.

```cs
using System.Threading.Tasks;
using PipServices4.Commons.Config;
using PipServices4.Commons.Refer;
using PipServices4.Commons.Run;
using PipServices4.Components.Connect;
using PipServices4.Components.Auth;


class MyComponent:IConfigurable, IReferenceable, IOpenable
{
  private ConnectionResolver _connectionResolver = new ConnectionResolver();
  private CredentialResolver _credentialResolver = new CredentialResolver();

  public void Configure(ConfigParams config)
  {
    this._connectionResolver.Configure(config);
    this._credentialResolver.Configure(config);
  }

  public void SetReferences(IReferences references)
  {
    this._connectionResolver.SetReferences(references);
    this._credentialResolver.SetReferences(references);
  }

  ...

  public Task OpenAsync(string correlationId)
  {
      Task task = Task.Factory.StartNew(async () =>
      {
          ConnectionParams _connectionParams = new ConnectionParams();

          ConnectionParams connection = await this._connectionResolver.ResolveAsync(correlationId);
          CredentialParams credential = await this._credentialResolver.LookupAsync(correlationId);

          string host = connection.Host;
          int port = connection.Port;
          string username = credential.Username;
          string password = credential.Password;

          ...
      });

      return task;
  }
}

// Using the component
MyComponent myComponent = new MyComponent();

myComponent.Configure(ConfigParams.FromTuples(
    "connection.host", "localhost",
    "connection.port", 1234,
    "credential.username", "anonymous",
    "credential.password", "pass123"
));

myComponent.OpenAsync(null);
```

Example how to use caching and locking.
Here we assume that references are passed externally.

```cs
using PipServices4.Commons.Refer;
using PipServices4.Components.Lock;
using PipServices4.Components.Cache;


class MyComponent: IReferenceable
{
  private ICache _cache;
  private Lock _lock;
        
  public void SetReferences(IReferences references)
  {
    this._cache = references.GetOneRequired<ICache>(new Descriptor("*", "cache", "*", "*", "1.0"));
    this._lock = references.GetOneRequired<Lock>(new Descriptor("*", "lock", "*", "*", "1.0"));
  }

  public async void MyMethod(string correlationId) 
  {
    // First check cache for result
    string result = await this._cache.RetrieveAsync<string>(correlationId, "mykey");

    // Lock..
    this._lock.AcquireLock(correlationId, "mykey", 1000, 1000);

    // Do processing
    ...

    // Store result to cache async
    this._cache.StoreAsync<string>(correlationId, "mykey", result, 3600000);

    // Release lock async
    this._lock.ReleaseLock(correlationId, "mykey");
  }
}

// Use the component
MyComponent myComponent = new MyComponent();

myComponent.SetReferences(References.FromTuples(
  new Descriptor("pip-services", "cache", "memory", "default", "1.0"), new MemoryCache(),
  new Descriptor("pip-services", "lock", "memory", "default", "1.0"), new MemoryLock()
));

myComponent.MyMethod(null);
```

If you need to create components using their locators (descriptors) implement 
component factories similar to the example below.

```cs
using PipServices4.Components.Build;
using PipServices4.Commons.Refer;


class MyFactory : Factory
{
    public static Descriptor myComponentDescriptor = new Descriptor("myservice", "mycomponent", "default", "*", "1.0");

    public MyFactory() : base()
    {
        this.RegisterAsType(MyFactory.myComponentDescriptor, MyComponent);
    }
}

// Using the factory
MyFactory myFactory = new MyFactory();

MyComponent myComponent1 = myFactory.Create(new Descriptor("myservice", "mycomponent", "default", "myComponent1", "1.0");
MyComponent myComponent2 = myFactory.Create(new Descriptor("myservice", "mycomponent", "default", "myComponent2", "1.0");

...
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
- **Andrew Harrington**
- **Volodymyr Tkachenko**
- **Mark Zontak**

Many thanks to contibutors, who put their time and talant into making this project better:
- **Nick Jimenez**, BootBarn Inc.