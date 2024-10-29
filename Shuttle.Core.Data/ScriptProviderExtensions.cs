using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class ScriptProviderExtensions
{
    public static async ValueTask<string> GetAsync(this IScriptProvider scriptProvider, string connectionStringName, string scriptName, params object[] parameters)
    {
        return string.Format(await Guard.AgainstNull(scriptProvider).GetAsync(connectionStringName, scriptName), parameters);
    }
}