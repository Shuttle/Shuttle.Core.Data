using System;
using System.Linq;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private static AsyncLocal<DatabaseContextAmbientData> _ambientData;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public DatabaseContextService()
        {
            if (_ambientData != null)
            {
                return;
            }

            _ambientData = new AsyncLocal<DatabaseContextAmbientData>(OnAsyncLocalValueChanged);
        }

        public IDatabaseContext Current
        {
            get
            {
                _lock.Wait();

                try
                {
                    return GetAmbientData().ActiveDatabaseContext ?? throw new InvalidOperationException(Resources.DatabaseContextMissing);
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public IDisposable BeginScope()
        {
            _ambientData.Value = new DatabaseContextAmbientData();

            DatabaseContextAsyncLocalValueAssigned?.Invoke(this, new DatabaseContextAsyncLocalValueAssignedEventArgs(_ambientData.Value, true));

            return new AmbientScope();
        }

        public event EventHandler<DatabaseContextAsyncLocalValueChangedEventArgs> DatabaseContextAsyncLocalValueChanged;
        public event EventHandler<DatabaseContextAsyncLocalValueAssignedEventArgs> DatabaseContextAsyncLocalValueAssigned;

        public void Activate(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                var current = GetAmbientData().ActiveDatabaseContext;

                if (current != null && current.Name.Equals(databaseContext.Name))
                {
                    throw new Exception(string.Format(Resources.DatabaseContextAlreadyActiveException, databaseContext.Name));
                }

                var activate = GetAmbientData().DatabaseContexts.FirstOrDefault(item => item.Name.Equals(databaseContext.Name, StringComparison.InvariantCultureIgnoreCase));

                if (activate == null)
                {
                    throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, databaseContext.Name));
                }

                GetAmbientData().Activate(activate);
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Add(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                GetAmbientData().Add(databaseContext);
            }
            finally
            {
                _lock.Release();
            }

            Activate(databaseContext);
        }

        public void Remove(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                GetAmbientData().Remove(databaseContext);
            }
            finally
            {
                _lock.Release();
            }
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            Guard.AgainstNull(match, nameof(match));

            _lock.Wait();

            try
            {
                return GetAmbientData().Find(match);
            }
            finally
            {
                _lock.Release();
            }
        }

        private DatabaseContextAmbientData GetAmbientData()
        {
            if (_ambientData.Value == null)
            {
                throw new InvalidOperationException(Resources.NoAmbientScopeException);
            }

            return _ambientData.Value;
        }

        private void OnAsyncLocalValueChanged(AsyncLocalValueChangedArgs<DatabaseContextAmbientData> args)
        {
            DatabaseContextAsyncLocalValueChanged?.Invoke(this, new DatabaseContextAsyncLocalValueChangedEventArgs(args));
        }

        private class AmbientScope : IDisposable
        {
            public void Dispose()
            {
                _ambientData.Value = null;
            }
        }
    }
}