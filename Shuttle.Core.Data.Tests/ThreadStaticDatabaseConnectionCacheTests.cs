using System;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class ThreadStaticDatabaseConnectionCacheTests : Fixture
	{
		[Test]
		public void Should_be_able_perform_basic_operations()
		{
			var dataSource = DefaultDataSource();
			var connection = new Mock<IDatabaseConnection>();
			var cache = new ThreadStaticDatabaseConnectionCache();

			Assert.IsFalse(cache.Contains(dataSource));

			cache.Add(dataSource, connection.Object);

			Assert.IsTrue(cache.Contains(dataSource));
			Assert.AreSame(connection.Object, cache.Get(dataSource));

			cache.Remove(dataSource);

			Assert.IsFalse(cache.Contains(dataSource));
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void Should_not_be_able_to_get_a_connection_that_has_not_been_added()
		{
			var cache = new ThreadStaticDatabaseConnectionCache();

			try
			{
				cache.Get(DefaultDataSource());
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.Message.Contains("Attempt to retrieve non-existent connection name"));

				throw;
			}
		}
	}
}