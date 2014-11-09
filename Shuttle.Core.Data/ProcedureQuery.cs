using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class ProcedureQuery : IQuery
    {
        private readonly Dictionary<IMappedColumn, object> parameterValues;
        private readonly string procedure;

        public ProcedureQuery(string procedure)
        {
            this.procedure = procedure;
            parameterValues = new Dictionary<IMappedColumn, object>();
        }

        public void Prepare(DataSource source, IDbCommand command)
        {
			Guard.AgainstNull(source, "source");
            Guard.AgainstNull(command, "command");

            command.CommandText = procedure;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var pair in parameterValues)
            {
                command.Parameters.Add(pair.Key.CreateDataParameter(source.DbDataParameterFactory, pair.Value));
            }
        }

	    public IQuery AddParameterValue(IMappedColumn column, object value)
        {
            parameterValues.Add(column, value);

            return this;
        }

        public static IQuery Create(string procedure)
        {
            return new ProcedureQuery(procedure);
        }
    }
}