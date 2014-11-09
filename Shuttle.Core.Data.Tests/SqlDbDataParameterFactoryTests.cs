using System;
using System.Data;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class SqlDbDataParameterFactoryTests
	{
		[Test]
		public void Should_be_able_to_create_a_data_parameter_with_only_value()
		{
			var guid = Guid.NewGuid();
			var factory = new SqlDbDataParameterFactory();

			var parameter1 = factory.Create("column", DbType.Guid, guid);
			var parameter2 = factory.Create("column", DbType.Guid, null);

			Assert.AreEqual(guid, parameter1.Value);
			Assert.AreEqual(DBNull.Value, parameter2.Value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_be_able_to_create_a_data_parameter_with_only_value_and_unknown_dbtype()
		{
			var factory = new SqlDbDataParameterFactory();

			try
			{
				factory.Create("column", DbType.VarNumeric, null);
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.Message.Contains("Cannot convert DbType VarNumeric to an equivalent SqlDbType"));

				throw;
			}
		}

		[Test]
		public void Should_be_able_to_create_a_data_parameter_with_size()
		{
			const string value = "some-value";

			var factory = new SqlDbDataParameterFactory();

			var parameter1 = factory.Create("column", DbType.AnsiString, 65, value);
			var parameter2 = factory.Create("column", DbType.AnsiString, 65, null);

			Assert.AreEqual(value, parameter1.Value);
			Assert.AreEqual(65, parameter1.Size);
			Assert.AreEqual(DBNull.Value, parameter2.Value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_be_able_to_create_a_data_parameter_with_size_and_unknown_dbtype()
		{
			var factory = new SqlDbDataParameterFactory();

			try
			{
				factory.Create("column", DbType.VarNumeric, 65, "some-value");
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.Message.Contains("Cannot convert DbType VarNumeric to an equivalent SqlDbType"));

				throw;
			}
		}

		[Test]
		public void Should_be_able_to_create_a_data_parameter_with_precision_and_scale()
		{
			var factory = new SqlDbDataParameterFactory();

			var parameter1 = factory.Create("column", DbType.Decimal, 10, 2, 100);
			var parameter2 = factory.Create("column", DbType.Decimal, 10, 2, null);

			Assert.AreEqual(100, parameter1.Value);
			Assert.AreEqual(10, parameter1.Precision);
			Assert.AreEqual(2, parameter1.Scale);
			Assert.AreEqual(DBNull.Value, parameter2.Value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_be_able_to_create_a_data_parameter_with_precision_and_scale_and_unknown_dbtype()
		{
			var factory = new SqlDbDataParameterFactory();

			try
			{
				factory.Create("column", DbType.VarNumeric, 10, 2, 100);
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.Message.Contains("Cannot convert DbType VarNumeric to an equivalent SqlDbType"));

				throw;
			}
		}
	}
}