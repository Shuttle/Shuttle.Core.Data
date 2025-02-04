﻿using System;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class DatabaseContextFactoryExtensions
{
    public static bool IsAvailable(this IDatabaseContextFactory databaseContextFactory, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
    {
        return IsAvailable(() =>
        {
            using (Guard.AgainstNull(databaseContextFactory).Create())
            {
            }
        }, cancellationToken, retries, secondsBetweenRetries);
    }

    public static bool IsAvailable(this IDatabaseContextFactory databaseContextFactory, string name, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
    {
        return IsAvailable(() =>
        {
            using (Guard.AgainstNull(databaseContextFactory).Create(name))
            {
            }
        }, cancellationToken, retries, secondsBetweenRetries);
    }

    private static bool IsAvailable(Action action, CancellationToken cancellationToken, int retries = 4, int secondsBetweenRetries = 15)
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
                    var wait = DateTime.Now.AddSeconds(secondsBetweenRetries);

                    while (!cancellationToken.IsCancellationRequested && DateTime.Now < wait)
                    {
                        Thread.Sleep(250);
                    }
                }
            }
        } while (attempt < retries);

        return attempt <= retries;
    }
}