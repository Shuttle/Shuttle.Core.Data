using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string connectionStringName);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(IDbConnection dbConnection);
        IDatabaseContext Create();

        void ConfigureWith(string connectionStringName);
        void ConfigureWith(string providerName, string connectionString);
        void ConfigureWith(IDbConnection dbConnection);

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextCache DatabaseContextCache { get; }
    }
}