using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Decimal类型映射
	/// </summary>
	public class OracleDecimalTypeMapping : DecimalTypeMapping
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		/// <param name="precision">精度</param>
		/// <param name="scale">范围</param>
		/// <param name="storeTypePostfix">存储类型后缀</param>
		public OracleDecimalTypeMapping(
			[NotNull] string storeType, 
			DbType? dbType = null, 
			int? precision = null, 
			int? scale = null, 
			StoreTypePostfix storeTypePostfix = StoreTypePostfix.None)
			: base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(decimal)), storeType, storeTypePostfix, dbType, unicode: false, null, fixedLength: false, precision, scale))
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleDecimalTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDecimalTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">值转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDecimalTypeMapping(Parameters.WithComposedConverter(converter));
		}
	}
}
