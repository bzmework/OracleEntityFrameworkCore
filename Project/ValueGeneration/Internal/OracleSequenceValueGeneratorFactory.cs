using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// ����ֵ���������������ھ���ѡ����һ������ֵ������
	/// </summary>
	public class OracleSequenceValueGeneratorFactory : IOracleSequenceValueGeneratorFactory
	{
		private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

		private readonly IUpdateSqlGenerator _sqlGenerator;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// ʵ��������ֵ����������
		/// </summary>
		/// <param name="rawSqlCommandBuilder">SQL�������</param>
		/// <param name="sqlGenerator">SQL������</param>
		/// <param name="logger">��־</param>
		public OracleSequenceValueGeneratorFactory(
			[NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder, 
			[NotNull] IUpdateSqlGenerator sqlGenerator,
			IDiagnosticsLogger<DbLoggerCategory.Update> logger = null)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSequenceValueGeneratorFactory, OracleTraceFuncName.ctor);
			}

			Check.NotNull(rawSqlCommandBuilder, nameof(rawSqlCommandBuilder));
			Check.NotNull(sqlGenerator, nameof(sqlGenerator));
			_rawSqlCommandBuilder = rawSqlCommandBuilder;
			_sqlGenerator = sqlGenerator;
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSequenceValueGeneratorFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="generatorState">����ֵ������״̬</param>
		/// <param name="connection">���Ӷ���</param>
		/// <returns></returns>
		public virtual ValueGenerator Create(
			IProperty property, 
			OracleSequenceValueGeneratorState generatorState, 
			IOracleConnection connection)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSequenceValueGeneratorFactory, OracleTraceFuncName.Create);
				}

				Check.NotNull(property, nameof(property));
				Check.NotNull(generatorState, nameof(generatorState));
				Check.NotNull(connection, nameof(connection));
				
				Type type = property.ClrType.UnwrapNullableType().UnwrapEnumType();
				if (type == typeof(long))
				{
					return new OracleSequenceHiLoValueGenerator<long>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(int))
				{
					return new OracleSequenceHiLoValueGenerator<int>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(short))
				{
					return new OracleSequenceHiLoValueGenerator<short>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(byte))
				{
					return new OracleSequenceHiLoValueGenerator<byte>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(char))
				{
					return new OracleSequenceHiLoValueGenerator<char>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(ulong))
				{
					return new OracleSequenceHiLoValueGenerator<ulong>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(uint))
				{
					return new OracleSequenceHiLoValueGenerator<uint>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (type == typeof(ushort))
				{
					return new OracleSequenceHiLoValueGenerator<ushort>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
				}
				if (!(type == typeof(sbyte)))
				{
					throw new ArgumentException(CoreStrings.InvalidValueGeneratorFactoryProperty("OracleSequenceValueGeneratorFactory", property.Name, property.DeclaringEntityType.DisplayName()));
				}
				return new OracleSequenceHiLoValueGenerator<sbyte>(_rawSqlCommandBuilder, _sqlGenerator, generatorState, connection);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSequenceValueGeneratorFactory, OracleTraceFuncName.Create, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSequenceValueGeneratorFactory, OracleTraceFuncName.Create);
				}
			}
		}
	}
}
