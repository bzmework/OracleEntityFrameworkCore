using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// 值生成器缓存
	/// </summary>
	public class OracleValueGeneratorCache : ValueGeneratorCache, IOracleValueGeneratorCache, IValueGeneratorCache
	{
		private readonly ConcurrentDictionary<string, OracleSequenceValueGeneratorState> _sequenceGeneratorCache = new ConcurrentDictionary<string, OracleSequenceValueGeneratorState>();

		private IDiagnosticsLogger<DbLoggerCategory.Database> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">值生成器缓存依赖</param>
		/// <param name="logger">日志</param>
		public OracleValueGeneratorCache(
			[NotNull] ValueGeneratorCacheDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Database> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorCache, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleValueGeneratorCache, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 获得或增加序列状态
		/// </summary>
		/// <param name="property">属性</param>
		/// <returns></returns>
		public virtual OracleSequenceValueGeneratorState GetOrAddSequenceState(IProperty property)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorCache, OracleTraceFuncName.GetOrAddSequenceState);
				}

				Check.NotNull(property, nameof(property));
				ISequence sequence = property.Oracle().FindHiLoSequence();
				Debug.Assert(sequence != null);
				return _sequenceGeneratorCache.GetOrAdd(GetSequenceName(sequence), (string _) => new OracleSequenceValueGeneratorState(sequence));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleValueGeneratorCache, OracleTraceFuncName.GetOrAddSequenceState, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleValueGeneratorCache, OracleTraceFuncName.GetOrAddSequenceState);
				}
			}
		}

		private static string GetSequenceName(ISequence sequence)
		{
			return ((sequence.Schema == null) ? "" : (sequence.Schema + ".")) + sequence.Name;
		}
	}
}
