using System;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextAsyncLocalValueChangedEventArgs : EventArgs
    {
        public AsyncLocalValueChangedArgs<DatabaseContextAmbientData> AsyncLocalValueChangedArgs { get; }

        public DatabaseContextAsyncLocalValueChangedEventArgs(AsyncLocalValueChangedArgs<DatabaseContextAmbientData> asyncLocalValueChangedArgs)
        {
            AsyncLocalValueChangedArgs = Guard.AgainstNull(asyncLocalValueChangedArgs, nameof(asyncLocalValueChangedArgs));
        }
    }
}