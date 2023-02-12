namespace Shuttle.Core.Data
{
    public interface IScriptProvider
    {
        string Get(string providerName, string scriptName);
        string Get(string providerName, string scriptName, params object[] parameters);
    }
}