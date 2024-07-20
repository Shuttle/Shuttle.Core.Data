using System;
using System.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextScope : IDisposable
    {
        private static readonly AsyncLocal<DatabaseContextCollection> AmbientData = new AsyncLocal<DatabaseContextCollection>();

        public DatabaseContextScope()
        {
            if (AmbientData.Value != null)
            {
                throw new InvalidOperationException(Resources.AmbientScopeException);
            }

            AmbientData.Value = new DatabaseContextCollection();
        }

        public static DatabaseContextCollection Current => AmbientData.Value;

        public void Dispose()
        {
            AmbientData.Value = null;
        }
    }
}