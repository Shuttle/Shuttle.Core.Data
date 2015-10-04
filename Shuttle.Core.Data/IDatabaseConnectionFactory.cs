namespace Shuttle.Core.Data
{
	public interface IDatabaseConnectionFactory 
    {
	    IDatabaseConnection Create(string connectionStringName);
		IDatabaseConnection Create(string providerName, string connectionString);
    }
}