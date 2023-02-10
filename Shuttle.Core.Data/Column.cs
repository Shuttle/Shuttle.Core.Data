using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class Column : IColumn
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

        public Column(string name, Type type, DbType dbType)
            : this(name, type, dbType, null)
        {
            Type = type;
        }

        public Column(string name, Type type, DbType dbType, int? size)
        {
            Name = name;
            Type = type;
            DbType = dbType;
            Size = size;
            Precision = null;
            Scale = null;
        }

        public Column(string name, Type type, DbType dbType, byte precision, byte scale)
        {
            Name = name;
            Type = type;
            DbType = dbType;
            Precision = precision;
            Scale = scale;
            Size = null;
        }

        public Type Type { get; }

        public string Name { get; protected set; }
        public DbType DbType { get; }
        public int? Size { get; protected set; }
        public byte? Precision { get; protected set; }
        public byte? Scale { get; protected set; }

        public string FlattenedName()
        {
            return Name.Replace(".", "_");
        }

        public object RawValue(dynamic instance)
        {
            Guard.AgainstNull(instance, nameof(instance));

            var property = instance.GetType().GetProperty(Name);

            if (property == null)
            {
                return null;
            }

            return property.GetValue(instance, null);
        }

        public object RawValue(DataRow row)
        {
            return Guard.AgainstNull(row, nameof(row))[Name];
        }

        public object RawValue(IDataRecord record)
        {
            return Guard.AgainstNull(record, nameof(record))[Name];
        }

        public bool IsNull(dynamic instance)
        {
            return Guard.AgainstNull(instance, nameof(instance))[Name] == DBNull.Value;
        }

        public bool IsNull(DataRow row)
        {
            return Guard.AgainstNull(row, nameof(row)).IsNull(Name);
        }

        public bool IsNull(IDataRecord record)
        {
            return Guard.AgainstNull(record, nameof(record)).IsDBNull(Ordinal(record));
        }

        public IDbDataParameter CreateDataParameter(IDbCommand command, object value)
        {
            var result = command.CreateParameter();

            result.ParameterName = string.Concat("@", FlattenedName());
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

        public static DbType GetDbType(Type type)
        {
            Guard.AgainstNull(type, nameof(type));

            if (!DbTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format(Resources.DbTypeMappingException, type.FullName));
            }

            return DbTypes[type];
        }

        protected bool HasProperty(dynamic instance, string name)
        {
            return instance != null
                   &&
                   (
                       instance is ExpandoObject
                           ? ((IDictionary<string, object>)instance).ContainsKey(name)
                           : (bool)(instance.GetType().GetProperty(name) != null)
                   );
        }

        public T Value<T>(dynamic instance)
        {
            Guard.AgainstNull(instance, nameof(instance));

            if (HasProperty(instance, Name))
            {
                var type = typeof(T);
                var value = instance.GetType().GetProperty(Name).GetValue(instance, null);

                return value == null || DBNull.Value.Equals(value)
                    ? default
                    : (T)Convert.ChangeType(RawValue(value), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public T Value<T>(DataRow row)
        {
            Guard.AgainstNull(row, nameof(row));

            if (row.Table.Columns.Contains(Name))
            {
                var type = typeof(T);

                return row.IsNull(Name)
                    ? default
                    : (T)Convert.ChangeType(RawValue(row), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public T Value<T>(IDataRecord record)
        {
            Guard.AgainstNull(record, nameof(record));

            var ordinal = Ordinal(record);

            if (ordinal > -1)
            {
                var type = typeof(T);

                return record.IsDBNull(ordinal)
                    ? default
                    : (T)Convert.ChangeType(RawValue(record), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public static implicit operator string(Column column)
        {
            Guard.AgainstNull(column, nameof(column));

            return column.Name;
        }

        protected int Ordinal(IDataRecord record)
        {
            try
            {
                return record.GetOrdinal(Name);
            }
            catch
            {
                return -1;
            }
        }

        public Column Rename(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return Size.HasValue
                ? new Column(name, Type, DbType, Size.Value)
                : Precision.HasValue
                    ? new Column(name, Type, DbType, Precision.Value, Scale ?? 0)
                    : new Column(name, Type, DbType);
        }
    }

    public class Column<T> : Column
    {
        private Type _underlyingSystemType;

        public Column(string name, DbType dbType)
            : this(name, dbType, null)
        {
        }

        public Column(string name, DbType dbType, int? size) : base(name, typeof(T), dbType, size)
        {
            GetUnderlyingSystemType();
        }

        public Column(string name, DbType dbType, byte precision, byte scale) : base(name, typeof(T), dbType, precision, scale)
        {
            GetUnderlyingSystemType();
        }

        private void GetUnderlyingSystemType()
        {
            _underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        }

        public T Value(dynamic instance)
        {
            Guard.AgainstNull(instance, nameof(instance));

            if (HasProperty(instance, Name))
            {
                var type = typeof(T);
                var value = instance.GetType().GetProperty(Name).GetValue(instance, null);

                return value == null
                    ? default
                    : (T)Convert.ChangeType(RawValue(instance), Nullable.GetUnderlyingType(type) ?? type);
            }

            return default;
        }

        public T Value(DataRow row)
        {
            Guard.AgainstNull(row, nameof(row));

            if (row.Table.Columns.Contains(Name))
            {
                return row.IsNull(Name)
                    ? default
                    : (T)Convert.ChangeType(RawValue(row), _underlyingSystemType);
            }

            return default;
        }

        public T Value(IDataRecord record)
        {
            Guard.AgainstNull(record, nameof(record));

            var ordinal = Ordinal(record);

            if (ordinal > -1)
            {
                return record.IsDBNull(ordinal)
                    ? default
                    : (T)Convert.ChangeType(RawValue(record), _underlyingSystemType);
            }

            return default;
        }

        public static implicit operator string(Column<T> column)
        {
            Guard.AgainstNull(column, nameof(column));

            return column.Name;
        }

        public new Column<T> Rename(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return Size.HasValue
                ? new Column<T>(name, DbType, Size.Value)
                : Precision.HasValue
                    ? new Column<T>(name, DbType, Precision.Value, Scale ?? 0)
                    : new Column<T>(name, DbType);
        }
    }
}