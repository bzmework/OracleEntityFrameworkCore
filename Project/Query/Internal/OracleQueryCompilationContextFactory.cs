using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.Internal
{
	/// <summary>
	/// 查询编译上下文工厂
	/// </summary>
	public class OracleQueryCompilationContextFactory : RelationalQueryCompilationContextFactory
	{
		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		internal string _oracleSQLCompatibility = "12";

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">编译上下文依赖</param>
		/// <param name="relationalDependencies">关系编译上下文依赖</param>
		/// <param name="options">选项</param>
		/// <param name="logger">日志</param>
		public OracleQueryCompilationContextFactory(
			[NotNull] QueryCompilationContextDependencies dependencies,
			[NotNull] RelationalQueryCompilationContextDependencies relationalDependencies, 
			[NotNull] IOracleOptions options, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies, relationalDependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQueryCompilationContextFactory, OracleTraceFuncName.ctor);
			}

			if (options != null && options.OracleSQLCompatibility != null)
			{
				_oracleSQLCompatibility = options.OracleSQLCompatibility;
			}
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQueryCompilationContextFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 创建编译上下文
		/// </summary>
		/// <param name="async">是否异步</param>
		/// <returns></returns>
		public override QueryCompilationContext Create(bool async)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQueryCompilationContextFactory, OracleTraceFuncName.Create);
				}

				return async 
					? new OracleQueryCompilationContext(
						Dependencies, 
						new AsyncLinqOperatorProvider(), 
						new AsyncQueryMethodProvider(), 
						TrackQueryResults, 
						_oracleSQLCompatibility) 
					: new OracleQueryCompilationContext(
						Dependencies, 
						new LinqOperatorProvider(), 
						new QueryMethodProvider(),
						TrackQueryResults, 
						_oracleSQLCompatibility);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQueryCompilationContextFactory, OracleTraceFuncName.Create, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQueryCompilationContextFactory, OracleTraceFuncName.Create);
				}
			}
		}
	}
}
