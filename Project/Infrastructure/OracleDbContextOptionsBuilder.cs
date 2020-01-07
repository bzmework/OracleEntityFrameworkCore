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
	/// ���ݿ�������ѡ����� 
	/// </summary>
	public class OracleDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<OracleDbContextOptionsBuilder, OracleOptionsExtension>
	{
		private IDiagnosticsLogger<DbLoggerCategory.Infrastructure> m_oracleLogger;

		/// <summary>
		/// ʵ�������ݿ�������ѡ�����
		/// </summary>
		/// <param name="optionsBuilder">���ݿ�������ѡ�����</param>
		/// <param name="logger">��־</param>
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
		/// ʹ�ü���OracleSQL 
		/// </summary>
		/// <param name="useOracleSQLCompatibility">ʹ�ü���OracleSQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
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

				// ������д��OracleOptionsExtensionѡ����չ
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
		/// ������������Ϊʹ��Ĭ�ϵ����� <see cref="IExecutionStrategy" />
		/// </summary>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure()
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c));
		}

		/// <summary>
		/// ������������Ϊʹ��Ĭ�ϵ����� <see cref="IExecutionStrategy" />
		/// </summary>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure(int maxRetryCount)
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c, maxRetryCount));
		}

		/// <summary>
		/// ������������Ϊʹ��Ĭ�ϵ����� <see cref="IExecutionStrategy" />
		/// </summary>
		/// <param name="maxRetryCount">����������</param>
		/// <param name="maxRetryDelay">��������ӳ�</param>
		/// <param name="errorNumbersToAdd">Ӧ����Ϊ��ʱ�Ե�����SQL�����</param>
		public virtual OracleDbContextOptionsBuilder EnableRetryOnFailure(
			int maxRetryCount,
			TimeSpan maxRetryDelay,
			[CanBeNull] ICollection<int> errorNumbersToAdd)
		{
			return ExecutionStrategy(c => new OracleRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorNumbersToAdd));
		}

	}
}
