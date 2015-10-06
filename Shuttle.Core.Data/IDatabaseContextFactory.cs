using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string connectionStringName);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(IDbConnection dbConnection);
	}
}