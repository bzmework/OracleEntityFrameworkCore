using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
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
	/// Guid类型映射
	/// </summary>
	public class OracleGuidTypeMapping : GuidTypeMapping
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="storeType">存储类型</param>
		/// <param name="dbType">数据库类型</param>
		/// <param name="size">大小</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="comparer">值比较器</param>
		/// <param name="storeTypePostfix">存储类型后缀</param>
		public OracleGuidTypeMapping(
			[NotNull] string storeType, 
			[CanBeNull] DbType? dbType = System.Data.DbType.Binary, 
			int? size = null,
			bool fixedLength = false,
			ValueComparer comparer = null, 
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(Guid), null, comparer), storeType, StoreTypePostfix.None, dbType, unicode: false, size, fixedLength))
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="parameters">类型映射参数</param>
		protected OracleGuidTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleGuidTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// 克隆
		/// </summary>
		/// <param name="converter">转换器</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleGuidTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// 获得DataReader方法
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return typeof(DbDataReader).GetTypeInfo().GetDeclaredMethod("GetGuid");
		}

		/// <summary>
		/// 创建参数
		/// </summary>
		/// <param name="command">数据库命令</param>
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
			OracleParameter val = new OracleParameter(name, OracleDbType.Raw, (object)16, ParameterDirection.Input);
			((DbParameter)val).Value = value;
			return (DbParameter)val;
		}

		/// <summary>
		/// 配置参数
		/// </summary>
		/// <param name="parameter">数据库参数</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
			((OracleParameter)parameter).OracleDbType = OracleDbType.Raw;
		}

		/// <summary>
		/// 生成非空Sql字面量
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			byte[] array = ((Guid)value).ToByteArray();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("'");
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			stringBuilder.Append("'");
			return stringBuilder.ToString();
		}
	}
}
