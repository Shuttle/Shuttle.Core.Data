using System;
using System.Data;

namespace Shuttle.Core.Data
{
	public class MappedColumn<T> : IMappedColumn
	{
		public string ColumnName { get; protected set; }
		public DbType DbType { get; private set; }
		public int? Size { get; protected set; }
		public byte? Precision { get; protected set; }
		public byte? Scale { get; protected set; }

		private Type underlyingSystemType;

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

		public static implicit operator string(MappedColumn<T> column)
		{
			return column.ColumnName;
		}

		private void GetUnderlyingSystemType()
		{
			underlyingSystemType = Nullable.GetUnderlyingType(typeof (T)) ?? typeof (T);
		}

		public T MapFrom(DataRow row)
		{
			if (row.Table.Columns.Contains(ColumnName))
			{
				return (row.IsNull(ColumnName)
					        ? default(T)
					        : (T) Convert.ChangeType(RetrieveRawValueFrom(row), underlyingSystemType));
			}

			return default(T);
		}

		public T MapFrom(IDataRecord record)
		{
			var ordinal = Ordinal(record);

			if (ordinal > -1)
			{
				return (record.IsDBNull(ordinal)
					        ? default(T)
					        : (T) Convert.ChangeType(RetrieveRawValueFrom(record), underlyingSystemType));
			}

			return default(T);
		}

		private object RetrieveRawValueFrom(IDataRecord record)
		{
			return record[ColumnName];
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

		public IDbDataParameter CreateDataParameter(IDbDataParameterFactory factory, object value)
		{
			return Size.HasValue
				       ? factory.Create("@" + FlattenedColumnName(), DbType, Size.Value, value)
				       : (Precision.HasValue
					          ? factory.Create("@" + FlattenedColumnName(), DbType, Precision.Value, Scale ?? 0, value)
					          : factory.Create("@" + FlattenedColumnName(), DbType, value));
		}

		public MappedColumn<T> Rename(string name)
		{
			return Size.HasValue
				       ? new MappedColumn<T>(name, DbType, Size.Value)
				       : Precision.HasValue
					         ? new MappedColumn<T>(name, DbType, Precision.Value, Scale ?? 0)
					         : new MappedColumn<T>(name, DbType);
		}
	}
}