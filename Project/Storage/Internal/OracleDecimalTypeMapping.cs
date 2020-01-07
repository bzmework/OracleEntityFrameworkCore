using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Decimal����ӳ��
	/// </summary>
	public class OracleDecimalTypeMapping : DecimalTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		/// <param name="precision">����</param>
		/// <param name="scale">��Χ</param>
		/// <param name="storeTypePostfix">�洢���ͺ�׺</param>
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
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleDecimalTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDecimalTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ֵת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDecimalTypeMapping(Parameters.WithComposedConverter(converter));
		}
	}
}
