namespace Shuttle.Core.Data
{
    public interface IDbConnectionConfiguration
    {
        string Name { get; }
        string ProviderName { get; }
        string ConnectionString { get; }
    }
}