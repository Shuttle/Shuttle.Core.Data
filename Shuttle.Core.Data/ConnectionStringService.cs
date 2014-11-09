using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class ConnectionStringService : IConnectionStringService
	{
	    private readonly ILog _log;

	    public ConnectionStringService()
	    {
		    _log = Log.For(this);
	    }

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
                    using (var connection = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection())
                    {
						Guard.AgainstNull(connection, "connection");

// ReSharper disable PossibleNullReferenceException
                        connection.ConnectionString = settings.ConnectionString;
// ReSharper restore PossibleNullReferenceException

                        connection.Open();
                    }

					_log.Information(string.Format(DataResources.ConnectionStringApproved, settings.Name));
                }
                catch (Exception ex)
                {
                    var message = string.Format(DataResources.DbConnectionOpenException, settings.Name, ex.Message);

                    _log.Error(message);

                    throw new DataException(message);
                }
            }
        }
    }
}