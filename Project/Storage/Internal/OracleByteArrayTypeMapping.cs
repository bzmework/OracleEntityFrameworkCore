using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 字节数组类型映射
	/// </summary>
	public class OracleByteArrayTypeMapping : ByteArrayTypeMapping
	{
		//private const int MaxSize = int.MaxValue;

		private readonly int _maxSpecificSize;

		private readonly StoreTypePostfix? _storeTypePostfix;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		/// <param name="size">大小</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="comparer"></param>
		/// <param name="storeTypePostfix"></param>
		public OracleByteArrayTypeMapping(
			[NotNull] string storeType, 
			[CanBeNull] DbType? dbType = System.Data.DbType.Binary, 
			int? size = null, 
			bool fixedLength = false, 
			ValueComparer comparer = null, 
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(byte[]), null, comparer), storeType, GetStoreTypePostfix(storeTypePostfix, size), dbType, unicode: false, size, fixedLength))
		{
			_storeTypePostfix = storeTypePostfix;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleByteArrayTypeMapping(RelationalTypeMappingParameters parameters)
			: base(parameters)
		{
			_maxSpecificSize = CalculateSize(parameters.Size);
		}

		private static StoreTypePostfix GetStoreTypePostfix(StoreTypePostfix? storeTypePostfix, int? size)
		{
			StoreTypePostfix? storeTypePostfix2 = storeTypePostfix;
			if (!storeTypePostfix2.HasValue)
			{
				if (!size.HasValue || !(size <= int.MaxValue))
				{
					return StoreTypePostfix.None;
				}
				return StoreTypePostfix.Size;
			}
			return storeTypePostfix2.GetValueOrDefault();
		}

		private static int CalculateSize(int? size)
		{
			if (!size.HasValue || !(size < int.MaxValue))
			{
				return int.MaxValue;
			}
			return size.Value;
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="size">大小</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleByteArrayTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size, GetStoreTypePostfix(_storeTypePostfix, size)));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">值转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleByteArrayTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="command">命令</param>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <param name="nullable">是否可空</param>
		/// <returns></returns>
		public override DbParameter CreateParameter(
			[NotNull] DbCommand command, 
			[NotNull] string name, 
			[CanBeNull] object value, 
			bool? nullable = null)
		{
			Check.NotNull(command, nameof(command));
			OracleParameter val = (OracleParameter)(object)(OracleParameter)base.CreateParameter(command, name, value, nullable);
			if (StoreType == "BLOB")
			{
				if (((DbParameter)(object)val).Direction == ParameterDirection.Input)
				{
					if (value == null || (value != null && value is byte[] && ((byte[])value).Length > 32767))
					{
						val.OracleDbType = OracleDbType.Blob;
					}
				}
				else
				{
					val.OracleDbType = OracleDbType.Blob;
				}
			}
			else if (StoreType == "BFILE")
			{
				val.OracleDbType = OracleDbType.BFile;
			}
			return (DbParameter)(object)val;
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			object value = parameter.Value;
			int? num = (value as byte[])?.Length;
			parameter.Size = ((value == null || value == DBNull.Value || (num.HasValue && num <= _maxSpecificSize)) ? _maxSpecificSize : parameter.Size);
		}

		/// <summary>
		/// 生成非空SQL字面量
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("'");
			byte[] array = (byte[])value;
			foreach (byte b in array)
			{
				builder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			builder.Append("'");
			return builder.ToString();
		}
	}
}
