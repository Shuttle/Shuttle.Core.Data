namespace Shuttle.Core.Data
{
    public interface IDatabaseConnectionCache
    {
        IDatabaseConnection Get(DataSource source);
        IDatabaseConnection Add(DataSource source, IDatabaseConnection connection);
        void Remove(DataSource source);
        bool Contains(DataSource source);
    }
}