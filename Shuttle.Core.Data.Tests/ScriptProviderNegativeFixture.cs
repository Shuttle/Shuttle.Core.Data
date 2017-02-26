using System;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	public class ScriptProviderNegativeFixture : Fixture
	{
		[Test]
		public void Should_fail_when_there_is_no_ambient_database_context()
		{
			DatabaseContext.Assign(new Mock<IDatabaseContextCache>().Object);

			Assert.Throws<InvalidOperationException>(
				() => new ScriptProvider(new Mock<IScriptProviderConfiguration>().Object).Get("throw"));
		}
	}
}