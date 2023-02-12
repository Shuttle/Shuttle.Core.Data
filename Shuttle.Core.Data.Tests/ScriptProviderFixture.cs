using System;
using System.IO;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    public class ScriptProviderFixture : Fixture
    {
	    [Test]
	    public void Should_fail_when_there_is_no_ambient_database_context()
	    {
		    Assert.Throws<InvalidOperationException>(
			    () => new ScriptProvider(Options.Create(new ScriptProviderOptions())).Get("Microsoft.Data.SqlClient", "throw"));
	    }

	    [Test]
        public void Should_be_able_to_retrieve_script_from_file()
	    {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                FileNameFormat = "{ScriptName}.sql",
                ScriptFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\.scripts\")
            }));

            var script = provider.Get("Microsoft.Data.SqlClient", "file-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'file-script'", script);
        }

        [Test]
        public void Should_be_able_to_retrieve_script_from_resource()
        {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                ResourceAssembly = GetType().Assembly,
                ResourceNameFormat = "Shuttle.Core.Data.Tests..scripts.Microsoft.Data.SqlClient.{ScriptName}.sql"
            }));

            var script = provider.Get("Microsoft.Data.SqlClient", "embedded-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'embedded-script'", script);
        }

        [Test]
        public void Should_throw_exception_when_no_resource_or_file_found()
        {
            var provider = new ScriptProvider(Options.Create(new ScriptProviderOptions
            {
                ResourceAssembly = GetType().Assembly
            }));

            Assert.Throws<InvalidOperationException>(() => provider.Get("Microsoft.Data.SqlClient", "missing-script"));
        }
    }
}