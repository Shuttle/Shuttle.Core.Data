using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string name);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(string providerName, IDbConnection dbConnection);
        IDatabaseContext Create();

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextCache DatabaseContextCache { get; }
    }
}