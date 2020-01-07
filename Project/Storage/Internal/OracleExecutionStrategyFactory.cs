using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 执行策略工厂，用于选择哪种执行策略
	/// </summary>
	public class OracleExecutionStrategyFactory : RelationalExecutionStrategyFactory
	{
		private IDiagnosticsLogger<DbLoggerCategory.Migrations> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		/// <param name="logger">日志</param>
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
		/// 创建默认执行策略
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		/// <returns></returns>
		protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
		{
			return new OracleExecutionStrategy(dependencies);
		}
	}
}
