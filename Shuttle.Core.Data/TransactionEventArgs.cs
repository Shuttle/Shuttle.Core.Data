using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class TransactionEventArgs : EventArgs
    {
        public TransactionEventArgs(IDbTransaction transaction)
        {
            Transaction = Guard.AgainstNull(transaction, nameof(transaction));
        }

        public IDbTransaction Transaction { get; }
    }
}