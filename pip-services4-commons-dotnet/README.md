# <img src="https://uploads-ssl.webflow.com/5ea5d3315186cf5ec60c3ee4/5edf1c94ce4c859f2b188094_logo.svg" alt="Pip.Services Logo" width="200"> <br/> Portable abstractions and patterns for .NET

This module is a part of the [Pip.Services](http://pip.services.org) polyglot microservices toolkit.
It provides a set of basic patterns used in microservices or backend services.
Also the module implemenets a reasonably thin abstraction layer over most fundamental functions across
all languages supported by the toolkit to facilitate symmetric implementation.

The module contains the following packages:

- **Commands** - commanding and eventing patterns
- **Config** - configuration framework
- **Convert** - soft value converters
- **Data** - data patterns
- **Errors** - application errors
- **Random** - random data generators
- **Refer** - locator (IoC) pattern
- **Reflect** - reflection framework
- **Run** - execution framework
- **Validate** - validation framework

<a name="links"></a> Quick links:

* [Configuration Pattern](https://www.pipservices.org/recipies/configuration) 
* [Locator Pattern](https://www.pipservices.org/recipies/references)
* [Component Lifecycle](https://www.pipservices.org/recipies/component-lifecycle)
* [Components with Active Logic](https://www.pipservices.org/recipies/active-logic)
* [Data Patterns](https://www.pipservices.org/recipies/memory-persistence)
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)


## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Commons
```

Then you are ready to start using the Pip.Services patterns to augment your backend code.

For instance, here is how you can implement a component, that receives configuration, get assigned references,
can be opened and closed using the patterns from this module.

```cs
using PipServices4.Commons;
using PipServices4.Commons.Config;
using PipServices4.Commons.Refer;
using PipServices4.Commons.Run;

public class MyComponentA : IConfigurable, IReferenceable, IOpenable
{
    private string _param1 = "ABC";
    private int _param2 = 123;
    private MyComponentB _anotherComponent;
    private bool _opened = true;

    public void Configure(ConfigParams config)
    {
        this._param1 = config.GetAsStringWithDefault("param1", this._param1);
        this._param2 = config.GetAsIntegerWithDefault("param2", this._param2);
    }

    public void SetReferences(IReferences references)
    {
        this._anotherComponent = references.GetOneRequired<MyComponentB>(
            new Descriptor("myservice", "mycomponent-b", "*", "*", "1.0")
        );
    }

    public bool IsOpen()
    {
        return this._opened;
    }

    public Task OpenAsync(string correlationId)
    {
        Task task = Task.Factory.StartNew(() => 
        {
            this._opened = true;
            Console.WriteLine("MyComponentA has been opened .");
        });

        return task;   
    }

    public Task CloseAsync(string correlationId)
    {
        Task task = Task.Factory.StartNew(() =>
        {
            this._opened = true;
            Console.WriteLine("MyComponentA has been closed.");
        });

        return task;        
    }
}

```

Then here is how the component can be used in the code

```cs
using PipServices4.Commons.Config;
using PipServices4.Commons.Refer;

MyComponentA myComponentA = new MyComponentA();

// Configure the component
myComponentA.Configure(ConfigParams.FromTuples(
    "param1", "XYZ",
    "param2", "987"
));

// Set references to the component
myComponentA.SetReferences(References.FromTuples(
    new Descriptor("myservice", "mycomponent-b", "default", "default", "1.0"), myComponentB
));

// Open the component
myComponentA.OpenAsync("123");
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