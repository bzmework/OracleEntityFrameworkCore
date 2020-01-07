using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// DateTime����ӳ��
	/// </summary>
	public class OracleDateTimeTypeMapping : DateTimeTypeMapping
	{
		private const string DateTimeFormatConst = "TO_DATE('{0:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')";

		/// <summary>
		/// SQL��������ʽ���ַ���
		/// </summary>
		protected override string SqlLiteralFormatString
		{
			get { return DateTimeFormatConst; }
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		public OracleDateTimeTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleDateTimeTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDateTimeTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDateTimeTypeMapping(Parameters.WithComposedConverter(converter));
		}
	}
}
