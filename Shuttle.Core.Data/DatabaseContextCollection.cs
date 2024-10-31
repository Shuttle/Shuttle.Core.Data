using System;
using System.Collections.Generic;

namespace Shuttle.Core.Data;

public class DatabaseContextCollection
{
    private readonly List<IDatabaseContext> _databaseContexts = new();

    public IDatabaseContext? ActiveDatabaseContext { get; private set; }
    public IEnumerable<IDatabaseContext> DatabaseContexts => _databaseContexts.AsReadOnly();

    internal void Activate(IDatabaseContext databaseContext)
    {
        ActiveDatabaseContext = databaseContext;
    }

    internal IDisposable Add(IDatabaseContext context)
    {
        if (_databaseContexts.Find(candidate => candidate.Name.Equals(context.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
            throw new(string.Format(Resources.DuplicateDatabaseContextException, context.Name));
        }

        _databaseContexts.Add(context);

        return new DatabaseContextCollectionRemover(this, context);
    }

    public IDatabaseContext? Find(Predicate<IDatabaseContext> match)
    {
        return _databaseContexts.Find(match);
    }

    internal void Remove(IDatabaseContext databaseContext)
    {
        if (_databaseContexts.Find(candidate => candidate.Name.Equals(databaseContext.Name)) == null)
        {
            throw new InvalidOperationException(string.Format(Resources.DatabaseContextKeyNotFoundException, databaseContext.Name, databaseContext.Name));
        }

        if (ActiveDatabaseContext != null && databaseContext.Name.Equals(ActiveDatabaseContext.Name))
        {
            ActiveDatabaseContext = null;
        }

        _databaseContexts.Remove(databaseContext);
    }
}

internal class DatabaseContextCollectionRemover : IDisposable
{
    private readonly DatabaseContextCollection _databaseContextCollection;
    private readonly IDatabaseContext _databaseContext;

    public DatabaseContextCollectionRemover(DatabaseContextCollection databaseContextCollection, IDatabaseContext databaseContext)
    {
        _databaseContextCollection = databaseContextCollection;
        _databaseContext = databaseContext;
    }

    public void Dispose()
    {
        _databaseContextCollection.Remove(_databaseContext);
    }
}