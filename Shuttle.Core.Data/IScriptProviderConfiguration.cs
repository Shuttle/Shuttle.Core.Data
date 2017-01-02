using System.Reflection;

namespace Shuttle.Core.Data
{
    public interface IScriptProviderConfiguration
    {
        Assembly ResourceAssembly { get; }
        string ResourceNameFormat { get; }
        string FileNameFormat { get; }
        string ScriptFolder { get; }
    }
}