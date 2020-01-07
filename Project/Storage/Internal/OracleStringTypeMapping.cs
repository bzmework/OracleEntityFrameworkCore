using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// String����ӳ��
	/// </summary>
	public class OracleStringTypeMapping : StringTypeMapping
	{
		internal const int UnicodeMax = 2000;

		internal const int AnsiMax = 4000;

		private string _oracleSQLCompatibility = "12";
		private readonly int _maxSpecificSize;
        private readonly string _storeType;
		private readonly StoreTypePostfix? _storeTypePostfix;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="dbType">���ݿ�����</param>
		/// <param name="oracleSQLCompatibility">����SQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
		/// <param name="unicode">ʹ��Unicode</param>
		/// <param name="size">��С</param>
		/// <param name="fixedLength">�̶�����</param>
		/// <param name="storeTypePostfix">�洢���ͺ�׺</param>
		public OracleStringTypeMapping(
			[NotNull] string storeType, 
			[CanBeNull] DbType? dbType, 
			string oracleSQLCompatibility = "12", 
			bool unicode = false, 
			int? size = null,
			bool fixedLength = false,
			StoreTypePostfix? storeTypePostfix = null)
			: this(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(string)), storeType, GetStoreTypePostfix(storeTypePostfix, unicode, size), dbType, unicode, size, fixedLength), oracleSQLCompatibility)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
			_storeTypePostfix = storeTypePostfix;
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="parameters">����ӳ�����</param>
		/// <param name="oracleSQLCompatibility">����SQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
		protected OracleStringTypeMapping(RelationalTypeMappingParameters parameters, string oracleSQLCompatibility = "12")
			: base(parameters)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
			_maxSpecificSize = CalculateSize(parameters.Unicode, parameters.Size);
            _storeType = parameters.StoreType;
		}

		private static StoreTypePostfix GetStoreTypePostfix(StoreTypePostfix? storeTypePostfix, bool unicode, int? size)
		{
			StoreTypePostfix? postfix = storeTypePostfix;
			if (!postfix.HasValue)
			{
				if (!unicode)
				{
					if (!size.HasValue || !(size <= UnicodeMax))
					{
						return StoreTypePostfix.None;
					}
					return StoreTypePostfix.Size;
				}
				if (!size.HasValue || !(size <= AnsiMax))
				{
					return StoreTypePostfix.None;
				}
				return StoreTypePostfix.Size;
			}
			return postfix.GetValueOrDefault();

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
			int num = 32767;
			if (_oracleSQLCompatibility != null && _oracleSQLCompatibility == "11")
			{
				num = 4000;
			}
			if (StoreType == "CLOB")
			{
				if (((DbParameter)(object)val).Direction == ParameterDirection.Input)
				{
					if (value == null || (value != null && value is string && ((string)value).Length * 4 > 32767))
					{
						val.OracleDbType = OracleDbType.Clob;
					}
					else if (value == null || (value != null && value is char[] && ((char[])value).Length * 4 > 32767))
					{
						val.OracleDbType = OracleDbType.Clob;
					}
				}
				else
				{
					val.OracleDbType = OracleDbType.Clob;
				}
			}
			if (StoreType == "XMLTYPE")
			{
				if (((DbParameter)(object)val).Direction == ParameterDirection.Input)
				{
					if (value == null || (value != null && value is string && ((string)value).Length * 4 > num))
					{
						val.OracleDbType = OracleDbType.XmlType;
					}
					else if (value == null || (value != null && value is char[] && ((char[])value).Length * 4 > num))
					{
						val.OracleDbType = OracleDbType.XmlType;
					}
				}
				else
				{
					val.OracleDbType = OracleDbType.XmlType;
				}
			}
			else if (StoreType == "NCLOB")
			{
				val.OracleDbType = OracleDbType.NClob;
			}
			return (DbParameter)(object)val;
		}

		private static int CalculateSize(bool unicode, int? size)
		{
			if (!unicode)
			{
				if (!size.HasValue || !(size <= UnicodeMax))
				{
					return UnicodeMax;
				}
				return size.Value;
			}
			if (!size.HasValue || !(size <= AnsiMax))
			{
				return AnsiMax;
			}
			return size.Value;
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="storeType">�洢����</param>
		/// <param name="size">��С</param>
		/// <returns></returns>
		public override RelationalTypeMapping Clone(string storeType, int? size)
		{
			return new OracleStringTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size, GetStoreTypePostfix(_storeTypePostfix, IsUnicode, size)), _oracleSQLCompatibility);
		}

		/// <summary>
		/// ��¡
		/// </summary>
		/// <param name="converter">ת����</param>
		/// <returns></returns>
		public override CoreTypeMapping Clone(ValueConverter converter)
		{
			return new OracleStringTypeMapping(Parameters.WithComposedConverter(converter), _oracleSQLCompatibility);
		}

		/// <summary>
		/// ���ò���
		/// </summary>
		/// <param name="parameter">���ݿ����</param>
		protected override void ConfigureParameter(DbParameter parameter)
		{
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // _maxSpecificSize bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // 0 to avoid SQL client size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length;

			try
			{
				parameter.Size = ((value == null || value == DBNull.Value || (length.HasValue && length <= _maxSpecificSize)) ? _maxSpecificSize : 0);
                if (_storeType == "CLOB")
                {
                    ((OracleParameter)parameter).OracleDbType = OracleDbType.Clob;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
			if (string.IsNullOrEmpty((string)value))
			{
				return "NULL";
			}
			if (!IsUnicode)
			{
				return "'" + EscapeSqlLiteral((string)value) + "'";
			}
			return "N'" + EscapeSqlLiteral((string)value) + "'";
		}
	}
}
