using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class QueryExtensions
    {
        public static IQuery AddParameters(this IQuery query, object parameters)
        {
            Guard.AgainstNull(query, nameof(query));

            if (parameters != null)
            {
                foreach (var pi in (parameters).GetType().GetProperties())
                {
                    try
                    {
                        query.AddParameter(new Column(pi.Name, pi.PropertyType, Column.GetDbType(pi.PropertyType)), pi.GetValue(parameters));
                    }
                    catch
                    {
                        throw new InvalidOperationException(string.Format(Resources.DynamicGetValueException, pi.Name));
                    }
                }
            }

            return query;
        }
    }
}