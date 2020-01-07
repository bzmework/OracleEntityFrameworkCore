using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.Internal
{
	/// <summary>
	/// ����Ĳ�ѯ�����������
	/// </summary>
	public class OracleCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
	{
		private struct OracleCompiledQueryCacheKey : IEquatable<OracleCompiledQueryCacheKey>
		{
			private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;

			/// <summary>
			/// ʵ��������Ĳ�ѯ�����
			/// </summary>
			/// <param name="relationalCompiledQueryCacheKey">��ѯ���������</param>
			public OracleCompiledQueryCacheKey(RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey)
			{
				_relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
			}

			/// <summary>
			/// �Ƿ����
			/// </summary>
			/// <param name="obj">OracleCompiledQueryCacheKey��ѯ���������</param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				if (obj != null && obj is OracleCompiledQueryCacheKey)
				{
					OracleCompiledQueryCacheKey other = (OracleCompiledQueryCacheKey)obj;
					return _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey);
				}
				return false;
			}

			/// <summary>
			/// ���HashCode
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return _relationalCompiledQueryCacheKey.GetHashCode();
			}

			public bool Equals(OracleCompiledQueryCacheKey other)
			{
				throw new NotImplementedException();
			}
		}

		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		/// <summary>
		/// ʵ��������Ĳ�ѯ�����������
		/// </summary>
		/// <param name="dependencies">����Ĳ�ѯ���������������</param>
		/// <param name="relationalDependencies">��������Ĳ�ѯ���������������</param>
		/// <param name="logger">��־</param>
		public OracleCompiledQueryCacheKeyGenerator(
			[NotNull] CompiledQueryCacheKeyGeneratorDependencies dependencies,
			[NotNull] RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies, relationalDependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleCompiledQueryCacheKeyGenerator, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleCompiledQueryCacheKeyGenerator, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ���ɻ����
		/// </summary>
		/// <param name="query"></param>
		/// <param name="async"></param>
		/// <returns></returns>
		public override object GenerateCacheKey(Expression query, bool async)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleCompiledQueryCacheKeyGenerator, OracleTraceFuncName.GenerateCacheKey);
				}

				return new OracleCompiledQueryCacheKey(base.GenerateCacheKeyCore(query, async));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompiledQueryCacheKeyGenerator, OracleTraceFuncName.GenerateCacheKey, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleCompiledQueryCacheKeyGenerator, OracleTraceFuncName.GenerateCacheKey);
				}
			}
		}
	}
}
