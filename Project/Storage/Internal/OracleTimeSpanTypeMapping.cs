using System;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// TimeSpan类型映射
	/// </summary>
	public class OracleTimeSpanTypeMapping : TimeSpanTypeMapping
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		public OracleTimeSpanTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleTimeSpanTypeMapping(RelationalTypeMappingParameters parameters)
			: base(parameters)
		{
			//
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="size">大小</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleTimeSpanTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">值转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleTimeSpanTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 生成非空SQL字面量
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
            var ts = (TimeSpan)value;
            var milliseconds = ts.Milliseconds.ToString();
            milliseconds = milliseconds.PadLeft(4 - milliseconds.Length, '0');
            return $"INTERVAL '{ts.Days} {ts.Hours}:{ts.Minutes}:{ts.Seconds}.{milliseconds}' DAY TO SECOND";
		}
	}
}
