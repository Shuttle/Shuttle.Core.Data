using System.Reflection;

namespace Shuttle.Core.Data;

public class ScriptProviderOptions
{
    public string FileNameFormat { get; set; } = default!;
    public Assembly ResourceAssembly { get; set; } = default!;
    public string ResourceNameFormat { get; set; } = default!;
    public string ScriptFolder { get; set; } = default!;
}