using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Uint16类型映射
	/// </summary>
	public class OracleUint16TypeMapping : ShortTypeMapping
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		public OracleUint16TypeMapping([NotNull] string storeType, DbType? dbType = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(ushort), new OracleUInt16Converter()), storeType, StoreTypePostfix.PrecisionAndScale, dbType))
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleUint16TypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleUint16TypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleUint16TypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 获得DataReader方法
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return typeof(DbDataReader).GetTypeInfo().GetDeclaredMethod("GetInt32");
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
			((OracleParameter)parameter).OracleDbType = OracleDbType.Int32;
		}
	}
}
