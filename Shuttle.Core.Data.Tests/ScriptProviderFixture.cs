using System;
using System.IO;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    public class ScriptProviderFixture : Fixture
    {
		[SetUp]
	    public void SetupContext()
		{
			var cache = new Mock<IDatabaseContextCache>();

			cache.Setup(m => m.Current).Returns(new Mock<IDatabaseContext>().Object);
		}

	    [Test]
	    public void Should_fail_when_there_is_no_ambient_database_context()
	    {
		    Assert.Throws<InvalidOperationException>(
			    () => new ScriptProvider(Options.Create(new ScriptProviderOptions()), new DatabaseContextCache()).Get("throw"));
	    }

	    [Test]
        public void Should_eb_able_to_retrieve_script_from_file()
	    {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                FileNameFormat = "{ScriptName}.sql",
                ScriptFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\.scripts\")
            }), MockDatabaseContextCache());

            var script = provider.Get("file-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'file-script'", script);
        }

        private static IDatabaseContextCache MockDatabaseContextCache()
        {
	        var databaseContextCache = new Mock<IDatabaseContextCache>();

	        databaseContextCache.Setup(m => m.Current).Returns(new Mock<IDatabaseContext>().Object);

            return databaseContextCache.Object;
        }

        [Test]
        public void Should_eb_able_to_retrieve_script_from_resource()
        {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                ResourceAssembly = GetType().Assembly,
                ResourceNameFormat = "Shuttle.Core.Data.Tests..scripts.System.Data.SqlClient.{ScriptName}.sql"
            }), MockDatabaseContextCache());

            var script = provider.Get("embedded-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'embedded-script'", script);
        }

        [Test]
        public void Should_throw_exception_when_no_resource_or_file_found()
        {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                ResourceAssembly = GetType().Assembly
            }), MockDatabaseContextCache());

            Assert.Throws<InvalidOperationException>(() => provider.Get("System.Data.SqlClient", "missing-script"));
        }
    }
}