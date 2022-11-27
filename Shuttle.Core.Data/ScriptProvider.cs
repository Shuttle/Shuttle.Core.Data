using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class ScriptProvider : IScriptProvider
	{
		private static readonly object Padlock = new object();
		private readonly ScriptProviderOptions _options;
		private readonly IDatabaseContextCache _databaseContextCache;
		private readonly string[] _emptyFiles = Array.Empty<string>();

		private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();

		public ScriptProvider(IOptions<ScriptProviderOptions> options, IDatabaseContextCache databaseContextCache)
		{
			Guard.AgainstNull(options, nameof(options));

			_options = Guard.AgainstNull(options.Value, nameof(options.Value));
            _databaseContextCache = Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
        }

		public string Get(string scriptName)
		{
			return Get(scriptName, null);
		}

		public string Get(string scriptName, params object[] parameters)
		{
			Guard.AgainstNullOrEmptyString(scriptName, nameof(scriptName));

			var key = Key(scriptName);

			lock (Padlock)
			{
				if (!_scripts.ContainsKey(key))
				{
					AddScript(scriptName);
				}

				return parameters != null
					? string.Format(_scripts[key], parameters)
					: _scripts[key];
			}
		}

		private string Key(string scriptName)
		{
			lock (Padlock)
			{
				return $"[{_databaseContextCache.Current.ProviderName}]-{scriptName}";
			}
		}

		private void AddScript(string scriptName)
		{
			var key = Key(scriptName);

			lock (Padlock)
			{
				if (_scripts.ContainsKey(key))
				{
					return;
				}

				var files = _emptyFiles;

				if (!string.IsNullOrEmpty(_options.ScriptFolder) && Directory.Exists(_options.ScriptFolder))
				{
					files = Directory.GetFiles(_options.ScriptFolder, FormattedFileName(scriptName), SearchOption.AllDirectories);
				}

				if (files.Length == 0)
				{
					AddEmbeddedScript(scriptName);

					return;
				}

				if (files.Length > 1)
				{
					throw new InvalidOperationException(string.Format(Resources.ScriptCountException, _options.ScriptFolder,
						scriptName, files.Length));
				}

				_scripts.Add(key, File.ReadAllText(files[0]));
			}
		}

		private string FormattedFileName(string scriptName)
		{
			return FormattedScriptPath(_options.FileNameFormat, scriptName);
		}

		private string FormattedResourceName(string scriptName)
		{
			return FormattedScriptPath(_options.ResourceNameFormat, scriptName);
		}

		private string FormattedScriptPath(string format, string scriptName)
		{
			return format.Replace("{ScriptName}", scriptName).Replace("{ProviderName}", _databaseContextCache.Current.ProviderName);
		}

		private void AddEmbeddedScript(string scriptName)
		{
			if (_options.ResourceAssembly == null)
			{
				throw new InvalidOperationException(Resources.ResourceAssemblyMissingException);
			}

			var path = _options.ResourceNameFormat != null ? FormattedResourceName(scriptName) : scriptName;

			using (var stream = _options.ResourceAssembly.GetManifestResourceStream(path))
			{
				if (stream == null)
				{
					throw new InvalidOperationException(string.Format(Resources.EmbeddedScriptMissingException, scriptName, path));
				}

				using (var reader = new StreamReader(stream))
				{
					_scripts.Add(Key(scriptName), reader.ReadToEnd());
				}
			}
		}
	}
}