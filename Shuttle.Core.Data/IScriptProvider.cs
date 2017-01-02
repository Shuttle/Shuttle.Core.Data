namespace Shuttle.Core.Data
{
    public interface IScriptProvider
    {
        string Get(string name);
        string Get(string name, params object[] parameters);
    }
}