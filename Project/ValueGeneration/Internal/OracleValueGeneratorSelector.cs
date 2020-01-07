using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// ֵ������ѡ���� 
	/// </summary>
	public class OracleValueGeneratorSelector : RelationalValueGeneratorSelector
	{
		private readonly IOracleSequenceValueGeneratorFactory _sequenceFactory;

		private readonly IOracleConnection _connection;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// ����
		/// </summary>
		public new virtual IOracleValueGeneratorCache Cache
		{
			get { return (IOracleValueGeneratorCache)base.Cache; }
		}

		/// <summary>
		/// ʵ����ֵ������ѡ����
		/// </summary>
		/// <param name="dependencies">ֵ������ѡ��������</param>
		/// <param name="sequenceFactory">����ֵ����������</param>
		/// <param name="connection">���Ӷ���</param>
		/// <param name="logger">��־</param>
		public OracleValueGeneratorSelector(
			[NotNull] ValueGeneratorSelectorDependencies dependencies, 
			[NotNull] IOracleSequenceValueGeneratorFactory sequenceFactory,
			[NotNull] IOracleConnection connection, 
			IDiagnosticsLogger<DbLoggerCategory.Update> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.ctor);
			}

			Check.NotNull(sequenceFactory, nameof(sequenceFactory));
			Check.NotNull(connection, nameof(connection));
			_sequenceFactory = sequenceFactory;
			_connection = connection;
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ѡ��
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="entityType">ʵ������</param>
		/// <returns></returns>
		public override ValueGenerator Select(IProperty property, IEntityType entityType)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Select);
				}

				Check.NotNull(property, nameof(property));
				Check.NotNull(entityType, nameof(entityType));
				return (property.GetValueGeneratorFactory() == null && property.Oracle().ValueGenerationStrategy == OracleValueGenerationStrategy.SequenceHiLo) ? _sequenceFactory.Create(property, Cache.GetOrAddSequenceState(property), _connection) : base.Select(property, entityType);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Select, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Select);
				}
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="entityType">ʵ������</param>
		/// <returns></returns>
		public override ValueGenerator Create(IProperty property, IEntityType entityType)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Create);
				}

				Check.NotNull(property, nameof(property));
				Check.NotNull(entityType, nameof(entityType));
				return (!(property.ClrType.UnwrapNullableType() == typeof(Guid))) ? base.Create(property, entityType) : ((property.ValueGenerated == ValueGenerated.Never || property.Oracle().DefaultValueSql != null) ? new TemporaryGuidValueGenerator() : new GuidValueGenerator());
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Create, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleValueGeneratorSelector, OracleTraceFuncName.Create);
				}
			}
		}
	}
}
