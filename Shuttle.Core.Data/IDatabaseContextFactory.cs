using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string name);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(string providerName, IDbConnection dbConnection);
        IDatabaseContext Create();

        void ConfigureWith(string connectionStringName);
        void ConfigureWith(string providerName, string connectionString);
        void ConfigureWith(IDbConnection dbConnection);

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextCache DatabaseContextCache { get; }
    }
}