namespace Shuttle.Core.Data
{
    public interface IDbConnectionConfigurationProvider
    {
        IDbConnectionConfiguration Get(DataSource source);
    }
}