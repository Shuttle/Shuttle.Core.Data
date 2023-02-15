using System.Data.Common;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		IDatabaseContext Create(string name);
		IDatabaseContext Create(string providerName, string connectionString);
		IDatabaseContext Create(string providerName, DbConnection dbConnection);
        IDatabaseContext Create();

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextService DatabaseContextService { get; }
    }
}