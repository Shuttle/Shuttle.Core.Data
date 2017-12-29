using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ProcedureQuery : IQueryParameter
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
            Guard.AgainstNull(command, nameof(command));

            command.CommandText = _procedure;
            command.CommandType = CommandType.StoredProcedure;

            foreach (var pair in _parameterValues)
            {
                command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
            }
        }

        public IQueryParameter AddParameterValue(IMappedColumn column, object value)
        {
            _parameterValues.Add(column, value);

            return this;
        }

        public static IQueryParameter Create(string procedure)
        {
            return new ProcedureQuery(procedure);
        }
    }
}