using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Migrations.Internal
{
	/// <summary>
	/// 迁移注解提供器
	/// </summary>
	public class OracleMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
		private IDiagnosticsLogger<DbLoggerCategory.Migrations> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">迁移注解提供器依赖</param>
		/// <param name="logger">日志</param>
		public OracleMigrationsAnnotationProvider(
			[NotNull] MigrationsAnnotationProviderDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Migrations> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsAnnotationProvider, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsAnnotationProvider, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// For
		/// </summary>
		/// <param name="property">属性</param>
		/// <returns></returns>
		public override IEnumerable<IAnnotation> For(IProperty property)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsAnnotationProvider, OracleTraceFuncName.For);
				}

				if (property.Oracle().ValueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn)
				{
					yield return new Annotation(OracleAnnotationNames.ValueGenerationStrategy, OracleValueGenerationStrategy.IdentityColumn);
				}
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsAnnotationProvider, OracleTraceFuncName.For);
				}
			}
		}
	}
}
