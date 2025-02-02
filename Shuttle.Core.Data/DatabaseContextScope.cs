using System;
using System.Collections.Generic;
using System.Threading;

namespace Shuttle.Core.Data;

public class DatabaseContextScope : IDisposable
{
    private static readonly AsyncLocal<Stack<DatabaseContextCollection>> DatabaseContextCollectionStack = new();
    private static readonly AsyncLocal<DatabaseContextCollection?> AmbientData = new();

    public DatabaseContextScope()
    {
        AmbientData.Value = new();
        DatabaseContextCollectionStack.Value ??= new();
        DatabaseContextCollectionStack.Value.Push(AmbientData.Value);
    }

    public static DatabaseContextCollection? Current => AmbientData.Value;

    public void Dispose()
    {
        if (DatabaseContextCollectionStack.Value == null ||
            DatabaseContextCollectionStack.Value.Count == 0)
        {
            AmbientData.Value = null;
            return;
        }

        AmbientData.Value = DatabaseContextCollectionStack.Value?.Pop();
    }
}