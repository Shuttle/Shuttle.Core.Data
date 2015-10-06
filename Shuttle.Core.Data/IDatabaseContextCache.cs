namespace Shuttle.Core.Data
{
	public interface IDatabaseContextCache
	{
		IDatabaseContext Current { get; }
		void Use(string name);
		void Use(IDatabaseContext context);
		bool Contains(string connectionString);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
		IDatabaseContext Get(string connectionString);
	}
}