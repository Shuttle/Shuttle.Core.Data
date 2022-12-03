using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class MappedColumn : IMappedColumn
    {
        private static readonly Dictionary<Type, DbType> DbTypes = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
        };

        public static DbType GetDbType(Type type)
        {
            Guard.AgainstNull(type, nameof(type));

            if (!DbTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format(Resources.DbTypeMappingException, type.FullName));
            }

            return DbTypes[type];
        }

        public Type Type { get; }

        public MappedColumn(string columnName, Type type, DbType dbType)
            : this(columnName, type, dbType, null)
        {
            Type = type;
        }

        public MappedColumn(string columnName, Type type, DbType dbType, int? size)
        {
            ColumnName = columnName;
            Type = type;
            DbType = dbType;
            Size = size;
            Precision = null;
            Scale = null;
        }

        public MappedColumn(string columnName, Type type, DbType dbType, byte precision, byte scale)
        {
            ColumnName = columnName;
            Type = type;
            DbType = dbType;
            Precision = precision;
            Scale = scale;
            Size = null;
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

        public T MapFrom<T>(DataRow row)
        {
            Guard.AgainstNull(row, nameof(row));

            if (row.Table.Columns.Contains(ColumnName))
            {
                var type = typeof(T);

                return row.IsNull(ColumnName)
                    ? default
                    : (T)Convert.ChangeType(RetrieveRawValueFrom(row), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public T MapFrom<T>(IDataRecord record)
        {
            Guard.AgainstNull(record, nameof(record));

            var ordinal = Ordinal(record);

            if (ordinal > -1)
            {
                var type = typeof(T);

                return record.IsDBNull(ordinal)
                    ? default
                    : (T)Convert.ChangeType(RetrieveRawValueFrom(record), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public static implicit operator string(MappedColumn column)
        {
            Guard.AgainstNull(column, nameof(column));

            return column.ColumnName;
        }

        protected int Ordinal(IDataRecord reader)
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

        public MappedColumn Rename(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return Size.HasValue
                ? new MappedColumn(name, Type, DbType, Size.Value)
                : Precision.HasValue
                    ? new MappedColumn(name, Type, DbType, Precision.Value, Scale ?? 0)
                    : new MappedColumn(name, Type, DbType);
        }

        public object RetrieveRawValueFrom(IDataRecord record)
        {
            return record[ColumnName];
        }


    }

    public class MappedColumn<T> : MappedColumn
    {
        private Type _underlyingSystemType;

        public MappedColumn(string columnName, DbType dbType)
            : this(columnName, dbType, null)
        {
        }

        public MappedColumn(string columnName, DbType dbType, int? size) : base(columnName, typeof(T), dbType, size)
        {
            GetUnderlyingSystemType();
        }

        public MappedColumn(string columnName, DbType dbType, byte precision, byte scale) : base(columnName, typeof(T), dbType, precision, scale)
        {
            GetUnderlyingSystemType();
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

        public new MappedColumn<T> Rename(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return Size.HasValue
                ? new MappedColumn<T>(name, DbType, Size.Value)
                : Precision.HasValue
                    ? new MappedColumn<T>(name, DbType, Precision.Value, Scale ?? 0)
                    : new MappedColumn<T>(name, DbType);
        }
    }
}