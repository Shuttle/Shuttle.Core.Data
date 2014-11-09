using System.Data;

namespace Shuttle.Core.Data
{
    public class MappedRow<T>
    {
        public DataRow Row { get; private set; }
        public T Result { get; private set; }

        public MappedRow(DataRow row, T result)
        {
            Row = row;
            Result = result;
        }
    }
}