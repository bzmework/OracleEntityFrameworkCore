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
	/// 值生成器选择器 
	/// </summary>
	public class OracleValueGeneratorSelector : RelationalValueGeneratorSelector
	{
		private readonly IOracleSequenceValueGeneratorFactory _sequenceFactory;

		private readonly IOracleConnection _connection;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// 缓存
		/// </summary>
		public new virtual IOracleValueGeneratorCache Cache
		{
			get { return (IOracleValueGeneratorCache)base.Cache; }
		}

		/// <summary>
		/// 实例化值生成器选择器
		/// </summary>
		/// <param name="dependencies">值生成器选择器依赖</param>
		/// <param name="sequenceFactory">序列值生成器工厂</param>
		/// <param name="connection">连接对象</param>
		/// <param name="logger">日志</param>
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
		/// 选择
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="entityType">实体类型</param>
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
		/// 创建
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="entityType">实体类型</param>
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
