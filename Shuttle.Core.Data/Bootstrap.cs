using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class Bootstrap : IComponentRegistryBootstrap
	{
		public void Register(IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, nameof(registry));

			registry.AttemptRegister<IConnectionConfigurationProvider, ConnectionConfigurationProvider>();
			registry.AttemptRegister<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
			registry.AttemptRegister<IDatabaseContextFactory, DatabaseContextFactory>();
			registry.AttemptRegister<IDbConnectionFactory, DbConnectionFactory>();
			registry.AttemptRegister<IDbCommandFactory, DbCommandFactory>();
			registry.AttemptRegister<IDatabaseGateway, DatabaseGateway>();
			registry.AttemptRegister<IQueryMapper, QueryMapper>();
		    registry.AttemptRegisterGeneric(typeof(IDataRepository<>), typeof(DataRepository<>));
		}
	}
}