namespace Shuttle.Core.Data
{
	public interface IDatabaseContextCache
	{
		IDatabaseContext Current { get; }
		ActiveDatabaseContext Use(string name);
		ActiveDatabaseContext Use(IDatabaseContext context);
        bool Contains(string name);
        bool ContainsConnectionString(string connectionString);
        IDatabaseContext Get(string name);
        IDatabaseContext GetConnectionString(string connectionString);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
	}
}