using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Double����ӳ��
	/// </summary>
	public class OracleDoubleTypeMapping : DoubleTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		public OracleDoubleTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleDoubleTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDoubleTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDoubleTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���ɷǿ�SQL������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
            var literal = base.GenerateNonNullSqlLiteral(value);

            var doubleValue = (double)value;
            if (!literal.Contains("E")
                && !literal.Contains("e")
                && !double.IsNaN(doubleValue)
                && !double.IsInfinity(doubleValue))
            {
                return literal + "E0";
            }

            return literal;
		}
	}
}
