using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class MappedColumn<T> : IMappedColumn
    {
        private Type _underlyingSystemType;

        public MappedColumn(string columnName, DbType type)
            : this(columnName, type, null)
        {
        }

        public MappedColumn(string columnName, DbType type, int? size)
        {
            ColumnName = columnName;
            DbType = type;
            Size = size;
            Precision = null;
            Scale = null;

            GetUnderlyingSystemType();
        }

        public MappedColumn(string columnName, DbType type, byte precision, byte scale)
        {
            ColumnName = columnName;
            DbType = type;
            Precision = precision;
            Scale = scale;
            Size = null;

            GetUnderlyingSystemType();
        }

        public string ColumnName { get; protected set; }
        public DbType DbType { get; }
        public int? Size { get; protected set; }
        public byte? Precision { get; protected set; }
        public byte? Scale { get; protected set; }

        public string FlattenedColumnName()
        {
            return ColumnName.Replace(".", "_");
        }

        public object RetrieveRawValueFrom(DataRow row)
        {
            return row[ColumnName];
        }

        public bool IsNullFor(DataRow row)
        {
            return row.IsNull(ColumnName);
        }

        public IDbDataParameter CreateDataParameter(IDbCommand command, object value)
        {
            var result = command.CreateParameter();

            result.ParameterName = string.Concat("@", FlattenedColumnName());
            result.DbType = DbType;
            result.Value = value ?? DBNull.Value;

            if (Size.HasValue)
            {
                result.Size = Size.Value;
            }

            if (Precision.HasValue)
            {
                result.Precision = Precision.Value;
            }

            result.Scale = Scale ?? 0;

            return result;
        }

        private void GetUnderlyingSystemType()
        {
            _underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        }

        public T MapFrom(DataRow row)
        {
            Guard.AgainstNull(row, nameof(row));

            if (row.Table.Columns.Contains(ColumnName))
            {
                return row.IsNull(ColumnName)
                    ? default
                    : (T)Convert.ChangeType(RetrieveRawValueFrom(row), _underlyingSystemType);
            }

            return default;
        }

        public T MapFrom(IDataRecord record)
        {
            Guard.AgainstNull(record, nameof(record));

            var ordinal = Ordinal(record);

            if (ordinal > -1)
            {
                return record.IsDBNull(ordinal)
                    ? default
                    : (T)Convert.ChangeType(RetrieveRawValueFrom(record), _underlyingSystemType);
            }

            return default;
        }

        public static implicit operator string(MappedColumn<T> column)
        {
            Guard.AgainstNull(column, nameof(column));

            return column.ColumnName;
        }

        private int Ordinal(IDataRecord reader)
        {
            try
            {
                return reader.GetOrdinal(ColumnName);
            }
            catch
            {
                return -1;
            }
        }

        public MappedColumn<T> Rename(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return Size.HasValue
                ? new MappedColumn<T>(name, DbType, Size.Value)
                : Precision.HasValue
                    ? new MappedColumn<T>(name, DbType, Precision.Value, Scale ?? 0)
                    : new MappedColumn<T>(name, DbType);
        }

        private object RetrieveRawValueFrom(IDataRecord record)
        {
            return record[ColumnName];
        }
    }
}