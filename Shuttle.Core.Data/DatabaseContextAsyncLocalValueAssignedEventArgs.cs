using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextAsyncLocalValueAssignedEventArgs : EventArgs
    {
        public DatabaseContextAmbientData AmbientData { get; }

        public DatabaseContextAsyncLocalValueAssignedEventArgs(DatabaseContextAmbientData ambientData)
        {
            AmbientData = Guard.AgainstNull(ambientData, nameof(ambientData));
        }
    }
}