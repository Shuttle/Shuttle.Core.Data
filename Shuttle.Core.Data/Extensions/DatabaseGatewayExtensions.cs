﻿using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DatabaseGatewayExtensions
    {
        public static async Task<int> Execute(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.Execute(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<int> Execute(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.Execute(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<int> Execute(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.Execute(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<int> Execute(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.Execute(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<DataTable> GetDataTable(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTable(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<DataTable> GetDataTable(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTable(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<DataTable> GetDataTable(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTable(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<DataTable> GetDataTable(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTable(RawQuery.Create(sql, parameters), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReader(RawQuery.Create(sql), CancellationToken.None);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReader(RawQuery.Create(sql, parameters), CancellationToken.None);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReader(RawQuery.Create(sql), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReader(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<DataRow> GetRow(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRow(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<DataRow> GetRow(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRow(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<DataRow> GetRow(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRow(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<DataRow> GetRow(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRow(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRows(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRows(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<DataRow>> GetRows(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRows(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<DataRow>> GetRows(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRows(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRows(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRows(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> GetScalar<T>(this IDatabaseGateway databaseGateway, string sql)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalar<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<T> GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, dynamic parameters)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalar<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<T> GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalar<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalar<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }
    }
}