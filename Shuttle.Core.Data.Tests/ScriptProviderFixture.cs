using System;
using System.IO;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    public class ScriptProviderFixture : Fixture
    {
        [Test]
        public void Should_eb_able_to_retrieve_script_from_file()
        {
            var provider = new ScriptProvider(new ScriptProviderConfiguration
            {
                FileNameFormat = "{0}.sql",
                ScriptFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\.scripts\")
            });

            var script = provider.Get("file-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'file-script'", script);
        }

        [Test]
        public void Should_eb_able_to_retrieve_script_from_resource()
        {
            var provider = new ScriptProvider(new ScriptProviderConfiguration
            {
                ResourceAssembly = GetType().Assembly,
                ResourceNameFormat = "Shuttle.Core.Data.Tests..scripts.System.Data.SqlClient.{0}.sql"
            });

            var script = provider.Get("embedded-script");

            Assert.IsFalse(string.IsNullOrEmpty(script));
            Assert.AreEqual("select 'embedded-script'", script);
        }

        [Test]
        public void Should_throw_exception_when_no_resource_or_file_found()
        {
            var provider = new ScriptProvider(new ScriptProviderConfiguration
            {
                ResourceAssembly = GetType().Assembly
            });

            Assert.Throws<InvalidOperationException>(() => provider.Get("missing-script"));
        }
    }
}