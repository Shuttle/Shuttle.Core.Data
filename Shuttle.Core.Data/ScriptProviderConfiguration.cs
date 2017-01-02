using System.Reflection;

namespace Shuttle.Core.Data
{
    public class ScriptProviderConfiguration : IScriptProviderConfiguration
    {
        public Assembly ResourceAssembly { get; set; }
        public string ResourceNameFormat { get; set; }
        public string FileNameFormat { get; set; }
        public string ScriptFolder { get; set; }
    }
}