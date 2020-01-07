using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Unbounded类型映射
	/// </summary>
	public class OracleUnboundedTypeMapping : StringTypeMapping
	{
		private const int MaxSize = int.MaxValue;

		private readonly int _maxSpecificSize;

		private readonly StoreTypePostfix? _storeTypePostfix;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		/// <param name="unicode">Unicode</param>
		/// <param name="size">大小</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="storeTypePostfix">存储类型后缀</param>
		public OracleUnboundedTypeMapping(
			[NotNull] string storeType,
			[CanBeNull] DbType? dbType, 
			bool unicode = false, int? size = null,
			bool fixedLength = false, 
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(string)), storeType, GetStoreTypePostfix(storeTypePostfix, unicode, size), dbType, unicode, null, fixedLength))
		{
			size = null;
			_storeTypePostfix = storeTypePostfix;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleUnboundedTypeMapping(RelationalTypeMappingParameters parameters)
			: base(parameters)
		{
			_maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
		}

		private static StoreTypePostfix GetStoreTypePostfix(StoreTypePostfix? storeTypePostfix, bool unicode, int? size)
		{
			return StoreTypePostfix.None;
		}

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="command">数据库命令</param>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <param name="nullable">是否可空</param>
		/// <returns></returns>
		public override DbParameter CreateParameter([NotNull] DbCommand command, [NotNull] string name, [CanBeNull] object value, bool? nullable = null)
		{
			Check.NotNull(command, nameof(command));
			OracleParameter val = (OracleParameter)(object)(OracleParameter)base.CreateParameter(command, name, value, nullable);
			if (StoreType == "CLOB")
			{
				val.OracleDbType = OracleDbType.Clob;
			}
			else if (StoreType == "NCLOB")
			{
				val.OracleDbType = OracleDbType.NClob;
			}
			return (DbParameter)(object)val;
		}

		private static int CalculateSize(bool unicode, int? size)
		{
			return 0;
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="size">大小</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleUnboundedTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size, GetStoreTypePostfix(_storeTypePostfix, IsUnicode, size)));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleUnboundedTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			_ = (parameter.Value as string)?.Length;
			try
			{
				parameter.Size = 0;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw;
			}
		}

		/// <summary>
		/// 生成非空SQL字面量
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			if (!IsUnicode)
			{
				return "'" + EscapeSqlLiteral((string)value) + "'";
			}
			return "N'" + EscapeSqlLiteral((string)value) + "'";
		}
	}
}
