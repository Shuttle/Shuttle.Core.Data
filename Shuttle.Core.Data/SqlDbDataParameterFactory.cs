using System;
using System.Data;
using System.Data.SqlClient;

namespace Shuttle.Core.Data
{
    public class SqlDbDataParameterFactory : IDbDataParameterFactory
    {
        public IDbDataParameter Create(string name, DbType type, object value)
        {
            var result = new SqlParameter(name, value ?? DBNull.Value);

            try
            {
                result.DbType = type;
            }
            catch
            {
				throw new ArgumentException(string.Format("Cannot convert DbType {0} to an equivalent SqlDbType.", type));
            }

            return result;
        }

        public IDbDataParameter Create(string name, DbType type, int size, object value)
        {
            var result = new SqlParameter(name, value ?? DBNull.Value)
                             {
                                 Size = size
                             };

            try
            {
                result.DbType = type;
            }
            catch
            {
				throw new ArgumentException(string.Format("Cannot convert DbType {0} to an equivalent SqlDbType.", type));
			}

            return result;
        }

        public IDbDataParameter Create(string name, DbType type, byte precision, byte scale, object value)
        {
            var result = new SqlParameter(name, value ?? DBNull.Value)
                             {
                                 Precision = precision,
                                 Scale = scale
                             };

            try
            {
                result.DbType = type;
            }
            catch
            {
				throw new ArgumentException(string.Format("Cannot convert DbType {0} to an equivalent SqlDbType.", type));
			}

            return result;
        }
    }
}
