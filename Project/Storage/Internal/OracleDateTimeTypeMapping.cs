using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// DateTime类型映射
	/// </summary>
	public class OracleDateTimeTypeMapping : DateTimeTypeMapping
	{
		private const string DateTimeFormatConst = "TO_DATE('{0:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')";

		/// <summary>
		/// SQL字面量格式化字符串
		/// </summary>
		protected override string SqlLiteralFormatString
		{
			get { return DateTimeFormatConst; }
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		public OracleDateTimeTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleDateTimeTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDateTimeTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDateTimeTypeMapping(Parameters.WithComposedConverter(converter));
		}
	}
}
