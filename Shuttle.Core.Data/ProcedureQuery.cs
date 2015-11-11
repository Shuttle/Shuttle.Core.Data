using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class ProcedureQuery : IQueryParemeter
    {
        private readonly Dictionary<IMappedColumn, object> _parameterValues;
        private readonly string _procedure;

        public ProcedureQuery(string procedure)
        {
            _procedure = procedure;
            _parameterValues = new Dictionary<IMappedColumn, object>();
        }

        public void Prepare(IDbCommand command)
        {
            Guard.AgainstNull(command, "command");

            command.CommandText = _procedure;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var pair in _parameterValues)
            {
                command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
            }
        }

	    public IQuery AddParameterValue(IMappedColumn column, object value)
        {
            _parameterValues.Add(column, value);

            return this;
        }

        public static IQueryParemeter Create(string procedure)
        {
            return new ProcedureQuery(procedure);
        }
    }
}