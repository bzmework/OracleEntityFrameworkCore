using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ִ�в��Թ���������ѡ������ִ�в���
	/// </summary>
	public class OracleExecutionStrategyFactory : RelationalExecutionStrategyFactory
	{
		private IDiagnosticsLogger<DbLoggerCategory.Migrations> m_oracleLogger;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		/// <param name="logger">��־</param>
		public OracleExecutionStrategyFactory(
			[NotNull] ExecutionStrategyDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Migrations> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleExecutionStrategyFactory, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleExecutionStrategyFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ����Ĭ��ִ�в���
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		/// <returns></returns>
		protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
		{
			return new OracleExecutionStrategy(dependencies);
		}
	}
}
