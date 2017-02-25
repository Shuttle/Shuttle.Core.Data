namespace Shuttle.Core.Data
{
    public interface IScriptProvider
    {
        string Get(string scriptName);
        string Get(string scriptName, params object[] parameters);
    }
}