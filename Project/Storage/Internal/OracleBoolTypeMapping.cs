using System.Data;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ��������ӳ��
	/// </summary>
	public class OracleBoolTypeMapping : BoolTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		public OracleBoolTypeMapping([NotNull] string storeType, DbType? dbType = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(bool)), storeType, StoreTypePostfix.PrecisionAndScale, dbType))
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleBoolTypeMapping(RelationalTypeMappingParameters parameters)
			: base(parameters)
		{
			//
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="size">��С</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleBoolTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ֵת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleBoolTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���DataReader����
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return typeof(DbDataReader).GetTypeInfo().GetDeclaredMethod("GetBoolean");
		}

		/// <summary>
		/// ���ò���
		/// </summary>
		/// <param name="parameter">���ݿ����</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
		}

		/// <summary>
		/// ���ɷǿ�SQL������
		/// </summary>
		/// <param name="value">ֵ����</param>
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
