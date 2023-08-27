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
        IDbCommand CreateCommand(IQuery query);
        Task<IDbCommand> CreateCommandAsync(IQuery query);

        bool HasTransaction { get; }
        string ProviderName { get; }

        IDatabaseContext BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        void CommitTransaction();
        Task CommitTransactionAsync();
	    IDatabaseContext WithName(string name);
	    IDatabaseContext Suppressed();
	    IDatabaseContext SuppressDispose();
    }
}