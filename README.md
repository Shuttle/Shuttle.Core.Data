# Shuttle.Core.Data

```
PM> Install-Package Shuttle.Core.Data
```

Provides a thin abstraction over ADO.NET.

# Overview

The `Shuttle.Core.Data` package provides a thin abstraction over ADO.NET by making use of the `DbProviderFactories` (see `Shuttle.Core.Data.SqlClient` for .Net Core Provider Factory adapter).  Even though it provides object/relational mapping mechanisms it is in no way an ORM.

## Configuration

### Connections

Connections may be added by providing all the required information:

```c#
services.AddDataAccess(builder => 
{
	builder.AddConnection(name, providerName, connectionString);
});
```

A connection may also be added by omitting the `connectionString`, in which case it will be read from the `ConnectionStrings` section:

```c#
services.AddDataAccess(builder => 
{
	builder.AddConnectionString(name, providerName);
});
```

### Options

The relevant options may be set using the builder:

```c#
services.AddDataAccess(builder => 
{
	builder.Options.CommandTimeout = timeout;
	builder.Options.DatabaseContextFactory = new DatabaseContextFactoryOptions
    {
        DefaultConnectionStringName = "connection-string-name",
		// -- or --
        DefaultProviderName = "provider-name",
        DefaultConnectionString = "connection-string"
    }
});
```

The default JSON settings structure is as follows:

```json
{
	"Shuttle": {
		"DataAccess": {
			"CommandTimeout": 25,
			"DatabaseContextFactory":
			{
				"DefaultConnectionStringName": "connection-string-name",
				// or
				"DefaultProviderName": "provider-name",
				"DefaultConnectionString": "connection-string"
			}
		} 
	}
}
```

# IDatabaseContextFactory

In order to access a database we need a database connection.  A database connection is represented by an `IDatabaseContext` instance that may be obtained by using an instance of an `IDatabaseContextFactory` implementation.

The `DatabaseContextFactory` implementation makes use of an `IDbConnectionFactory` implementation which creates a `System.Data.IDbConnection` by using the provider name and connection string.  An `IDbCommandFactory` creates a `System.Data.IDbCommand` by using an `IDbConnection` instance.  The `DatabaseContextFactory` also requires an instance of an `IDatabaseContextCache` that stores connections and is assigned to the `DatabaseContext` in order to obtain the active connection.

``` c#
var factory = provider.GetRequiredService<DatabaseContextFactory>();

using (var context = factory.Create("connection-name"))
{
	// database interaction
}

using (var context = factory
	.Create("System.Data.SqlClient", 
		"Data Source=.\sqlexpress;Initial Catalog=Shuttle;Integrated Security=SSPI"))
{
	// database interaction
}

using (var context = factory.Create(existingIDbConnection))
{
	// database interaction
}
```

# IDatabaseGateway

The `DatabaseGateway` is used to execute `IQuery` instances in order return data from, or make changes to, the underlying data store.  If there is no active open `IDatabaseContext` returned by the `DatabaseContext.Current` and `InvalidOperationException` will be thrown.

The following section each describe the methods available in the `IDatabaseGateway` interface.

## GetReader

``` c#
IDataReader GetReader(IQuery query);
```

Returns an `IDataReader` instance for the given `select` statement:

``` c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	var reader = gateway.GetReader(RawQuery.Create("select Id, Username from dbo.Member"));
}
```

## Execute

``` c#
int Execute(IQuery query);
```

Executes the given query and returns the number of rows affected:

``` c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	gateway.Execute(RawQuery.Create("delete from dbo.Member where Username = 'mr.resistor'"));
}
```

## GetScalar

```c#
T GetScalar<T>(IQuery query);
```

Get the scalar value returned by the `select` query.  The query shoud return only one value (scalar):

```c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	var username = gateway.GetScalar<string>(
		RawQuery.Create("select Username from dbo.Member where Id = 10")
	);
	
	var id = gateway.GetScalar<int>(
		RawQuery.Create("select Id from dbo.Member where Username = 'mr.resistor'")
	);
}
```

## GetDataTable

``` c#
DataTable GetDataTable(IQuery query);
```

Returns a `DataTable` containing the rows returned for the given `select` statement.

``` c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	var table = gateway.GetDataTable(RawQuery.Create("select Id, Username from dbo.Member"));
}
```

## GetRows

``` c#
IEnumerable<DataRow> GetRows(IQuery query);
```

Returns an enumerable containing the `DataRow` instances returned for a `select` query:

``` c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	var rows = gateway.GetRows(RawQuery.Create("select Id, Username from dbo.Member"));
}
```

## GetRow

``` c#
DataRow GetRow(IQuery query);
```

Returns a single `DataRow` containing the values returned for a `select` statement that returns exactly one row:

``` c#
var factory = DatabaseContextFactory.Default();
var gateway = new DatabaseGateway();

using (var context = factory.Create("connection-name"))
{
	var row = gateway.GetRow(
		RawQuery.Create("select Id, Username, EMail, DateActivated from dbo.Member where Id = 10")
	);
}
```

# IDataRepository

An `IDataRepository<T>` implementation is responsible for returning a hydrated object.  To this end you make use of the `DataReposity<T>` class that takes a `IDatabaseGateway` instance along with a `IDataRowMapper<T>` used to create the hydrated instance.

The following methods can be used to interact with your object type.

## FetchItems

``` c#
IEnumerable<T> FetchItems(IQuery query);
```

Uses the `select` clause represented by the `IQuery` instance to create a list of objects of type `T`.  The `select` clause will need to select all the required columns and will, typically, return more than one instance.

## FetchItem

``` c#
T FetchItem(IQuery query);
```

Returns a single object instance of type `T` that is hydrated using the data returned from the `select` clause represented by the `IQuery` instance.

## FetchMappedRows

``` c#
IEnumerable<MappedRow<T>> FetchMappedRows(IQuery query);
```

This is similar to the `FetchItems` method but instead returns a list of `MappedRow<T>` instances.  Uses the `select` clause represented by the `IQuery` instance to create a list of `MappedRow` instances of type `T`.  The `select` clause will need to select all the required columns and will, typically, return more than one instance.

## FetchMappedRow

``` c#
MappedRow<T> FetchMappedRow(IQuery query);
```

Similar to the `FetchItem` method but instead return a `MappedRow<T>` instance that is hydrated using the data returned from the `select` clause represented by the `IQuery` instance.

## Contains

``` c#
bool Contains(IQuery query);
```

Returns `true` is the `IQuery` instance `select` clause returns an `int` scalar that equals `1`; else returns `false`.

# IQuery

An `IQuery` represent a database query that can be executed against the relevant database type.  There is only one method that needs to be implemented:

``` c#
void Prepare(IDbCommand command);
```

This should ensure that the given `IDbCommand` is configured for execution by setting the relvant command attributes and parameters.

# IQueryParameter: IQuery

An `IQueryParameter` inherits the `IQuery` interface and extends it by allowing you to add parameters to a query by specifying an `IMappedColumn` (see below) instance along with the value for the parameter.

There are two implementations of this interface.

## RawQuery

The `RawQuery` enables you to create any query using the native language structure:

``` c#
var query = RawQuery.Create("select UserName from dbo.Member where Id = @Id")
	.AddParameterValue(new MappedColumn<Guid>("Id", DbType.Guid), 
		new Guid('{75208260-CF93-454E-95EC-FE1903F3664E}'));
```

## ProcedureQuery

The `ProcedureQuery` is used to execute a stored procedure:

``` c#
var query = ProcedureQuery.Create("uspMemberById")
	.AddParameterValue(new MappedColumn<Guid>("Id", DbType.Guid), 
		new Guid('{75208260-CF93-454E-95EC-FE1903F3664E}'));
```

# MappedColumn

Typically you would not want to create a `MappedColumn` each time you need it and these are also quite fixed.  A column mapping can, therefore, by defined statically:

``` c#
using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderColumns
    {
        public static readonly MappedColumn<Guid> Id =
            new MappedColumn<Guid>("Id", DbType.Guid);

        public static readonly MappedColumn<string> OrderNumber =
            new MappedColumn<string>("OrderNumber", DbType.String, 20);

        public static readonly MappedColumn<string> OrderDate =
            new MappedColumn<string>("OrderDate", DbType.DateTime);

        public static readonly MappedColumn<string> CustomerName =
        new MappedColumn<string>("CustomerName", DbType.String, 65);

        public static readonly MappedColumn<string> CustomerEMail =
            new MappedColumn<string>("CustomerEMail", DbType.String, 130);
    }
}
```

There are quite a few options that you can set on the `MappedColumn` in order to represent your column properly.

## MapFrom

``` c#
public T MapFrom(DataRow row)
```

This will return the typed value of the specified column as contained in the passed-in `DataRow`.

# IDataRowMapper

You use this interface to implement a mapper for a `DataRow` that will result in an object of type `T`:

``` c#
using System.Data;
using Shuttle.Core.Data;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessMapper : IDataRowMapper<OrderProcess>
    {
        public MappedRow<OrderProcess> Map(DataRow row)
        {
            var result = new OrderProcess(OrderProcessColumns.Id.MapFrom(row))
            {
                CustomerName = OrderProcessColumns.CustomerName.MapFrom(row),
                CustomerEMail = OrderProcessColumns.CustomerEMail.MapFrom(row),
                OrderId = OrderProcessColumns.OrderId.MapFrom(row),
                InvoiceId = OrderProcessColumns.InvoiceId.MapFrom(row),
                DateRegistered = OrderProcessColumns.DateRegistered.MapFrom(row),
                OrderNumber = OrderProcessColumns.OrderNumber.MapFrom(row)
            };

            return new MappedRow<OrderProcess>(row, result);
        }
    }
}
```

# MappedRow

A `MappedRow` instance contains bother a `DataRow` and the object that the `DataRow` mapped to.  

This may be useful in situation where the `DataRow` contains more information that is available on the object.  An example may be an `OrderLine` where the `DataRow` contains the `OrderId` column but the `OrderLine` object does not.  In order to still be able to make that association it is useful to have both available.

# IAssembler

An `IAssembler` implementation is used to create multiple mappings with as few calls as possible.  An example may be where we perform two `select` queries; one to get 3 orders and another to get the order lines belonging to those 3 orders.

> `select OrderId, OrderNumber, OrderDate from dbo.Order where OrderId in (2, 6, 44)`

| Order Id | Order Number | Order Date |
| --- | --- | --- |
| 2 | ORD-002 | 14 Feb 2016 |
| 6 | ORD-006 | 24 Mar 2016 |
| 44 | ORD-044 | 4 Apr 2016 |

> `select OrderId, Product, Quantity from dbo.OrderLine where OrderId in (2, 6, 44)`

| Order Id | Product | Quantity |
| --- | --- | --- |
| 2 | Red Socks | 2 |
| 2 | Blue Socks | 3 |
| 6 | Sports Towel | 1 |
| 6 | Squash Racquet | 1 |
| 6 | Squash Ball | 3 |
| 44 | Vaughn's DDD Book | 1 |
| 44 | Shuttle.Sentinel License | 5 |

Using a `MappedData` instance we can keep adding the `MappedRow` instances to the `MappedData` and then have the assembler return the three `Order` aggregates:

``` c#
public class OrderAssembler : IAssembler<Order>
{
	public IEnumerable<Order> Assemble(MappedData data)
	{
		var result = new List<Order>();

		foreach (var orderRow in data.MappedRows<Order>())
		{
			var order = orderRow.Result;

			foreach (var orderLineRow in data.MappedRows<OrderLine>())
			{
				if (orderLineRow.Row["OrderId"].Equals(order.OrderId))
				{
					order.AddLine(orderLineRow.Result);
				}
			}

			result.Add(order);
		}

		return result;
	}
}
```
