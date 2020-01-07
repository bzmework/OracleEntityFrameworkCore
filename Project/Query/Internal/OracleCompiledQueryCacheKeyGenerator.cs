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
	/// 编译的查询缓存键生成器
	/// </summary>
	public class OracleCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
	{
		private struct OracleCompiledQueryCacheKey : IEquatable<OracleCompiledQueryCacheKey>
		{
			private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;

			/// <summary>
			/// 实例化编译的查询缓存键
			/// </summary>
			/// <param name="relationalCompiledQueryCacheKey">查询缓存键对象</param>
			public OracleCompiledQueryCacheKey(RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey)
			{
				_relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
			}

			/// <summary>
			/// 是否相等
			/// </summary>
			/// <param name="obj">OracleCompiledQueryCacheKey查询缓存键对象</param>
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
			/// 获得HashCode
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
		/// 实例化编译的查询缓存键生成器
		/// </summary>
		/// <param name="dependencies">编译的查询缓存键生成器依赖</param>
		/// <param name="relationalDependencies">关联编译的查询缓存键生成器依赖</param>
		/// <param name="logger">日志</param>
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
		/// 生成缓存键
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
