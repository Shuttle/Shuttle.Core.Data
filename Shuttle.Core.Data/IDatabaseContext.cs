using System;
using System.Data;
using System.Data.Common;

namespace Shuttle.Core.Data
{
    public interface IDatabaseContext : IDisposable
    {
		Guid Key { get; }
		string Name { get; }

        IDbTransaction Transaction { get; }
        IDbConnection Connection { get; }
        IDbCommand CreateCommand(IQuery query);

        bool HasTransaction { get; }
        string ProviderName { get; }

        IDatabaseContext BeginTransaction();
        IDatabaseContext BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
	    IDatabaseContext WithName(string name);
	    IDatabaseContext Suppressed();
	    IDatabaseContext SuppressDispose();
    }
}