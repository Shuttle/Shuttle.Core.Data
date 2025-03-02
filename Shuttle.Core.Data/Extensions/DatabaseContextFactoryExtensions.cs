using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class DatabaseContextFactoryExtensions
{
    public static async ValueTask<bool> IsAvailableAsync(this IDatabaseContextFactory databaseContextFactory, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
    {
        return await IsAvailableAsync(() =>
        {
            using (Guard.AgainstNull(databaseContextFactory).Create())
            {
            }
        }, cancellationToken, retries, secondsBetweenRetries);
    }

    public static async ValueTask<bool> IsAvailableAsync(this IDatabaseContextFactory databaseContextFactory, string name, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
    {
        return await IsAvailableAsync(() =>
        {
            using (Guard.AgainstNull(databaseContextFactory).Create(name))
            {
            }
        }, cancellationToken, retries, secondsBetweenRetries);
    }

    private static async ValueTask<bool> IsAvailableAsync(Action action, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
    {
        var attempt = 0;

        do
        {
            try
            {
                action.Invoke();

                break;
            }
            catch
            {
                attempt++;

                if (attempt < retries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(secondsBetweenRetries), cancellationToken);
                }
            }
        } while (attempt < retries);

        return attempt <= retries;
    }
}