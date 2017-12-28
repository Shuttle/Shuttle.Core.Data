using System;
using System.Data.Common;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DefaultDbProviderFactories : IDbProviderFactories
    {
        public DbProviderFactory GetFactory(string providerName)
        {
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));

            switch (providerName.ToLowerInvariant())
            {
                case "System.Data.SqlClient":
                {
                    return new DefaultSqlProviderFactory();
                }
                default:
                {
                    throw new InvalidOperationException(string.Format(Resources.UnknownProviderNameException, providerName));
                }
            }
        }
    }
}