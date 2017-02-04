using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class ScriptProvider : IScriptProvider
    {
        private readonly IScriptProviderConfiguration _configuration;
        private static readonly object Padlock = new object();

        private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();
        private readonly string[] _emptyFiles = new string[0];

        public ScriptProvider(IScriptProviderConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            _configuration = configuration;
        }

        public string Get(string name)
        {
            return Get(name, null);
        }

        public string Get(string name, params object[] parameters)
        {
            lock (Padlock)
            {
                if (!_scripts.ContainsKey(name))
                {
                    AddScript(name);
                }

                return parameters != null
                    ? string.Format(_scripts[name], parameters)
                    : _scripts[name];
            }
        }

        private void AddScript(string name)
        {
            lock (Padlock)
            {
                if (_scripts.ContainsKey(name))
                {
                    return;
                }

                var files = _emptyFiles;

                if (!string.IsNullOrEmpty(_configuration.ScriptFolder) && Directory.Exists(_configuration.ScriptFolder))
                {
                    files = Directory.GetFiles(_configuration.ScriptFolder, string.Format(_configuration.FileNameFormat, name), SearchOption.AllDirectories);
                }

                if (files.Length == 0)
                {
                    AddEmbeddedScript(name);

                    return;
                }

                if (files.Length > 1)
                {
                    throw new InvalidOperationException(string.Format(DataResources.ScriptCountException, _configuration.ScriptFolder, name, files.Length));
                }

                _scripts.Add(name, File.ReadAllText(files[0]));
            }
        }

        private void AddEmbeddedScript(string name)
        {
            if (_configuration.ResourceAssembly == null)
            {
                throw new InvalidOperationException(DataResources.ResourceAssemblyMissingException);
            }

            var path = _configuration.ResourceNameFormat != null ? string.Format(_configuration.ResourceNameFormat, name) : name;

            using (var stream = _configuration.ResourceAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(string.Format(DataResources.EmbeddedScriptMissingException, name, path));
                }

                using (var reader = new StreamReader(stream))
                {
                    _scripts.Add(name, reader.ReadToEnd());
                }
            }
        }
    }
}