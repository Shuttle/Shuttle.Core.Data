using System.Reflection;

namespace Shuttle.Core.Data;

public class ScriptProviderOptions
{
    public string FileNameFormat { get; set; } = string.Empty;
    public Assembly? ResourceAssembly { get; set; }
    public string ResourceNameFormat { get; set; } = string.Empty;
    public string ScriptFolder { get; set; } = string.Empty;
}