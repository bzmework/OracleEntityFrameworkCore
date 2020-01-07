using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Real����ӳ��
	/// </summary>
	public class OracleRealTypeMapping : DecimalTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		/// <param name="precision">����</param>
		/// <param name="scale">��Χ</param>
		/// <param name="storeTypePostfix">�洢���ͺ�׺</param>
		public OracleRealTypeMapping(
			[NotNull] string storeType, 
			DbType? dbType = null, 
			int? precision = null, 
			int? scale = null, 
			StoreTypePostfix storeTypePostfix = StoreTypePostfix.None)
			: base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(double)), storeType, storeTypePostfix, dbType, unicode: false, null, fixedLength: false, precision, scale))
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleRealTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleRealTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ֵת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleRealTypeMapping(Parameters.WithComposedConverter(converter));
		}
	}
}
