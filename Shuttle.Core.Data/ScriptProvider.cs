using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class ScriptProvider : IScriptProvider
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
    private readonly string[] _emptyFiles = Array.Empty<string>();
    private readonly ScriptProviderOptions _options;

    private readonly Dictionary<string, string> _scripts = new();

    public ScriptProvider(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<ScriptProviderOptions> options)
    {
        _connectionStringOptions = Guard.AgainstNull(connectionStringOptions);
        _options = Guard.AgainstNull(Guard.AgainstNull(options).Value);
    }

    public async ValueTask<string> GetAsync(string connectionStringName, string scriptName, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNullOrEmptyString(connectionStringName);
        Guard.AgainstNullOrEmptyString(scriptName);

        await Lock.WaitAsync(cancellationToken);

        try
        {
            var connectionStringOptions = _connectionStringOptions.Get(connectionStringName);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, connectionStringName));
            }

            var key = Key(connectionStringOptions.ProviderName, scriptName);

            if (!_scripts.ContainsKey(key))
            {
                AddEmbedded(connectionStringOptions.ProviderName, scriptName);
            }

            return _scripts[key];
        }
        finally
        {
            Lock.Release();
        }
    }

    private void AddEmbedded(string providerName, string scriptName)
    {
        var key = Key(providerName, scriptName);

        if (_scripts.ContainsKey(key))
        {
            return;
        }

        var files = _emptyFiles;

        if (!string.IsNullOrEmpty(_options.ScriptFolder) && Directory.Exists(_options.ScriptFolder))
        {
            files = Directory.GetFiles(_options.ScriptFolder, FormattedFileName(providerName, scriptName), SearchOption.AllDirectories);
        }

        if (files.Length == 0)
        {
            AddEmbeddedScript(providerName, scriptName);

            return;
        }

        if (files.Length > 1)
        {
            throw new InvalidOperationException(string.Format(Resources.ScriptCountException, _options.ScriptFolder, scriptName, files.Length));
        }

        _scripts.Add(key, File.ReadAllText(files[0]));
    }

    private void AddEmbeddedScript(string providerName, string scriptName)
    {
        if (_options.ResourceAssembly == null)
        {
            throw new InvalidOperationException(Resources.ResourceAssemblyMissingException);
        }

        var path = FormattedResourceName(providerName, scriptName);

        using (var stream = _options.ResourceAssembly.GetManifestResourceStream(path))
        {
            if (stream == null)
            {
                throw new InvalidOperationException(string.Format(Resources.EmbeddedScriptMissingException, scriptName, path));
            }

            using (var reader = new StreamReader(stream))
            {
                _scripts.Add(Key(providerName, scriptName), reader.ReadToEnd());
            }
        }
    }

    private string FormattedFileName(string providerName, string scriptName)
    {
        return FormattedScriptPath(providerName, _options.FileNameFormat, scriptName);
    }

    private string FormattedResourceName(string providerName, string scriptName)
    {
        return FormattedScriptPath(providerName, _options.ResourceNameFormat, scriptName);
    }

    private string FormattedScriptPath(string providerName, string format, string scriptName)
    {
        return format.Replace("{ScriptName}", scriptName).Replace("{ProviderName}", providerName);
    }

    private string Key(string providerName, string scriptName)
    {
        lock (Lock)
        {
            return $"[{providerName}]-{scriptName}";
        }
    }
}