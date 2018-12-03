namespace Shuttle.Core.Data
{
	public interface IDatabaseContextCache
	{
		IDatabaseContext Current { get; }
		ActiveDatabaseContext Use(string name);
		ActiveDatabaseContext Use(IDatabaseContext context);
		bool Contains(string connectionString);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
		IDatabaseContext Get(string connectionString);
	}
}