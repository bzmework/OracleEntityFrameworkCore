using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Infrastructure
{
	/// <summary>
	/// 数据库上下文选项创建器 
	/// </summary>
	public class OracleDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<OracleDbContextOptionsBuilder, OracleOptionsExtension>
	{
		private IDiagnosticsLogger<DbLoggerCategory.Infrastructure> m_oracleLogger;

		/// <summary>
		/// 实例化数据库上下文选项创建器
		/// </summary>
		/// <param name="optionsBuilder">数据库上下文选项创建器</param>
		/// <param name="logger">日志</param>
		public OracleDbContextOptionsBuilder(
			[NotNull] DbContextOptionsBuilder optionsBuilder, 
			IDiagnosticsLogger<DbLoggerCategory.Infrastructure> logger = null)
			: base(optionsBuilder)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDbContextOptionsBuilder, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDbContextOptionsBuilder, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 使用兼容OracleSQL 
		/// </summary>
		/// <param name="useOracleSQLCompatibility">使用兼容OracleSQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库</param>
		public virtual void UseOracleSQLCompatibility(string useOracleSQLCompatibility)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDbContextOptionsBuilder, OracleTraceFuncName.UseOracleSQLCompatibility);
				}

				if (useOracleSQLCompatibility == null)
				{
					throw new ArgumentNullException();
				}

				if (useOracleSQLCompatibility != "11" && useOracleSQLCompatibility != "12")
				{
					throw new ArgumentException();
				}

				// 将设置写入OracleOptionsExtension选项扩展
				base.WithOption((OracleOptionsExtension e) => e.WithOracleSQLCompatibility(useOracleSQLCompatibility));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDbContextOptionsBuilder, OracleTraceFuncName.UseOracleSQLCompatibility, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDbContextOptionsBuilder, OracleTraceFuncName.UseOracleSQLCompatibility);
				}
			}
		}

		/// <summary>
		/// 将上下文配置为使用默认的重试 <see cref="IExecutionStrategy" />
		/// </summary>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure()
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c));
		}

		/// <summary>
		/// 将上下文配置为使用默认的重试 <see cref="IExecutionStrategy" />
		/// </summary>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure(int maxRetryCount)
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c, maxRetryCount));
		}

		/// <summary>
		/// 将上下文配置为使用默认的重试 <see cref="IExecutionStrategy" />
		/// </summary>
		/// <param name="maxRetryCount">重试最大次数</param>
		/// <param name="maxRetryDelay">重试最大延迟</param>
		/// <param name="errorNumbersToAdd">应被视为暂时性的其他SQL错误号</param>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure(
			int maxRetryCount,
			TimeSpan maxRetryDelay,
			[CanBeNull] ICollection<int> errorNumbersToAdd)
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorNumbersToAdd));
		}

	}
}
