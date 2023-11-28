# <img src="https://uploads-ssl.webflow.com/5ea5d3315186cf5ec60c3ee4/5edf1c94ce4c859f2b188094_logo.svg" alt="Pip.Services Logo" width="200"> <br/> PostgreSQL components for .NET

This module is a part of the [Pip.Services](http://pipservices.org) polyglot microservices toolkit. It provides a set of components to implement PostgreSQL persistence.


The module contains the following packages:
- **Build** - Factory to create PostreSQL persistence components.
- **Connect** - Connection component to configure PostgreSQL connection to database.
- **Persistence** - abstract persistence components to perform basic CRUD operations.

<a name="links"></a> Quick links:

* [Configuration](https://www.pipservices.org/recipies/configuration)
* [API Reference](https://pip-services4-dotnet.github.io/pip-services4-postgres-dotnet/)
* [Change Log](CHANGELOG.md)
* [Get Help](https://www.pipservices.org/community/help)
* [Contribute](https://www.pipservices.org/community/contribute)

## Use

Install the dotnet package as
```bash
dotnet add package PipServices4.Postgres
```

As an example, lets create persistence for the following data object.

```cs
using PipServices4.Commons.Data;

class MyObject : IIdentifiable<string>
{
    public string Id { get; set; }
    public string key;
    public int value;
}

```

The persistence component shall implement the following interface with a basic set of CRUD operations.

```cs
interface IMyPersistance
{
    void GetPageByFilter(string correlationId, FilterParams filter, PagingParams paging);

    void GetOneById(string correlationId, string id);

    void GetOneByKey(string correlationId, string id);

    void Create(string correlationId, MyObject item);

    void Update(string correlationId, MyObject item);

    void DeleteById(string correlationId, string id);
}
```

To implement postgresql persistence component you shall inherit `IdentifiablePostgresPersistence`. 
Most CRUD operations will come from the base class. You only need to override `GetPageByFilter` method with a custom filter function.
And implement a `GetOneByKey` custom persistence method that doesn't exist in the base class.

```cs
class MyPostgresPersistence: IdentifiablePostgresPersistence<MyObject, string>
{
    public MyPostgresPersistence(): base("myobjects")
    {
        this.AutoCreateObject("CREATE TABLE myobjects (id VARCHAR(32) PRIMARY KEY, key VARCHAR(50), value VARCHAR(255)");

        IndexOptions options = new IndexOptions();
        Dictionary<string, bool> keys = new Dictionary<string, bool>{
            {"unique", true},
        };

        this.EnsureIndex("myobjects_key", keys, options);
    }

    private string ComposeFilter(FilterParams filter)
    {
        filter = filter != null ? filter : new FilterParams();

        List<string> criteria = new List<string>();

        string id = filter.GetAsNullableString("id");
        if (id != null)
            criteria.Add("id='" + id + "'");
                
        string tempIds = filter.GetAsNullableString("ids");
        if (tempIds != null)
        {
            string[] ids = tempIds.Split(",");
            filters.Add("id IN ('" + string.Join("','", ids) + "')");
        }

        string key = filter.GetAsNullableString("key");
        if (key != null)
            criteria.Add("key='" + key + "'");

        return criteria.Count > 0 ? string.Join(" AND ", criteria) : null;
    }

    public async Task<DataPage<MyObject>> GetPageByFilter(string correlationId, FilterParams filter, PagingParams paging)
    {
        return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, select: "id");
    }

    public async Task<MyObject> GetOneByKey(string correlationId, string key)
    {
        string query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE \"key\"=$1";
        List<string> param = new List<string> { key };

        var result = await this.ExecuteReaderAsync(query, param);
        AnyValueMap item = result != null && result[0] != null ? result[0]: null;    

        if (item == null)
            this._logger.Trace(correlationId, "Nothing found from %s with key = %s", this._tableName, key);
        else
            this._logger.Trace(correlationId, "Retrieved from %s with key = %s", this._tableName, key);

        item = this.ConvertToPublic(item);

        return item;
    }
}
```

Alternatively you can store data in non-relational format using `IdentificableJsonPostgresPersistence`.
It stores data in tables with two columns - `id` with unique object id and `data` with object data serialized as JSON.
To access data fields you shall use `data->'field'` expression or `data->>'field'` expression for string values.

```cs
class MyPostgresPersistence : IdentifiableJsonPostgresPersistence<MyObject, string>
{
    public MyPostgresPersistence() : base("myobjects")
    {
        this.EnsureTable("VARCHAR(32)", "JSONB");

        IndexOptions options = new IndexOptions();
        Dictionary<string, bool> keys = new Dictionary<string, bool>{
            { "data->>'key'", true },
        };

        this.EnsureIndex("myobjects_key", keys, options);
    }

    private string ComposeFilter(FilterParams filter)
    {
        filter = filter != null ? filter : new FilterParams();
        List<string> criteria = new List<string>();

        string id = filter.GetAsNullableString("id");
        if (id != null)
            criteria.Add("data->>'id'='" + id + "'");

        string tempIds = filter.GetAsNullableString("ids");
        if (tempIds != null)
        {
            string[] ids = tempIds.Split(",");
            filters.Add("data->>'id' IN ('" + string.Join("','", ids) + "')");
        }

        string key = filter.GetAsNullableString("key");
        if (key != null)
            criteria.Add("data->>'key'='" + key + "'");

        return criteria.Count > 0 ? string.Join(" AND ", criteria) : null;

    }

    public async DataPage<MyObject> GetPageByFilter(string correlationId, FilterParams filter, PagingParams paging)
    {
        return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id").Result;
    }

    public async Task<MyObject> GetOneByKey(string correlationId, string key)
    {
        string query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE data->>'key'=$1";
        List<string> param = new List<string> { key };

        var result = await this.ExecuteReaderAsync(query, param);
        AnyValueMap item = result != null && result[0] != null ? result[0] : null;


        if (item == null)
            this._logger.Trace(correlationId, "Nothing found from %s with key = %s", this._tableName, key);
        else
            this._logger.Trace(correlationId, "Retrieved from %s with key = %s", this._tableName, key);

        item = this.ConvertToPublic(item);

        return item;
    }
}
```

Configuration for your microservice that includes postgresql persistence may look the following way.

```yaml
...
{{#if POSTGRES_ENABLED}}
- descriptor: pip-services:connection:postgres:con1:1.0
  connection:
    uri: {{{POSTGRES_SERVICE_URI}}}
    host: {{{POSTGRES_SERVICE_HOST}}}{{#unless POSTGRES_SERVICE_HOST}}localhost{{/unless}}
    port: {{POSTGRES_SERVICE_PORT}}{{#unless POSTGRES_SERVICE_PORT}}5432{{/unless}}
    database: {{POSTGRES_DB}}{{#unless POSTGRES_DB}}app{{/unless}}
  credential:
    username: {{POSTGRES_USER}}
    password: {{POSTGRES_PASS}}
    
- descriptor: myservice:persistence:postgres:default:1.0
  dependencies:
    connection: pip-services:connection:postgres:con1:1.0
  table: {{POSTGRES_TABLE}}{{#unless POSTGRES_TABLE}}myobjects{{/unless}}
{{/if}}
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
- **Denis Kuznetsov**

