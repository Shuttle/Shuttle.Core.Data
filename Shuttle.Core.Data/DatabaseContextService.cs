using System;
using System.Linq;
using System.Threading;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private const string AmbientDataKey = "__DatabaseContextService-AmbientData__";
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static AsyncLocal<DatabaseContextAmbientData> _ambientData;

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

        public event EventHandler<DatabaseContextAsyncLocalValueChangedEventArgs> DatabaseContextAsyncLocalValueChanged;
        public event EventHandler<EventArgs> DatabaseContextAsyncLocalAssigned ;
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
            if (_ambientData == null)
            {
                _ambientData = new AsyncLocal<DatabaseContextAmbientData>(args =>
                {
                    DatabaseContextAsyncLocalValueChanged?.Invoke(this, new DatabaseContextAsyncLocalValueChangedEventArgs(args));
                });

                DatabaseContextAsyncLocalAssigned?.Invoke(this, EventArgs.Empty);
            }

            if (_ambientData.Value == null)
            {
                _ambientData.Value = new DatabaseContextAmbientData();

                DatabaseContextAsyncLocalValueAssigned?.Invoke(this, new DatabaseContextAsyncLocalValueAssignedEventArgs(_ambientData.Value));
            }

            return _ambientData.Value;
        }
    }
}