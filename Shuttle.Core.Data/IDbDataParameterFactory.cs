using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDbDataParameterFactory
    {
        IDbDataParameter Create(string name, DbType type, object value);
        IDbDataParameter Create(string name, DbType type, int size, object value);
        IDbDataParameter Create(string name, DbType type, byte precision, byte scale, object value);
    }
}