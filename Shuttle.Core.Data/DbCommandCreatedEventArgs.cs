using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DbCommandCreatedEventArgs : EventArgs
    {
        public IDbCommand Command { get; }

        public DbCommandCreatedEventArgs(IDbCommand command)
        {
            Command = Guard.AgainstNull(command, nameof(command));
        }
    }
}