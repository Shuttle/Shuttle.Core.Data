using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDatabaseContext : IDisposable
    {
		Guid Key { get; }
		string Name { get; }

        IDbTransaction Transaction { get; }
        IDbConnection Connection { get; }
        Task<IDbCommand> CreateCommand(IQuery query);

        bool HasTransaction { get; }
        string ProviderName { get; }

        Task<IDatabaseContext> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task CommitTransaction();
	    IDatabaseContext WithName(string name);
	    IDatabaseContext Suppressed();
	    IDatabaseContext SuppressDispose();
    }
}