# Shuttle.Core.Data

```
PM> Install-Package Shuttle.Core.Data
```

Provides an abstraction built directly on ADO.NET which falls within the Micro ORM space.

# Overview

***NOTE:*** Since a database connection is represented by a `IDatabaseContext` instance it is important to understand that this instance is not thread-safe.  It is therefore important to ensure that the `IDatabaseContext` instance is not shared between threads.  See the `DatabaseContextScope` to ensure thread-safe database context flow.

The `Shuttle.Core.Data` package provides a thin abstraction over ADO.NET by making use of the `DbProviderFactories`.  Even though it provides object/relational mapping mechanisms it is in no way a fully fledged ORM.

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
	
	builder.Options.DatabaseContextFactory.DefaultConnectionStringName = "connection-string-name";
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
			}
		} 
	}
}
```

# DatabaseContextScope

The `DatabaseContextService` contains a collection of the `DatabaseContext` instances created by `IDatabaseContextFactory`.  However, since the `DatabaseContextService` is a singleton the same collection will be used in all thread contexts.  This includes not only the same execution context, but also "peered" execution context running in parallel.

To enable an individual execution/thread context-specific collection which also enables async context flow wrap the initial database context creation in a new `DatabaseContextScope()`:

``` c#
using (new DatabaseContextScope())
{
	// database interaction
})
```

# IDatabaseContextFactory

In order to access a database we need a database connection.  A database connection is represented by an `IDatabaseContext` instance that may be obtained by using an instance of an `IDatabaseContextFactory` implementation.

The `DatabaseContextFactory` implementation makes use of an `IDbConnectionFactory` implementation which creates a `System.Data.IDbConnection` by using the provider name and connection string, which is obtained from the registered connection name.  An `IDbCommandFactory` creates a `System.Data.IDbCommand` by using an `IDbConnection` instance.

``` c#
var databaseContextFactory = provider.GetRequiredService<IDatabaseContextFactory>();

using (var databaseContext = databaseContextFactory.Create("connection-name"))
{
	// database interaction
}

// or, in async/await implementations

using (new DatabaseContextScope())
using (var databaseContext = databaseContextFactory.Create("connection-name"))
{
	// database interaction that will flow across threads
}
```

# IQuery

An `IQuery` encapsulates a database query that can be executed:

``` c#
void Prepare(IDbCommand command);
```

This should ensure that the given `IDbCommand` is configured for execution by setting the relvant command attributes and parameters.

```c#
IQuery AddParameter(IColumn column, object value);
```

This method is used to add a parameter to the query.  The `IColumn` instance is used to define the column type and the value is the value that should be used for the parameter.

## Query

The `Query` represents a `Text` command type:

``` c#
public Query(string commandText, CommandType commandType = CommandType.Text)
```

You can then add parameters to the query:

``` c#
query.AddParameter(new Column<Guid>("Id", DbType.Guid), new Guid('{75208260-CF93-454E-95EC-FE1903F3664E}'));
```

# Column

Typically you would not want to create a `Column` each time you need it and these are also quite fixed.  A column mapping can, therefore, by defined statically:

``` c#
using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderColumns
    {
        public static readonly Column<Guid> Id =
            new Column<Guid>("Id", DbType.Guid);

        public static readonly Column<string> OrderNumber =
            new Column<string>("OrderNumber", DbType.String, 20);

        public static readonly Column<string> OrderDate =
            new Column<string>("OrderDate", DbType.DateTime);

        public static readonly Column<string> CustomerName =
			new Column<string>("CustomerName", DbType.String, 65);

        public static readonly Column<string> CustomerEMail =
            new Column<string>("CustomerEMail", DbType.String); // size omitted
    }
}
```

There are quite a few options that you can set on the `Column` in order to represent your column properly.

## Value

``` c#
public T Value(DataRow row)
```

This will return the typed value of the specified column as contained in the passed-in `DataRow`.

# IDatabaseGateway

The `DatabaseGateway` is used to execute `IQuery` instances in order return data from, or make changes to, the underlying data store.  If there is no active open `IDatabaseContext` returned by the `DatabaseContextService.Current` an `InvalidOperationException` will be thrown.

The following sections each describe the methods available in the `IDatabaseGateway` interface.

## GetReaderAsync

``` c#
Task<IDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns an `IDataReader` instance for the given `query` statement:

``` c#
using (databaseContextFactory.Create("connection-name"))
{
	var reader = await gateway.GetReaderAsync(new Query("select Id, Username from dbo.Member"));
}
```

## ExecuteAsync

``` c#
Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken = default);
```

Executes the given query and returns the number of rows affected:

``` c#
using (databaseContextFactory.Create("connection-name"))
{
	await gateway.ExecuteAsync(new Query("delete from dbo.Member where Username = 'mr.resistor'"));
}
```

## GetScalarAsync

```c#
Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken = default);
```

Get the scalar value returned by the `select` query.  The query shoud return only one value (scalar):

```c#
using (var databaseContext = databaseContextFactory.Create("connection-name"))
{
	var username = await gateway.GetScalarAsync<string>(new Query("select Username from dbo.Member where Id = 10"));
	
	var id = await gateway.GetScalarAsync<int>(new Query.Create("select Id from dbo.Member where Username = 'mr.resistor'")	);
}
```

## GetDataTableAsync

``` c#
Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns a `DataTable` containing the rows returned for the given `select` query.

``` c#
using (databaseContextFactory.Create("connection-name"))
{
	var table = await gateway.GetDataTableAsync(new Query("select Id, Username from dbo.Member"));
}
```

## GetRowsAsync

``` c#
Task<IEnumerable<DataRow>> GetRowsAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns an enumerable containing the `DataRow` instances returned for a `select` query:

``` c#
using (databaseContextFactory.Create("connection-name"))
{
	var rows = await gateway.GetRowsAsync(new Query("select Id, Username from dbo.Member"));
}
```

## GetRowAsync

``` c#
Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns a single `DataRow` containing the values returned for a `select` statement that returns exactly one row:

``` c#
using (databaseContextFactory.Create("connection-name"))
{
	var row = await gateway.GetRowAsync(new Query("select Id, Username, EMail, DateActivated from dbo.Member where Id = 10")	);
}
```

# IDataRepository

An `IDataRepository<T>` implementation is responsible for returning a hydrated object.  To this end you make use of the `DataReposity<T>` class that takes a `IDatabaseGateway` instance along with a `IDataRowMapper<T>` used to create the hydrated instance.

The following methods can be used to interact with your object type.

## FetchItemsAsync

``` c#
Task<IEnumerable<T>> FetchItemsAsync(IQuery query, CancellationToken cancellationToken = default);
```

Uses the `select` clause represented by the `IQuery` instance to create a list of objects of type `T`.  The `select` clause will need to select all the required columns and will, typically, return more than one instance.

## FetchItemAsync

``` c#
Task<T> FetchItemAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns a single object instance of type `T` that is hydrated using the data returned from the `select` clause represented by the `IQuery` instance.

## FetchMappedRowsAsync

``` c#
Task<MappedRow<T>> FetchMappedRowsAsync(IQuery query, CancellationToken cancellationToken = default);
```

This is similar to the `FetchItems` method but instead returns a list of `MappedRow<T>` instances.  Uses the `select` clause represented by the `IQuery` instance to create a list of `MappedRow` instances of type `T`.  The `select` clause will need to select all the required columns and will, typically, return more than one instance.

## FetchMappedRowAsync

``` c#
Task<IEnumerable<MappedRow<T>>> FetchMappedRowAsync(IQuery query, CancellationToken cancellationToken = default);
```

Similar to the `FetchItem` method but instead return a `MappedRow<T>` instance that is hydrated using the data returned from the `select` clause represented by the `IQuery` instance.

## ContainsAsync

``` c#
Task<bool> ContainsAsync(IQuery query, CancellationToken cancellationToken = default);
```

Returns `true` is the `IQuery` instance `select` clause returns an `int` scalar that equals `1`; else returns `false`.

## Query

The `Query` enables you to create any query using the native language structure:

``` c#
var query = new Query("select UserName from dbo.Member where Id = @Id")
	.AddParameter(new Column<Guid>("Id", DbType.Guid), new Guid('{75208260-CF93-454E-95EC-FE1903F3664E}'));
```

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
            var result = new OrderProcess(OrderProcessColumns.Id.Value(row))
            {
                CustomerName = OrderProcessColumns.CustomerName.Value(row),
                CustomerEMail = OrderProcessColumns.CustomerEMail.Value(row),
                OrderId = OrderProcessColumns.OrderId.Value(row),
                InvoiceId = OrderProcessColumns.InvoiceId.Value(row),
                DateRegistered = OrderProcessColumns.DateRegistered.Value(row),
                OrderNumber = OrderProcessColumns.OrderNumber.Value(row)
            };

            return new MappedRow<OrderProcess>(row, result);
        }
    }
}
```

# MappedRow

A `MappedRow` instance contains both a `DataRow` and the object that the `DataRow` mapped to.  

This may be useful in situations where the `DataRow` contains more information than is available on the object.  An example may be an `OrderLine` where the `DataRow` contains the `OrderId` column but the `OrderLine` object does not.  In order to still be able to make that association it is useful to have both available.

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
			var order = orderRow;

			foreach (var orderLineRow in data.MappedRows<OrderLine>())
			{
				if (orderLineRow.Row["OrderId"].Equals(order.OrderId))
				{
					order.AddLine(orderLineRow);
				}
			}

			result.Add(order);
		}

		return result;
	}
}
```
