using System;
using System.Collections.Generic;
using System.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextScope : IDisposable
    {
        private static readonly AsyncLocal<Stack<DatabaseContextCollection>> DatabaseContextCollectionStack = new AsyncLocal<Stack<DatabaseContextCollection>>();
        private static readonly AsyncLocal<DatabaseContextCollection> AmbientData = new AsyncLocal<DatabaseContextCollection>();

        public DatabaseContextScope()
        {
            if (DatabaseContextCollectionStack.Value == null)
            {
                DatabaseContextCollectionStack.Value = new Stack<DatabaseContextCollection>();
            }

            DatabaseContextCollectionStack.Value.Push(AmbientData.Value);

            AmbientData.Value = new DatabaseContextCollection();
        }

        public static DatabaseContextCollection Current => AmbientData.Value;

        public void Dispose()
        {
            AmbientData.Value = DatabaseContextCollectionStack.Value.Pop();
        }
    }
}