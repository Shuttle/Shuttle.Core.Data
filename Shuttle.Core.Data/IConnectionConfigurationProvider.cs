namespace Shuttle.Core.Data
{
    public interface IConnectionConfigurationProvider
    {
        ConnectionConfiguration Get(string name);
    }
}