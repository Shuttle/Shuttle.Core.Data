using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class Bootstrap : IComponentRegistryBootstrap
	{
		public void Register(IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, "registry");

			registry.AttemptRegister<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
			registry.AttemptRegister<IDatabaseContextFactory, DatabaseContextFactory>();
			registry.AttemptRegister<IDbConnectionFactory, DbConnectionFactory>();
			registry.AttemptRegister<IDbCommandFactory, DbCommandFactory>();
			registry.AttemptRegister<IDatabaseGateway, DatabaseGateway>();
			registry.AttemptRegister<IQueryMapper, QueryMapper>();
		}
	}
}