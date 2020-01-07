using System;
using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// TimeSpan����ӳ��
	/// </summary>
	public class OracleTimeSpanTypeMapping : TimeSpanTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		public OracleTimeSpanTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleTimeSpanTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleTimeSpanTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ֵת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleTimeSpanTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���ɷǿ�SQL������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
            var ts = (TimeSpan)value;
            var milliseconds = ts.Milliseconds.ToString();
            milliseconds = milliseconds.PadLeft(4 - milliseconds.Length, '0');
            return $"INTERVAL '{ts.Days} {ts.Hours}:{ts.Minutes}:{ts.Seconds}.{milliseconds}' DAY TO SECOND";
		}
	}
}
