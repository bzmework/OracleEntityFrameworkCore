using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Metadata.Conventions.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// 约定设置创建器
	/// </summary>
	public class OracleConventionSetBuilder : RelationalConventionSetBuilder
	{
		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">依赖</param>
		/// <param name="logger">日志</param>
		public OracleConventionSetBuilder(
			[NotNull] RelationalConventionSetBuilderDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleConventionSetBuilder, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleConventionSetBuilder, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 添加约定
		/// </summary>
		/// <param name="conventionSet">约定集</param>
		/// <returns></returns>
		public override ConventionSet AddConventions(ConventionSet conventionSet)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleConventionSetBuilder, OracleTraceFuncName.AddConventions);
				}

				Check.NotNull(conventionSet, nameof(conventionSet));
				base.AddConventions(conventionSet);
				var valueGenerationStrategyConvention = new OracleValueGenerationStrategyConvention();
				conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);
				conventionSet.ModelInitializedConventions.Add(new RelationalMaxIdentifierLengthConvention(128));
				ValueGeneratorConvention valueGeneratorConvention = new OracleValueGeneratorConvention();
				ReplaceConvention(conventionSet.BaseEntityTypeChangedConventions, valueGeneratorConvention);
				ReplaceConvention(conventionSet.PrimaryKeyChangedConventions, valueGeneratorConvention);
				ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGeneratorConvention);
				ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGeneratorConvention);
				conventionSet.PropertyAnnotationChangedConventions.Add((OracleValueGeneratorConvention)valueGeneratorConvention);
				return conventionSet;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleConventionSetBuilder, OracleTraceFuncName.AddConventions, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleConventionSetBuilder, OracleTraceFuncName.AddConventions);
				}
			}
		}

		/// <summary>
		/// 创建约定集
		/// </summary>
		/// <returns></returns>
		public static ConventionSet Build()
		{
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkOracle()
                .AddDbContext<DbContext>(o => o.UseOracle("Data Source=."))
                .BuildServiceProvider();

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DbContext>())
                {
                    return ConventionSet.CreateConventionSet(context);
                }
            }
		}
	}
}
