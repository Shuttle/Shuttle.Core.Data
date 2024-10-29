using System;
using System.IO;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class ScriptProviderFixture : Fixture
{
    [Test]
    public void Should_be_able_to_retrieve_script_from_file()
    {
        Mock<IOptionsMonitor<ConnectionStringOptions>> connectionStringOptions = new();

        connectionStringOptions.Setup(m => m.Get("shuttle")).Returns(new ConnectionStringOptions { Name = "shuttle" });

        ScriptProvider provider = new(connectionStringOptions.Object, Options.Create(new ScriptProviderOptions { FileNameFormat = "{ScriptName}.sql", ScriptFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\.scripts\") }));

        var script = provider.Get("shuttle", "file-script");

        Assert.That(string.IsNullOrEmpty(script), Is.False);
        Assert.That(script, Is.EqualTo("select 'file-script'"));
    }

    [Test]
    public void Should_eb_able_to_retrieve_script_from_resource()
    {
        Mock<IOptionsMonitor<ConnectionStringOptions>> connectionStringOptions = new();

        connectionStringOptions.Setup(m => m.Get("shuttle")).Returns(new ConnectionStringOptions { Name = "shuttle" });

        ScriptProvider provider = new(connectionStringOptions.Object, Options.Create(new ScriptProviderOptions { ResourceAssembly = GetType().Assembly, ResourceNameFormat = "Shuttle.Core.Data.Tests..scripts.Microsoft.Data.SqlClient.{ScriptName}.sql" }));

        var script = provider.Get("shuttle", "embedded-script");

        Assert.That(string.IsNullOrEmpty(script), Is.False);
        Assert.That(script, Is.EqualTo("select 'embedded-script'"));
    }

    [Test]
    public void Should_fail_when_there_connections_string_is_not_found()
    {
        Mock<IOptionsMonitor<ConnectionStringOptions>> connectionStringOptions = new();

        Assert.Throws<InvalidOperationException>(
            () => new ScriptProvider(connectionStringOptions.Object, Options.Create(new ScriptProviderOptions())).Get("missing", "throw"));
    }

    [Test]
    public void Should_throw_exception_when_no_resource_or_file_found()
    {
        Mock<IOptionsMonitor<ConnectionStringOptions>> connectionStringOptions = new();

        connectionStringOptions.Setup(m => m.Get("shuttle")).Returns(new ConnectionStringOptions { Name = "shuttle" });

        ScriptProvider provider = new(connectionStringOptions.Object, Options.Create(new ScriptProviderOptions { ResourceAssembly = GetType().Assembly }));

        Assert.Throws<InvalidOperationException>(() => provider.Get("Microsoft.Data.SqlClient", "missing-script"));
    }
}