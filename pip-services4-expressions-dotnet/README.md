# <img src="https://github.com/pip-services/pip-services/raw/master/design/Logo.png" alt="Pip.Services Logo" style="max-width:30%"> <br/> Tokenizers, parsers and expression calculators for .NET

This module is a part of the [Pip.Services](http://pip.services.org) polyglot microservices toolkit.
It provides syntax and lexical analyzers and expression calculator optimized for repeated calculations.

The module contains the following packages:
- **Calculator** - Expression calculator
- **CSV** - CSV tokenizer
- **IO** - input/output utility classes to support lexical analysis
- **Tokenizers** - lexical analyzers to break incoming character streams into tokens
- **Variants** - dynamic objects that can hold any values and operators for them

<a name="links"></a> Quick links:

* [API Reference](https://pip-services4-node.github.io/pip-services4-expressions-dotnet/globals.html)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)

## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Expressions
```


The example below shows how to use expression calculator to dynamically
calculate user-defined expressions.

```cs
using PipServices4.Commons.Config;
using PipServices4.Commons.Refer;
using PipServices4.Commons.Run;
using PipServices4.Expressions.Calculator;
using PipServices4.Expressions.Calculator.Variables;
using PipServices4.Expressions.Variants;

ExpressionCalculator calculator = new ExpressionCalculator();

calculator.Expression = "A + b / (3 - Max(-123, 1)*2)";

VariableCollection vars = new VariableCollection();
vars.Add(new Variable("A", new Variant(1)));
vars.Add(new Variable("B", new Variant(3)));

Variant result = calculator.EvaluateUsingVariablesAsync(vars).Result;
Console.WriteLine("The result of the expression is " + result.ToString());
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
