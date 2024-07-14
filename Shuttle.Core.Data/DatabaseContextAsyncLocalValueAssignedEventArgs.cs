using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextAsyncLocalValueAssignedEventArgs : EventArgs
    {
        public bool ExplicitAmbientScope { get; }
        public DatabaseContextAmbientData AmbientData { get; }

        public DatabaseContextAsyncLocalValueAssignedEventArgs(DatabaseContextAmbientData ambientData, bool explicitAmbientScope)
        {
            ExplicitAmbientScope = explicitAmbientScope;
            AmbientData = Guard.AgainstNull(ambientData, nameof(ambientData));
        }
    }
}