using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;

namespace Shuttle.Core.Data
{
	public class ConnectionStringService : IConnectionStringService
	{
	    private readonly ILog _log;

#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
	    public ConnectionStringService()
	    {
	        _log = Log.For(this);
	    }
#else
	    private readonly IDbProviderFactories _providerFactories;

	    public ConnectionStringService(IDbProviderFactories providerFactories)
	    {
            Guard.AgainstNull(providerFactories, nameof(providerFactories));

	        _providerFactories = providerFactories;
	        _log = Log.For(this);
	    }
#endif

        public void Approve()
	    {
			Approve(ConfigurationManager.ConnectionStrings);
	    }

	    public void Approve(ConnectionStringSettingsCollection connectionStrings)
        {
            foreach (ConnectionStringSettings settings in connectionStrings)
            {
                try
                {
#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
                    var dbProviderFactory = DbProviderFactories.GetFactory(settings.ProviderName);
#else
        		    var dbProviderFactory = _providerFactories.GetFactory(settings.ProviderName);
#endif
                    using (var connection = dbProviderFactory.CreateConnection())
                    {
						Guard.AgainstNull(connection, nameof(connection));

// ReSharper disable PossibleNullReferenceException
                        connection.ConnectionString = settings.ConnectionString;
// ReSharper restore PossibleNullReferenceException

                        connection.Open();
                    }

					_log.Information(string.Format(Resources.ConnectionStringApproved, settings.Name));
                }
                catch (Exception ex)
                {
					var message = string.Format(Resources.ConnectionStringApproveException, settings.Name, ex.Message);

                    _log.Error(message);

                    throw new DataException(message);
                }
            }
        }
    }
}