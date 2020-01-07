using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// DateTimeOffset����ӳ��
	/// </summary>
	public class OracleDateTimeOffsetTypeMapping : DateTimeOffsetTypeMapping
	{
		private static readonly MethodInfo _readMethod = typeof(OracleDataReader).GetTypeInfo().GetDeclaredMethod("GetDateTimeOffset");

		private const string DateTimeOffsetFormatConst = "TIMESTAMP '{0:yyyy-MM-dd HH:mm:ss.fff zzz}'";

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		public OracleDateTimeOffsetTypeMapping([NotNull] string storeType, [CanBeNull] DbType? dbType = null)
			: base(storeType, dbType)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleDateTimeOffsetTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleDateTimeOffsetTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleDateTimeOffsetTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���ɷǿ�SQL������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			return string.Format(CultureInfo.InvariantCulture, DateTimeOffsetFormatConst, (DateTimeOffset)Check.NotNull(value, nameof(value)));
		}

		/// <summary>
		/// ���ò���
		/// </summary>
		/// <param name="parameter">���ݿ����</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
			((OracleParameter)parameter).OracleDbType = OracleDbType.TimeStampTZ;
		}

		/// <summary>
		/// ���DataReader����
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return _readMethod;
		}
	}
}
