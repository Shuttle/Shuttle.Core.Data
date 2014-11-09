using System;
using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public class ThreadStaticDatabaseConnectionCache : IDatabaseConnectionCache
    {
        [ThreadStatic]
        private static Dictionary<string, IDatabaseConnection> connections;

        public IDatabaseConnection Get(DataSource source)
        {
            Guard();

            if (!connections.ContainsKey(source.Key))
            {
                throw new ApplicationException(string.Format(DataResources.ThreadStaticDatabaseConnectionCacheMissingEntry, source.Name));
            }

            return connections[source.Key];
        }

        public IDatabaseConnection Add(DataSource source, IDatabaseConnection connection)
        {
			Infrastructure.Guard.AgainstNull(connection, "connection");
			
			Guard();

            connections.Add(source.Key, connection);

        	return connection;
        }

        public void Remove(DataSource source)
        {
            Guard();

            connections.Remove(source.Key);
        }

        public bool Contains(DataSource source)
        {
            Guard();

            return connections.ContainsKey(source.Key);
        }

        private static void Guard()
        {
            if (connections == null)
            {
                connections = new Dictionary<string, IDatabaseConnection>();
            }
        }
    }
}