using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ����ӳ��Դ
	/// ���ڽ�ģ�������õ�����ע����ϵͳ�涨�����ݿ�����(System.Data.DbType)����ӳ�䣬���磺
	///  [Table("Modules")]
	///  public class Modules
	///  {
	///     [Column("lngModuleID", TypeName = "number", MaxLength(10))]
	///     public long ModuleID { get; set; }
	///     
	///     ...
	///  }
	///  �����е�number����ӳ���DbType.Int32
	/// </summary>
	public class OracleTypeMappingSource : RelationalTypeMappingSource
	{
		internal static string _oracleSQLCompatibility = "12";

		private readonly OracleUnboundedTypeMapping _unboundedUnicodeString = new OracleUnboundedTypeMapping("NCLOB", null, unicode: true);
		private readonly OracleUnboundedTypeMapping _unboundedAnsiString = new OracleUnboundedTypeMapping("CLOB", null, unicode: true);

		private readonly OracleBoolTypeMapping _bool = new OracleBoolTypeMapping("NUMBER(1)", DbType.Byte);

		private readonly ByteTypeMapping _sbyte = new OracleSByteTypeMapping("NUMBER(3)", DbType.Int16);
		private readonly ByteTypeMapping _byte = new ByteTypeMapping("NUMBER(3)", DbType.Byte);

		private readonly ShortTypeMapping _short = new ShortTypeMapping("NUMBER(5)", DbType.Int16);
		private readonly ShortTypeMapping _ushort = new OracleUint16TypeMapping("NUMBER(5)", DbType.Int16);

		private readonly IntTypeMapping _int = new IntTypeMapping("NUMBER(10)", DbType.Int32);
		private readonly IntTypeMapping _uint = new OracleUint32TypeMapping("NUMBER(10)", DbType.Int32);

		private readonly LongTypeMapping _long = new LongTypeMapping("NUMBER(19)", DbType.Int64);
		private readonly LongTypeMapping _ulong = new OracleUint64TypeMapping("NUMBER(20)", DbType.Int64);

		private readonly FloatTypeMapping _float = new OracleFloatTypeMapping("BINARY_FLOAT");
		private readonly DoubleTypeMapping _double = new OracleDoubleTypeMapping("BINARY_DOUBLE");

		private readonly OracleRealTypeMapping _real = new OracleRealTypeMapping("NUMBER(17, 4)", DbType.Double);
		private readonly DecimalTypeMapping _decimal = new OracleDecimalTypeMapping("DECIMAL(29, 9)", DbType.Decimal);

		private readonly OracleStringTypeMapping _fixedLengthUnicodeString;
		private readonly OracleStringTypeMapping _variableLengthUnicodeString;

		private readonly OracleStringTypeMapping _fixedLengthAnsiString;
		private readonly OracleStringTypeMapping _variableLengthAnsiString;

		private readonly OracleByteArrayTypeMapping _variableLengthBinary = new OracleByteArrayTypeMapping("BLOB", DbType.Binary);

		private readonly OracleStringTypeMapping _urowID;

		private readonly OracleByteArrayTypeMapping _rowversion = new OracleByteArrayTypeMapping("RAW(8)", DbType.Binary, 8, fixedLength: false, new ValueComparer<byte[]>((byte[] v1, byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals(v1, v2), (byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(v), (byte[] v) => (v == null) ? null : v.ToArray()));

		private readonly OracleByteArrayTypeMapping _fixedLengthBinary = new OracleByteArrayTypeMapping("RAW", DbType.Binary);

		private readonly OracleDateTimeTypeMapping _date = new OracleDateTimeTypeMapping("DATE", DbType.Date);

		private readonly OracleDateTimeTypeMapping _datetime = new OracleDateTimeTypeMapping("TIMESTAMP(7)", DbType.DateTime);

		private readonly OracleDateTimeOffsetTypeMapping _datetimeoffset = new OracleDateTimeOffsetTypeMapping("TIMESTAMP WITH TIME ZONE", DbType.DateTimeOffset);

		private readonly OracleDateTimeOffsetTypeMapping _datetimeoffset3 = new OracleDateTimeOffsetTypeMapping("TIMESTAMP(3) WITH TIME ZONE", DbType.DateTimeOffset);

		private readonly OracleDateTimeOffsetTypeMapping _datetimeoffsetlocal = new OracleDateTimeOffsetTypeMapping("TIMESTAMP WITH LOCAL TIME ZONE");

		private readonly TimeSpanTypeMapping _time = new OracleTimeSpanTypeMapping("INTERVAL DAY(8) TO SECOND(7)");

		private readonly OracleStringTypeMapping _timeYTM;

		private readonly OracleStringTypeMapping _xml;

		private readonly GuidTypeMapping _guid = new OracleGuidTypeMapping("RAW(16)", DbType.Guid, 16);

		private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

		private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;

		private readonly HashSet<string> _disallowedMappings = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"binary varying",
			"binary",
			"char varying",
			"char",
			"character varying",
			"character",
			"national char varying",
			"national character varying",
			"national character",
			"nchar",
			"nvarchar2",
			"varchar2"
		};

		private IDiagnosticsLogger<DbLoggerCategory.Model> m_oracleLogger;

		/// <summary>
		/// ʵ��������ӳ��Դ
		/// </summary>
		/// <param name="dependencies">����ӳ��Դ</param>
		/// <param name="relationalDependencies">��ϵ����ӳ��Դ</param>
		/// <param name="oracleOptions">ѡ��</param>
		/// <param name="logger">��־</param>
		public OracleTypeMappingSource(
			[NotNull] TypeMappingSourceDependencies dependencies,
			[NotNull] RelationalTypeMappingSourceDependencies relationalDependencies,
			[NotNull] IOracleOptions oracleOptions, 
			IDiagnosticsLogger<DbLoggerCategory.Model> logger = null)
			: base(dependencies, relationalDependencies)
		{
			m_oracleLogger = logger;
			if (oracleOptions != null && oracleOptions.OracleSQLCompatibility != null)
			{
				_oracleSQLCompatibility = oracleOptions.OracleSQLCompatibility;
			}

			string oracleSQLCompatibility = _oracleSQLCompatibility;
			_xml = new OracleStringTypeMapping("XML", null, oracleSQLCompatibility, unicode: true);
			_fixedLengthUnicodeString = new OracleStringTypeMapping("NCHAR", oracleSQLCompatibility: _oracleSQLCompatibility, dbType: DbType.String, unicode: true, size: null, fixedLength: true);
			
			oracleSQLCompatibility = _oracleSQLCompatibility;
			_variableLengthUnicodeString = new OracleStringTypeMapping("NVARCHAR2", null, oracleSQLCompatibility, unicode: true);
			_fixedLengthAnsiString = new OracleStringTypeMapping("CHAR", oracleSQLCompatibility: _oracleSQLCompatibility, dbType: DbType.AnsiString, unicode: false, size: null, fixedLength: true);
			_variableLengthAnsiString = new OracleStringTypeMapping("VARCHAR2", oracleSQLCompatibility: _oracleSQLCompatibility, dbType: DbType.AnsiString);
			_timeYTM = new OracleStringTypeMapping("INTERVAL YEAR(2) TO MONTH", oracleSQLCompatibility: _oracleSQLCompatibility, dbType: DbType.String);
			
			oracleSQLCompatibility = _oracleSQLCompatibility;
			_urowID = new OracleStringTypeMapping("UROWID", null, oracleSQLCompatibility);

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.ctor);
			}

			// ͨ����������ʱ����ӳ���ͨ����������ʱ(CLR)֧�ֵ��������������ݿ�֧�ֵ�����֮���ӳ��
			_clrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
			{
				{ typeof(bool), _bool },
				{ typeof(sbyte), _sbyte },
				{ typeof(byte), _byte },
				{ typeof(short), _short },
				{ typeof(ushort), _ushort },
				{ typeof(int), _int },
				{ typeof(uint), _uint },
				{ typeof(long), _long },
				{ typeof(ulong), _ulong },
				{ typeof(float), _float },
				{ typeof(double), _double },
				{ typeof(decimal), _decimal },
				{ typeof(DateTime), _datetime },
				{ typeof(DateTimeOffset), _datetimeoffset3 },
				{ typeof(TimeSpan), _time },
				{ typeof(Guid), _guid }
			};

			// �洢����ӳ���, ģ���ϵ���������ע��(TypeName)�����ݿ�֧�ֵ�����֮���ӳ��
			_storeTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
			{
				{ "number(1)", _bool },
				{ "number(3)", _byte },
				{ "number(5)", _short },
				{ "number(10)", _int },
				{ "number(19)", _long },
				{ "number(17,4)", _real },
				{ "number(26,9)", _real }, // ����
				{ "number(29,9)", _decimal },
				{ "decimal(17,4)", _real },
				{ "decimal(29,9)", _decimal },
				{ "binary_integer", _int },
				{ "binary_float", _float },
				{ "binary_double", _double },
				{ "char", _fixedLengthAnsiString },
				{ "nchar", _fixedLengthUnicodeString },
				{ "varchar2", _variableLengthAnsiString },
				{ "nvarchar2", _variableLengthUnicodeString },
				{ "long", _variableLengthAnsiString },
				{ "long raw", _variableLengthBinary },
				{ "clob", _unboundedAnsiString },
				{ "nclob", _unboundedUnicodeString },
				{ "blob", _variableLengthBinary },
				{ "bfile", _variableLengthBinary },
				{ "date", _date },
				{ "timestamp", _datetime },
				{ "timestamp(0) with time zone", _datetimeoffset },
				{ "timestamp(1) with time zone", _datetimeoffset },
				{ "timestamp(2) with time zone", _datetimeoffset },
				{ "timestamp(3) with time zone", _datetimeoffset3 },
				{ "timestamp(4) with time zone", _datetimeoffset },
				{ "timestamp(5) with time zone", _datetimeoffset },
				{ "timestamp(6) with time zone", _datetimeoffset },
				{ "timestamp(7) with time zone", _datetimeoffset },
				{ "timestamp(8) with time zone", _datetimeoffset },
				{ "timestamp(9) with time zone", _datetimeoffset },
				{ "timestamp with time zone", _datetimeoffset },
				{ "timestamp(0) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(1) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(2) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(3) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(4) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(5) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(6) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(7) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(8) with local time zone", _datetimeoffsetlocal },
				{ "timestamp(9) with local time zone", _datetimeoffsetlocal },
				{ "timestamp with local time zone", _datetimeoffsetlocal },
				{ "interval", _time },
				{ "interval day", _time },
				{ "interval year", _timeYTM },
				{ "ROWID", _fixedLengthAnsiString },
				{ "UROWID", _urowID },
				{ "XMLTYPE", _xml },
				{ "xml", _xml }
			};

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ��������ӳ�䣬��ģ���ϵ���������ע��(TypeName)ӳ���ϵͳ�涨���ݿ�����(System.Data.DbType)
		/// </summary>
		/// <param name="mappingInfo">����ӳ��</param>
		/// <returns></returns>
		protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
		{
			RelationalTypeMapping relationalTypeMapping = null;

			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping);
				}

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping, $"RelationalTypeMappingInfo:: IsUnicode:{mappingInfo.IsUnicode}, IsKeyOrIndex:{mappingInfo.IsKeyOrIndex}, IsFixedLength:{mappingInfo.IsFixedLength}, Scale:{mappingInfo.Scale}, Precision:{mappingInfo.Precision}, Size:{mappingInfo.Size}, ClrType:{mappingInfo.ClrType}, StoreTypeNameBase:{mappingInfo.StoreTypeNameBase}, StoreTypeName:{mappingInfo.StoreTypeName}, IsRowVersion:{mappingInfo.IsRowVersion}, StoreTypeNameSizeIsMax:{mappingInfo.StoreTypeNameSizeIsMax}");
				}
				
				relationalTypeMapping = FindRawMapping(mappingInfo)?.Clone(in mappingInfo);
				
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					if (relationalTypeMapping == null)
					{
						Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping, "No mapping found");
					}
					else
					{
						Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping, $"MappingName:{relationalTypeMapping.ToString()}, ClrType:{relationalTypeMapping.ClrType}, DbType:{relationalTypeMapping.DbType}, IsFixedLength:{relationalTypeMapping.IsFixedLength}, IsUnicode:{relationalTypeMapping.IsUnicode}, Size:{relationalTypeMapping.Size}, StoreType:{relationalTypeMapping.StoreType}, StoreTypePostfix:{relationalTypeMapping.StoreTypePostfix}");
					}
				}

				return relationalTypeMapping;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindMapping);
				}
			}
		}

		private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
		{
			/*
				Linq��������ӳ���㷨��������ʾ�����£�

				[Column("lngDepartmentID", TypeName = "Number(10)")]
				public long DepartmentID { get; set; }
				
			    1. ��_storeTypeMappings���ע������: Number(10) ת����DbType����A��Int32
				2. ��_clrTypeMappings��ѱ�������: long ת����DbType����B��Int64
				3. �Ա�A��B��: ����(Type)��Precision(���ݾ���)��Scale(С��λ��)�������Խ��м���ת�������ת��ʧ���׳��쳣��
				   InvalidCastException: Unable to cast object of type 'A' to type 'B'.
				
				��ˣ��ڶ���ģ��ʱ����Ӧ�����Ǳ�֤��ע������->DbType == DbType<-��������
			 */

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping);
			}

			try
			{
				Type clrType = mappingInfo.ClrType;
				string storeTypeName = mappingInfo.StoreTypeName;
				string storeTypeNameBase = mappingInfo.StoreTypeNameBase;

				// ��ģ���ֶ�ָ������������(TypeName)�Ժ���Ӵ洢����ӳ����в���
				if (storeTypeName != null)
				{
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, $"storeTypeName: {storeTypeName}, storeTypeNameBase: {storeTypeNameBase} ");
					}

					if (clrType == typeof(float) && mappingInfo.Size.HasValue && mappingInfo.Size <= 24 && (storeTypeNameBase.Equals("float", StringComparison.OrdinalIgnoreCase) || storeTypeNameBase.Equals("double precision", StringComparison.OrdinalIgnoreCase)))
					{
						return _float;
					}
					if (!string.IsNullOrEmpty(storeTypeNameBase))
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, "Checking for the right mapping using storeTypeNameBase..");
						}

						switch (storeTypeNameBase.ToLowerInvariant())
						{
						case "char":
						case "nchar":
						case "varchar2":
						case "nvarchar2":
						case "clob":
						case "nclob":
						case "rowid":
						case "urowid":
						case "long":
							return new OracleStringTypeMapping(storeTypeNameBase, DbType.String, _oracleSQLCompatibility, mappingInfo.IsUnicode != false, fixedLength: mappingInfo.IsFixedLength == true, size: mappingInfo.Size);
						
						case "number":
						{
							RelationalTypeMapping result = null;
							RelationalTypeMapping mapping = null;

							// ���ԴӴ洢����ӳ������
							if (_storeTypeMappings.TryGetValue(storeTypeName, out mapping))
							{
								if (clrType == null || mapping.ClrType == clrType) // ���������������ƥ��
								{
									result = mapping;
								}
							}

							// ����Precision(���ݾ���)��Scale(С��λ��)������������
							if (result == null && clrType != null && mappingInfo.Precision > 0)
							{
								if (mappingInfo.Scale.HasValue && mappingInfo.Scale != 0) // ���Ը���Scale(С��λ��)��������ƥ������
								{
									// float��Ӧ��oracle���ȷ�Χ��number(9, 2)
									// double��oracle���ȷ�Χ��number(17, 4)
									// decimal��oracle���ȷ�Χ��number(29, 9)

									if (mappingInfo.Precision == 1 && clrType.Name == "Boolean")
									{
										result = new OracleBoolTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Byte);
									}
									else if (mappingInfo.Precision <= 3 && clrType.Name == "Byte")
									{
										result = new ByteTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Byte);
									}
									else if (mappingInfo.Precision <= 5 && clrType.Name == "Int16")
									{
										result = new ShortTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Int16);
									}
									else if (mappingInfo.Precision <= 9 && clrType.Name == "Float")
									{
										result = new FloatTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Single);
									}
									else if (mappingInfo.Precision <= 17 && clrType.Name == "Double")
									{
										result = new DoubleTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Double);
									}
									else if (mappingInfo.Precision <= 29 && clrType.Name == "Decimal")
									{
										result = new DecimalTypeMapping($"NUMBER({mappingInfo.Precision},{mappingInfo.Scale})", DbType.Decimal);
									}
								}
								else // ���Ը���Precision(���ݾ���)��������ƥ������
								{
									if (mappingInfo.Precision == 1 && clrType.Name == "Boolean")
									{
										result = new OracleBoolTypeMapping($"NUMBER({mappingInfo.Precision})", DbType.Byte);
									}
									else if (mappingInfo.Precision <= 3 && clrType.Name == "Byte")
									{
										result = new ByteTypeMapping($"NUMBER({mappingInfo.Precision})", DbType.Byte);
									}
									else if (mappingInfo.Precision <= 5 && clrType.Name == "Int16")
									{
										result = new ShortTypeMapping($"NUMBER({mappingInfo.Precision})", DbType.Int16);
									}
									else if (mappingInfo.Precision <= 10 && clrType.Name == "Int32")
									{
										result = new IntTypeMapping($"NUMBER({mappingInfo.Precision})", DbType.Int32);
									}
									else if (mappingInfo.Precision <= 19 && clrType.Name == "Int64")
									{
										result = new LongTypeMapping($"NUMBER({mappingInfo.Precision})", DbType.Int64);
									}
								}
							}

							// δ��������ƥ�����ͣ����ԴӴ�ͨ����������ʱӳ���ȡ��һ���������͡�
							// ע��: ȡ���ļ������Ϳ��ܺ����ݿ����ֶ����Ͳ�һ�£����µĺ���������ݿ��д������ʱ����ʧ�ܡ�
							if (result == null && clrType != null)
							{ 
								if (_clrTypeMappings.TryGetValue(clrType, out mapping))
								{
									result = mapping;
								}
							}

							return result;
						}
						case "raw":
							if (mappingInfo.Size == 16)
							{
								return _guid;
							}
							return new OracleByteArrayTypeMapping($"RAW({mappingInfo.Size})", DbType.Binary, mappingInfo.Size);
						}
					}

					if (_storeTypeMappings.TryGetValue(storeTypeName, out RelationalTypeMapping value))
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, "Using storeTypeName to check the storeTypeMappings for the right mapping to use..");
						}

						return (clrType == null || value.ClrType == clrType) ? value : null;
					}

					if (_storeTypeMappings.TryGetValue(storeTypeNameBase, out value))
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, "Using storeTypeNameBase to check the storeTypeMappings for the right mapping to use..");
						}

						return (clrType == null || value.ClrType == clrType) ? value : null;
					}
				}

				// ������Ҵ洢����ӳ���_storeTypeMappings����������ͺͱ�������ʵ���Ͳ�һ�£����ͨ����������ʱӳ���_clrTypeMappings����
				if (clrType != null)
				{
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, "finding mapping for clrType..");
					}

					if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
					{
						return mapping;
					}

					if (clrType == typeof(string))
					{
						// �ַ�������������ԭ��
						bool isAnsi = mappingInfo.IsUnicode == false;
						bool isFixedLength = mappingInfo.IsFixedLength == true;
						string baseName = (isAnsi ? "" : "N") + (isFixedLength ? "CHAR" : "VARCHAR2");
						int maxSize = (!isAnsi) ? (isFixedLength ? 1000 : 2000) : (isFixedLength ? 2000 : 4000);
						StoreTypePostfix? storeTypePostfix = null;
						int? size = mappingInfo.Size ?? ((!mappingInfo.IsKeyOrIndex) ? maxSize : (isAnsi ? 900 : 450));
						if (size > maxSize)
						{
							return new OracleUnboundedTypeMapping(isAnsi ? "CLOB" : "NCLOB", null, !isAnsi);
						}
						if (!isFixedLength && mappingInfo.Size.HasValue && mappingInfo.Size == 0)
						{
							return new OracleUnboundedTypeMapping(isAnsi ? "CLOB" : "NCLOB", null, !isAnsi);
						}
						return new OracleStringTypeMapping(baseName + "(" + size + ")", isAnsi ? new DbType?(DbType.AnsiString) : null, _oracleSQLCompatibility, !isAnsi, size, isFixedLength, storeTypePostfix);
					}

					if (clrType == typeof(byte[]))
					{
						if (mappingInfo.IsRowVersion == true)
						{
							return _rowversion;
						}
						int? size = mappingInfo.Size ?? (mappingInfo.IsKeyOrIndex ? new int?(900) : null);
						StoreTypePostfix? storeTypePostfix = null;
						string storeType = "BLOB";
						if (!size.HasValue)
						{
							size = 2000;
						}
						if (!(size <= 2000))
						{
							storeTypePostfix = StoreTypePostfix.None;
						}
						else
						{
							storeType = "RAW(" + size + ")";
						}
						return new OracleByteArrayTypeMapping(storeType, DbType.Binary, size, fixedLength: false, null, storeTypePostfix);
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping);
				}
			}

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleTypeMappingSource, OracleTraceFuncName.FindRawMapping, "No mappings found");
			}
			return null;
		}
	}
}
