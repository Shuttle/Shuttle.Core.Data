using System;
using System.Data;
using Microsoft.Data.SqlClient.Server;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class ColumnFixture : Fixture
{
    [Test]
    public void Should_be_able_to_create_mapped_columns()
    {
        const string columnName = "name";

        Column<Guid> mcGuid = new(columnName, DbType.Guid);

        Assert.That(mcGuid.Name, Is.EqualTo(columnName));
        Assert.That(mcGuid.DbType, Is.EqualTo(DbType.Guid));
        Assert.That(mcGuid.Size, Is.Null);
        Assert.That(mcGuid.Precision, Is.Null);
        Assert.That(mcGuid.Scale, Is.Null);

        Column<string> mcString = new(columnName, DbType.AnsiString, 65);

        Assert.That(mcString.Name, Is.EqualTo(columnName));
        Assert.That(mcString.DbType, Is.EqualTo(DbType.AnsiString));
        Assert.That(mcString.Size, Is.EqualTo(65));
        Assert.That(mcString.Precision, Is.Null);
        Assert.That(mcString.Scale, Is.Null);

        Column<decimal> mcDouble = new(columnName, DbType.Decimal, 10, 2);

        Assert.That(mcDouble.Name, Is.EqualTo(columnName));
        Assert.That(mcDouble.DbType, Is.EqualTo(DbType.Decimal));
        Assert.That(mcDouble.Size, Is.Null);
        Assert.That(mcDouble.Precision, Is.EqualTo((byte)10));
        Assert.That(mcDouble.Scale, Is.EqualTo((byte)2));
    }

    [Test]
    public void Should_be_able_to_create_data_parameters()
    {
        const string columnName = "name";

        Mock<IDbDataParameter> parameter = new();

        Column<Guid> mcGuid = new(columnName, DbType.Guid);

        var guid = Guid.NewGuid();

        Mock<IDbCommand> factory = new();

        factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

        var result = mcGuid.CreateDataParameter(factory.Object, guid);

        factory.VerifyAll();
        Assert.That(parameter.Object, Is.SameAs(result));

        Column<string> mcString = new(columnName, DbType.AnsiString, 65);

        factory = new();
        factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

        result = mcString.CreateDataParameter(factory.Object, "a-string");

        factory.VerifyAll();
        Assert.That(parameter.Object, Is.SameAs(result));

        Column<decimal> mcDouble = new(columnName, DbType.Decimal, 10, 2);

        factory = new();
        factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

        result = mcDouble.CreateDataParameter(factory.Object, 150.15d);

        factory.VerifyAll();
        Assert.That(parameter.Object, Is.SameAs(result));
    }

    [Test]
    public void Should_be_able_to_determine_if_column_value_is_null()
    {
        DataTable table = new();

        table.Columns.Add("column-1", typeof(string));

        var row = table.Rows.Add(DBNull.Value);

        Column<string> mc = new("column-1", DbType.AnsiString, 65);

        Assert.That(mc.IsNull(row), Is.True);

        row["column-1"] = "value-1";

        Assert.That(mc.IsNull(row), Is.False);
    }

    [Test]
    public void Should_be_able_to_map_from_a_dynamic_instance()
    {
        var instanceA = new { column1 = (string)null };

        Column<string> mc = new("column1", DbType.AnsiString, 65);
        Column<string> missing = new("missing", DbType.AnsiString, 65);

        Assert.That(mc.Value(instanceA), Is.EqualTo(default(string)));
        Assert.That(missing.Value(instanceA), Is.EqualTo(default(string)));
        Assert.That(mc.RawValue(instanceA), Is.Null);

        var instanceB = new { column1 = "value-1" };

        Assert.That(mc.Value(instanceB), Is.EqualTo("value-1"));
        Assert.That(missing.Value(instanceB), Is.EqualTo(default(string)));
        Assert.That(mc.RawValue(instanceB), Is.EqualTo("value-1"));
    }

    [Test]
    public void Should_be_able_to_map_from_a_data_row()
    {
        DataTable table = new();

        table.Columns.Add("column-1", typeof(string));

        var row = table.Rows.Add(DBNull.Value);

        Column<string> mc = new("column-1", DbType.AnsiString, 65);
        Column<string> missing = new("missing", DbType.AnsiString, 65);

        Assert.That(mc.Value(row), Is.EqualTo(default(string)));
        Assert.That(missing.Value(row), Is.EqualTo(default(string)));
        Assert.That(mc.RawValue(row), Is.EqualTo(DBNull.Value));

        row["column-1"] = "value-1";

        Assert.That(mc.Value(row), Is.EqualTo("value-1"));
        Assert.That(missing.Value(row), Is.EqualTo(default(string)));
        Assert.That(mc.RawValue(row), Is.EqualTo("value-1"));
    }

    [Test]
    public void Should_be_able_to_map_from_a_data_record()
    {
        SqlDataRecord record = new(new SqlMetaData("column-1", SqlDbType.VarChar, 65));

        record.SetSqlString(0, new(null));

        Column<string> column1 = new("column-1", DbType.AnsiString, 65);
        Column<string> column2 = new("column-2", DbType.AnsiString, 65);

        Assert.That(column1.Value(record), Is.EqualTo(default(string)));
        Assert.That(column2.Value(record), Is.EqualTo(default(string)));

        record.SetSqlString(0, new("value-1"));

        Assert.That(column1.Value(record), Is.EqualTo("value-1"));
        Assert.That(column2.Value(record), Is.EqualTo(default(string)));
    }

    [Test]
    public void Should_be_able_to_rename_a_mapped_column()
    {
        Column<string> column1Guid = new("column-1", DbType.Guid);
        var column2Guid = column1Guid.Rename("column-2");

        Assert.That(column1Guid.Name, Is.EqualTo("column-1"));
        Assert.That(column1Guid.DbType, Is.EqualTo(DbType.Guid));
        Assert.That(column2Guid.Name, Is.EqualTo("column-2"));
        Assert.That(column2Guid.DbType, Is.EqualTo(DbType.Guid));

        Column<string> column1String = new("column-1", DbType.AnsiString, 65);
        var column2String = column1String.Rename("column-2");

        Assert.That(column1String.Name, Is.EqualTo("column-1"));
        Assert.That(column1String.DbType, Is.EqualTo(DbType.AnsiString));
        Assert.That(column1String.Size, Is.EqualTo(65));
        Assert.That(column2String.Name, Is.EqualTo("column-2"));
        Assert.That(column2String.DbType, Is.EqualTo(DbType.AnsiString));
        Assert.That(column2String.Size, Is.EqualTo(65));

        Column<string> column1Double = new("column-1", DbType.Double, 10, 2);
        var column2Double = column1Double.Rename("column-2");

        Assert.That(column1Double.Name, Is.EqualTo("column-1"));
        Assert.That(column1Double.DbType, Is.EqualTo(DbType.Double));
        Assert.That(column1Double.Precision, Is.EqualTo((byte)10));
        Assert.That(column1Double.Scale, Is.EqualTo((byte)2));
        Assert.That(column2Double.Name, Is.EqualTo("column-2"));
        Assert.That(column2Double.DbType, Is.EqualTo(DbType.Double));
        Assert.That(column2Double.Precision, Is.EqualTo((byte)10));
        Assert.That(column2Double.Scale, Is.EqualTo((byte)2));
    }

    [Test]
    public void Should_be_able_to_implicitly_convert_to_string()
    {
        Column<string> mc = new("column-1", DbType.AnsiString, 65);

        string name = mc;

        Assert.That(name, Is.EqualTo("column-1"));
    }
}