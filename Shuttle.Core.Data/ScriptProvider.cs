using System;
using System.Collections.Generic;
using System.IO;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class ScriptProvider : IScriptProvider
	{
		private static readonly object Padlock = new object();
		private readonly IScriptProviderConfiguration _configuration;
		private readonly string[] _emptyFiles = new string[0];

		private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();

		public ScriptProvider(IScriptProviderConfiguration configuration)
		{
			Guard.AgainstNull(configuration, "configuration");

			_configuration = configuration;
		}

		public string Get(string scriptName)
		{
			return Get(scriptName, null);
		}

		public string Get(string scriptName, params object[] parameters)
		{
			Guard.AgainstNullOrEmptyString(scriptName, "scriptName");

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
			DatabaseContextInvariant();

			return string.Format("[{0}]-{1}", DatabaseContext.Current.ProviderName, scriptName);
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

				if (!string.IsNullOrEmpty(_configuration.ScriptFolder) && Directory.Exists(_configuration.ScriptFolder))
				{
					files = Directory.GetFiles(_configuration.ScriptFolder, FormattedFileName(scriptName), SearchOption.AllDirectories);
				}

				if (files.Length == 0)
				{
					AddEmbeddedScript(scriptName);

					return;
				}

				if (files.Length > 1)
				{
					throw new InvalidOperationException(string.Format(DataResources.ScriptCountException, _configuration.ScriptFolder,
						scriptName, files.Length));
				}

				_scripts.Add(key, File.ReadAllText(files[0]));
			}
		}

		private string FormattedFileName(string scriptName)
		{
			return FormattedScriptPath(_configuration.FileNameFormat, scriptName);
		}

		private string FormattedResourceName(string scriptName)
		{
			return FormattedScriptPath(_configuration.ResourceNameFormat, scriptName);
		}

		private string FormattedScriptPath(string format, string scriptName)
		{
			DatabaseContextInvariant();

			return format.Replace("{ScriptName}", scriptName).Replace("{ProviderName}", DatabaseContext.Current.ProviderName);
		}

		private static void DatabaseContextInvariant()
		{
			if (DatabaseContext.Current == null)
			{
				throw new InvalidOperationException(DataResources.DatabaseContextMissing);
			}
		}

		private void AddEmbeddedScript(string scriptName)
		{
			if (_configuration.ResourceAssembly == null)
			{
				throw new InvalidOperationException(DataResources.ResourceAssemblyMissingException);
			}

			var path = _configuration.ResourceNameFormat != null ? FormattedResourceName(scriptName) : scriptName;

			using (var stream = _configuration.ResourceAssembly.GetManifestResourceStream(path))
			{
				if (stream == null)
				{
					throw new InvalidOperationException(string.Format(DataResources.EmbeddedScriptMissingException, scriptName, path));
				}

				using (var reader = new StreamReader(stream))
				{
					_scripts.Add(Key(scriptName), reader.ReadToEnd());
				}
			}
		}
	}
}