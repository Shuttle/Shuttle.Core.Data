using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ProcedureQuery : IQuery
    {
        private readonly Dictionary<IColumn, object> _parameterValues;
        private readonly string _procedure;

        public ProcedureQuery(string procedure)
        {
            Guard.AgainstNullOrEmptyString(procedure, nameof(procedure));

            _procedure = procedure;
            _parameterValues = new Dictionary<IColumn, object>();
        }

        public void Prepare(IDbCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            command.CommandText = _procedure;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var pair in _parameterValues)
            {
                command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
            }
        }

        public IQuery AddParameterValue(IColumn column, object value)
        {
            Guard.AgainstNull(column, nameof(column));

            _parameterValues.Add(column, value);

            return this;
        }
    }
}