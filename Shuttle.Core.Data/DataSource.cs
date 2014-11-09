using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DataSource
	{
		public DataSource(string name, IDbDataParameterFactory dbDataParameterFactory)
		{
			Guard.AgainstNull(dbDataParameterFactory, "dbDataParameterFactory");

			DbDataParameterFactory = dbDataParameterFactory;

			Name = name;
			Key = name.ToLower();
		}

		public string Name { get; private set; }
		public string Key { get; private set; }

		public IDbDataParameterFactory DbDataParameterFactory { get; private set; }
	}
}