namespace Shuttle.Core.Data;

public interface IScriptProvider
{
    string Get(string connectionStringName, string scriptName);
}