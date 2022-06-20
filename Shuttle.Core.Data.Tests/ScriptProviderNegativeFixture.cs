using System;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	public class ScriptProviderNegativeFixture : Fixture
	{
		[Test]
		public void Should_fail_when_there_is_no_ambient_database_context()
		{
			Assert.Throws<InvalidOperationException>(
				() => new ScriptProvider(Options.Create(new ScriptProviderSettings()), new DatabaseContextCache()).Get("throw"));
		}
	}
}