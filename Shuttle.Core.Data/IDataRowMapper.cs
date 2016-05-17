using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDataRowMapper<T> where T : class
    {
        MappedRow<T> Map(DataRow row);
    }
}