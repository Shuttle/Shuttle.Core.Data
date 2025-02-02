using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class DatabaseContextExtensions
{
    public static async Task<int> ExecuteAsync(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        await using (var command = await Guard.AgainstNull(databaseContext).CreateCommandAsync(Guard.AgainstNull(query)))
        {
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public static async Task<DataTable> GetDataTableAsync(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(query);

        var results = new DataTable();

        await using (var reader = await GetReaderAsync(databaseContext, query, cancellationToken).ConfigureAwait(false))
        {
            results.Load(reader);
        }

        return results;
    }

    public static async Task<DbDataReader> GetReaderAsync(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        await using (var command = await Guard.AgainstNull(databaseContext).CreateCommandAsync(Guard.AgainstNull(query)))
        {
            return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        }
    
    }

    public static async Task<DataRow?> GetRowAsync(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        var table = await GetDataTableAsync(databaseContext, query, cancellationToken).ConfigureAwait(false);

        return table.Rows.Count == 0 ? null : table.Rows[0];
    }

    public static async Task<IEnumerable<DataRow>> GetRowsAsync(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        return (await GetDataTableAsync(databaseContext, query, cancellationToken).ConfigureAwait(false)).Rows.Cast<DataRow>();
    }

    public static async Task<T?> GetScalarAsync<T>(this IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(query);

        await using (var command = await Guard.AgainstNull(databaseContext).CreateCommandAsync(Guard.AgainstNull(query)))
        {
            var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return scalar != null && scalar != DBNull.Value ? (T)scalar : default;
        }
    }
}