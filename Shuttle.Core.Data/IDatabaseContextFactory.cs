using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string connectionStringName);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(IDbConnection dbConnection);

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextCache DatabaseContextCache { get; }
    }
}