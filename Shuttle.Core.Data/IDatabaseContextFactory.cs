using System.Data.Common;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		Task<IDatabaseContext> Create(string name);
		Task<IDatabaseContext> Create(string providerName, string connectionString);
		Task<IDatabaseContext> Create(string providerName, DbConnection dbConnection);
        Task<IDatabaseContext> Create();

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
    }
}