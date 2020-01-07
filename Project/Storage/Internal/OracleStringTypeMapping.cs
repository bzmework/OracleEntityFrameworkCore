using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// String类型映射
	/// </summary>
	public class OracleStringTypeMapping : StringTypeMapping
	{
		internal const int UnicodeMax = 2000;

		internal const int AnsiMax = 4000;

		private string _oracleSQLCompatibility = "12";
		private readonly int _maxSpecificSize;
        private readonly string _storeType;
		private readonly StoreTypePostfix? _storeTypePostfix;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		/// <param name="oracleSQLCompatibility">兼容SQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库</param>
		/// <param name="unicode">使用Unicode</param>
		/// <param name="size">大小</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="storeTypePostfix">存储类型后缀</param>
		public OracleStringTypeMapping(
			[NotNull] string storeType, 
			[CanBeNull] DbType? dbType, 
			string oracleSQLCompatibility = "12", 
			bool unicode = false, 
			int? size = null,
			bool fixedLength = false,
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(string)), storeType, GetStoreTypePostfix(storeTypePostfix, unicode, size), dbType, unicode, size, fixedLength), oracleSQLCompatibility)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
			_storeTypePostfix = storeTypePostfix;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		/// <param name="oracleSQLCompatibility">兼容SQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库</param>
		protected OracleStringTypeMapping(RelationalTypeMappingParameters parameters, string oracleSQLCompatibility = "12")
			: base(parameters)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
			_maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _storeType = parameters.StoreType;
		}

		private static StoreTypePostfix GetStoreTypePostfix(StoreTypePostfix? storeTypePostfix, bool unicode, int? size)
		{
			StoreTypePostfix? postfix = storeTypePostfix;
			if (!postfix.HasValue)
			{
				if (!unicode)
				{
					if (!size.HasValue || !(size <= UnicodeMax))
					{
						return StoreTypePostfix.None;
					}
					return StoreTypePostfix.Size;
				}
				if (!size.HasValue || !(size <= AnsiMax))
				{
					return StoreTypePostfix.None;
				}
				return StoreTypePostfix.Size;
			}
			return postfix.GetValueOrDefault();

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
			int num = 32767;
			if (_oracleSQLCompatibility != null && _oracleSQLCompatibility == "11")
			{
				num = 4000;
			}
			if (StoreType == "CLOB")
			{
				if (((DbParameter)(object)val).Direction == ParameterDirection.Input)
				{
					if (value == null || (value != null && value is string && ((string)value).Length * 4 > 32767))
					{
						val.OracleDbType = OracleDbType.Clob;
					}
					else if (value == null || (value != null && value is char[] && ((char[])value).Length * 4 > 32767))
					{
						val.OracleDbType = OracleDbType.Clob;
					}
				}
				else
				{
					val.OracleDbType = OracleDbType.Clob;
				}
			}
			if (StoreType == "XMLTYPE")
			{
				if (((DbParameter)(object)val).Direction == ParameterDirection.Input)
				{
					if (value == null || (value != null && value is string && ((string)value).Length * 4 > num))
					{
						val.OracleDbType = OracleDbType.XmlType;
					}
					else if (value == null || (value != null && value is char[] && ((char[])value).Length * 4 > num))
					{
						val.OracleDbType = OracleDbType.XmlType;
					}
				}
				else
				{
					val.OracleDbType = OracleDbType.XmlType;
				}
			}
			else if (StoreType == "NCLOB")
			{
				val.OracleDbType = OracleDbType.NClob;
			}
			return (DbParameter)(object)val;
		}

		private static int CalculateSize(bool unicode, int? size)
		{
			if (!unicode)
			{
				if (!size.HasValue || !(size <= UnicodeMax))
				{
					return UnicodeMax;
				}
				return size.Value;
			}
			if (!size.HasValue || !(size <= AnsiMax))
			{
				return AnsiMax;
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
			return new OracleStringTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size, GetStoreTypePostfix(_storeTypePostfix, IsUnicode, size)), _oracleSQLCompatibility);
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleStringTypeMapping(Parameters.WithComposedConverter(converter), _oracleSQLCompatibility);
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // _maxSpecificSize bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // 0 to avoid SQL client size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length;

			try
			{
				parameter.Size = ((value == null || value == DBNull.Value || (length.HasValue && length <= _maxSpecificSize)) ? _maxSpecificSize : 0);
                if (_storeType == "CLOB")
                {
                    ((OracleParameter)parameter).OracleDbType = OracleDbType.Clob;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
			if (string.IsNullOrEmpty((string)value))
			{
				return "NULL";
			}
			if (!IsUnicode)
			{
				return "'" + EscapeSqlLiteral((string)value) + "'";
			}
			return "N'" + EscapeSqlLiteral((string)value) + "'";
		}
	}
}
