using System.Data;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 布尔类型映射
	/// </summary>
	public class OracleBoolTypeMapping : BoolTypeMapping
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		public OracleBoolTypeMapping([NotNull] string storeType, DbType? dbType = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(bool)), storeType, StoreTypePostfix.PrecisionAndScale, dbType))
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleBoolTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleBoolTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">值转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleBoolTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 获得DataReader方法
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return typeof(DbDataReader).GetTypeInfo().GetDeclaredMethod("GetBoolean");
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
		}

		/// <summary>
		/// 生成非空SQL字面量
		/// </summary>
		/// <param name="value">值对象</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			if ((bool)value)
			{
				return "1";
			}
			return "0";
		}
	}
}
