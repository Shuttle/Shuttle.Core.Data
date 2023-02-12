using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class ScriptProvider : IScriptProvider
	{
		private static readonly object Lock = new object();
		private readonly ScriptProviderOptions _options;
		private readonly string[] _emptyFiles = Array.Empty<string>();

		private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();

		public ScriptProvider(IOptions<ScriptProviderOptions> options)
		{
			Guard.AgainstNull(options, nameof(options));

			_options = Guard.AgainstNull(options.Value, nameof(options.Value));
        }

		public string Get(string providerName, string scriptName)
		{
			return Get(providerName, scriptName, null);
		}

		public string Get(string providerName, string scriptName, params object[] parameters)
		{
			Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
			Guard.AgainstNullOrEmptyString(scriptName, nameof(scriptName));

			var key = Key(providerName, scriptName);

			lock (Lock)
			{
				if (!_scripts.ContainsKey(key))
				{
					AddScript(providerName, scriptName);
				}

				return parameters != null
					? string.Format(_scripts[key], parameters)
					: _scripts[key];
			}
		}

		private string Key(string providerName, string scriptName)
		{
			lock (Lock)
			{
				return $"[{providerName}]-{scriptName}";
			}
		}

		private void AddScript(string providerName, string scriptName)
		{
			var key = Key(providerName, scriptName);

			lock (Lock)
			{
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
					throw new InvalidOperationException(string.Format(Resources.ScriptCountException, _options.ScriptFolder,
						scriptName, files.Length));
				}

				_scripts.Add(key, File.ReadAllText(files[0]));
			}
		}

		private string FormattedFileName(string providerName, string scriptName)
		{
			return FormattedScriptPath(_options.FileNameFormat, providerName, scriptName);
		}

		private string FormattedResourceName(string providerName, string scriptName)
		{
			return FormattedScriptPath(_options.ResourceNameFormat, providerName, scriptName);
		}

		private string FormattedScriptPath(string format, string providerName, string scriptName)
		{
			return format.Replace("{ScriptName}", scriptName).Replace("{ProviderName}", providerName);
		}

		private void AddEmbeddedScript(string providerName, string scriptName)
		{
			if (_options.ResourceAssembly == null)
			{
				throw new InvalidOperationException(Resources.ResourceAssemblyMissingException);
			}

			var path = _options.ResourceNameFormat != null ? FormattedResourceName(providerName, scriptName) : scriptName;

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
	}
}