using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DatabaseGatewayExtensions
    {
        public static int Execute(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.Execute(RawQuery.Create(sql, parameters));
        }

        public static DataTable GetDataTable(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetDataTable(RawQuery.Create(sql, parameters));
        }

        public static IDataReader GetReader(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetReader(RawQuery.Create(sql, parameters));
        }

        public static DataRow GetRow(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRow(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<DataRow> GetRows(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRows(RawQuery.Create(sql, parameters));
        }

        public static T GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetScalar<T>(RawQuery.Create(sql, parameters));
        }
    }
}