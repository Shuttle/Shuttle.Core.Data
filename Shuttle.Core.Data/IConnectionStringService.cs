using System.Configuration;

namespace Shuttle.Core.Data
{
	public interface IConnectionStringService
	{
		void Approve();
		void Approve(ConnectionStringSettingsCollection connectionStrings);
	}
}