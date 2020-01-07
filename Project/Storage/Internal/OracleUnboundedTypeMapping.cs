using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Unbounded����ӳ��
	/// </summary>
	public class OracleUnboundedTypeMapping : StringTypeMapping
	{
		private const int MaxSize = int.MaxValue;

		private readonly int _maxSpecificSize;

		private readonly StoreTypePostfix? _storeTypePostfix;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		/// <param name="unicode">Unicode</param>
		/// <param name="size">��С</param>
		/// <param name="fixedLength">�̶�����</param>
		/// <param name="storeTypePostfix">�洢���ͺ�׺</param>
		public OracleUnboundedTypeMapping(
			[NotNull] string storeType,
			[CanBeNull] DbType? dbType, 
			bool unicode = false, int? size = null,
			bool fixedLength = false, 
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(string)), storeType, GetStoreTypePostfix(storeTypePostfix, unicode, size), dbType, unicode, null, fixedLength))
		{
			size = null;
			_storeTypePostfix = storeTypePostfix;
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleUnboundedTypeMapping(RelationalTypeMappingParameters parameters)
			: base(parameters)
		{
			_maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
		}

		private static StoreTypePostfix GetStoreTypePostfix(StoreTypePostfix? storeTypePostfix, bool unicode, int? size)
		{
			return StoreTypePostfix.None;
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="command">���ݿ�����</param>
		/// <param name="name">����</param>
		/// <param name="value">ֵ</param>
		/// <param name="nullable">�Ƿ�ɿ�</param>
		/// <returns></returns>
		public override DbParameter CreateParameter([NotNull] DbCommand command, [NotNull] string name, [CanBeNull] object value, bool? nullable = null)
		{
			Check.NotNull(command, nameof(command));
			OracleParameter val = (OracleParameter)(object)(OracleParameter)base.CreateParameter(command, name, value, nullable);
			if (StoreType == "CLOB")
			{
				val.OracleDbType = OracleDbType.Clob;
			}
			else if (StoreType == "NCLOB")
			{
				val.OracleDbType = OracleDbType.NClob;
			}
			return (DbParameter)(object)val;
		}

		private static int CalculateSize(bool unicode, int? size)
		{
			return 0;
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="size">��С</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleUnboundedTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size, GetStoreTypePostfix(_storeTypePostfix, IsUnicode, size)));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleUnboundedTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���ò���
		/// </summary>
		/// <param name="parameter">���ݿ����</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			_ = (parameter.Value as string)?.Length;
			try
			{
				parameter.Size = 0;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw;
			}
		}

		/// <summary>
		/// ���ɷǿ�SQL������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected override string GenerateNonNullSqlLiteral(object value)
		{
			if (!IsUnicode)
			{
				return "'" + EscapeSqlLiteral((string)value) + "'";
			}
			return "N'" + EscapeSqlLiteral((string)value) + "'";
		}
	}
}
