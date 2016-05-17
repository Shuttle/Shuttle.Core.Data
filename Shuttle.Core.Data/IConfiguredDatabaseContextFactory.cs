using System.Data;

namespace Shuttle.Core.Data
{
	public interface IConfiguredDatabaseContextFactory : IDatabaseContextFactory
	{
		IDatabaseContext Create();

		void ConfigureWith(string connectionStringName);
		void ConfigureWith(string providerName, string connectionString);
		void ConfigureWith(IDbConnection dbConnection);
	}
}