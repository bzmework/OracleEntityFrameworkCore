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
	/// Guid����ӳ��
	/// </summary>
	public class OracleGuidTypeMapping : GuidTypeMapping
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		/// <param name="size">��С</param>
		/// <param name="fixedLength">�̶�����</param>
		/// <param name="comparer">ֵ�Ƚ���</param>
		/// <param name="storeTypePostfix">�洢���ͺ�׺</param>
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
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		protected OracleGuidTypeMapping(RelationalTypeMappingParameters parameters)
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
			return new OracleGuidTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size));
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleGuidTypeMapping(Parameters.WithComposedConverter(converter));
		}

		/// <summary>
		/// ���DataReader����
		/// </summary>
		/// <returns></returns>
		public override MethodInfo GetDataReaderMethod()
		{
			return typeof(DbDataReader).GetTypeInfo().GetDeclaredMethod("GetGuid");
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="command">���ݿ�����</param>
		/// <param name="name">����</param>
		/// <param name="value">ֵ</param>
		/// <param name="nullable">�Ƿ�ɿ�</param>
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
		/// ���ò���
		/// </summary>
		/// <param name="parameter">���ݿ����</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
			base.ConfigureParameter(parameter);
			((OracleParameter)parameter).OracleDbType = OracleDbType.Raw;
		}

		/// <summary>
		/// ���ɷǿ�Sql������
		/// </summary>
		/// <param name="value">ֵ</param>
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
