using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient.Server;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class ColumnFixture : Fixture
    {
        [Test]
        public void Should_be_able_to_create_mapped_columns()
        {
            const string columnName = "name";

            var mcGuid = new Column<Guid>(columnName, DbType.Guid);

            Assert.AreEqual(columnName, mcGuid.Name);
            Assert.AreEqual(DbType.Guid, mcGuid.DbType);
            Assert.IsNull(mcGuid.Size);
            Assert.IsNull(mcGuid.Precision);
            Assert.IsNull(mcGuid.Scale);

            var mcString = new Column<string>(columnName, DbType.AnsiString, 65);

            Assert.AreEqual(columnName, mcString.Name);
            Assert.AreEqual(DbType.AnsiString, mcString.DbType);
            Assert.AreEqual(65, mcString.Size);
            Assert.IsNull(mcString.Precision);
            Assert.IsNull(mcString.Scale);

            var mcDouble = new Column<decimal>(columnName, DbType.Decimal, 10, 2);

            Assert.AreEqual(columnName, mcDouble.Name);
            Assert.AreEqual(DbType.Decimal, mcDouble.DbType);
            Assert.IsNull(mcDouble.Size);
            Assert.AreEqual((byte)10, mcDouble.Precision);
            Assert.AreEqual((byte)2, mcDouble.Scale);
        }

        [Test]
        public void Should_be_able_to_create_data_parameters()
        {
            const string columnName = "name";

            var parameter = new Mock<IDbDataParameter>();

            var mcGuid = new Column<Guid>(columnName, DbType.Guid);

            var guid = Guid.NewGuid();

            var factory = new Mock<IDbCommand>();

            factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

            var result = mcGuid.CreateDataParameter(factory.Object, guid);

            factory.VerifyAll();
            Assert.AreSame(result, parameter.Object);

            var mcString = new Column<string>(columnName, DbType.AnsiString, 65);

            factory = new Mock<IDbCommand>();
            factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

            result = mcString.CreateDataParameter(factory.Object, "a-string");

            factory.VerifyAll();
            Assert.AreSame(result, parameter.Object);

            var mcDouble = new Column<decimal>(columnName, DbType.Decimal, 10, 2);

            factory = new Mock<IDbCommand>();
            factory.Setup(m => m.CreateParameter()).Returns(parameter.Object);

            result = mcDouble.CreateDataParameter(factory.Object, 150.15d);

            factory.VerifyAll();
            Assert.AreSame(result, parameter.Object);
        }

        [Test]
        public void Should_be_able_to_determine_if_column_value_is_null()
        {
            var table = new DataTable();

            table.Columns.Add("column-1", typeof(string));

            var row = table.Rows.Add(DBNull.Value);

            var mc = new Column<string>("column-1", DbType.AnsiString, 65);

            Assert.IsTrue(mc.IsNull(row));

            row["column-1"] = "value-1";

            Assert.IsFalse(mc.IsNull(row));
        }

        [Test]
        public void Should_be_able_to_map_from_a_dynamic_instance()
        {
            var instanceA = new { column1 = (string)null };

            var mc = new Column<string>("column1", DbType.AnsiString, 65);
            var missing = new Column<string>("missing", DbType.AnsiString, 65);

            Assert.AreEqual(default(string), mc.Value(instanceA));
            Assert.AreEqual(default(string), missing.Value(instanceA));
            Assert.AreEqual(null, mc.RawValue(instanceA));

            var instanceB = new { column1 = "value-1" };

            Assert.AreEqual("value-1", mc.Value(instanceB));
            Assert.AreEqual(default(string), missing.Value(instanceB));
            Assert.AreEqual("value-1", mc.RawValue(instanceB));
        }

        [Test]
        public void Should_be_able_to_map_from_a_data_row()
        {
            var table = new DataTable();

            table.Columns.Add("column-1", typeof(string));

            var row = table.Rows.Add(DBNull.Value);

            var mc = new Column<string>("column-1", DbType.AnsiString, 65);
            var missing = new Column<string>("missing", DbType.AnsiString, 65);

            Assert.AreEqual(default(string), mc.Value(row));
            Assert.AreEqual(default(string), missing.Value(row));
            Assert.AreEqual(DBNull.Value, mc.RawValue(row));

            row["column-1"] = "value-1";

            Assert.AreEqual("value-1", mc.Value(row));
            Assert.AreEqual(default(string), missing.Value(row));
            Assert.AreEqual("value-1", mc.RawValue(row));
        }

        [Test]
        public void Should_be_able_to_map_from_a_data_record()
        {
            var record = new SqlDataRecord(new SqlMetaData("column-1", SqlDbType.VarChar, 65));

            record.SetSqlString(0, new SqlString(null));

            var column1 = new Column<string>("column-1", DbType.AnsiString, 65);
            var column2 = new Column<string>("column-2", DbType.AnsiString, 65);

            Assert.AreEqual(default(string), column1.Value(record));
            Assert.AreEqual(default(string), column2.Value(record));

            record.SetSqlString(0, new SqlString("value-1"));

            Assert.AreEqual("value-1", column1.Value(record));
            Assert.AreEqual(default(string), column2.Value(record));
        }

        [Test]
        public void Should_be_able_to_rename_a_mapped_column()
        {
            var column1Guid = new Column<string>("column-1", DbType.Guid);
            var column2Guid = column1Guid.Rename("column-2");

            Assert.AreEqual("column-1", column1Guid.Name);
            Assert.AreEqual(DbType.Guid, column1Guid.DbType);
            Assert.AreEqual("column-2", column2Guid.Name);
            Assert.AreEqual(DbType.Guid, column2Guid.DbType);

            var column1String = new Column<string>("column-1", DbType.AnsiString, 65);
            var column2String = column1String.Rename("column-2");

            Assert.AreEqual("column-1", column1String.Name);
            Assert.AreEqual(DbType.AnsiString, column1String.DbType);
            Assert.AreEqual(65, column1String.Size);
            Assert.AreEqual("column-2", column2String.Name);
            Assert.AreEqual(DbType.AnsiString, column2String.DbType);
            Assert.AreEqual(65, column2String.Size);

            var column1Double = new Column<string>("column-1", DbType.Double, 10, 2);
            var column2Double = column1Double.Rename("column-2");

            Assert.AreEqual("column-1", column1Double.Name);
            Assert.AreEqual(DbType.Double, column1Double.DbType);
            Assert.AreEqual((byte)10, column1Double.Precision);
            Assert.AreEqual((byte)2, column1Double.Scale);
            Assert.AreEqual("column-2", column2Double.Name);
            Assert.AreEqual(DbType.Double, column2Double.DbType);
            Assert.AreEqual((byte)10, column2Double.Precision);
            Assert.AreEqual((byte)2, column2Double.Scale);
        }

        [Test]
        public void Should_be_able_to_implicitly_convert_to_string()
        {
            var mc = new Column<string>("column-1", DbType.AnsiString, 65);

            string name = mc;

            Assert.AreEqual("column-1", name);
        }
    }
}