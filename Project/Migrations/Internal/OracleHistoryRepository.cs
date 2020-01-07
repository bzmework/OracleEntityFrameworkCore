using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;
using System;
using System.Text;

namespace Oracle.EntityFrameworkCore.Migrations.Internal
{
	/// <summary>
	/// 历史仓库
	/// </summary>
	public class OracleHistoryRepository : HistoryRepository
	{
		private IDiagnosticsLogger<DbLoggerCategory.Migrations> m_oracleLogger;

		/// <summary>
		/// 存在SQL
		/// </summary>
		protected override string ExistsSql
		{
			get
			{
				try
				{
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.ExistsSql);
					}

					RelationalTypeMapping mapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
					StringBuilder builder = new StringBuilder();
					
					string str = "user_tables";
					if (TableSchema != null)
					{
						str = "all_tables";
					}

					builder
						.Append("SELECT t.table_name ")
						.Append("FROM " + str + " t ")
						.Append("WHERE t.table_name = ")
						.Append(mapping.GenerateSqlLiteral(TableName));

					if (TableSchema != null)
					{
						builder
							.Append(" AND t.owner = ")
							.Append(mapping.GenerateSqlLiteral(TableSchema));
					}

					return builder.ToString();
				}
				catch (Exception ex)
				{
					if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.ExistsSql, ex.ToString());
					}
					throw;
				}
				finally
				{
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.ExistsSql);
					}
				}
			}
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">历史仓库依赖</param>
		/// <param name="logger">日志</param>
		public OracleHistoryRepository(
			[NotNull] HistoryRepositoryDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Migrations> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 解释存在结果
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override bool InterpretExistsResult(object value)
		{
			return value != null;
		}

		/// <summary>
		/// 如果脚本不存在，则获得Create脚本
		/// </summary>
		/// <returns></returns>
		public override string GetCreateIfNotExistsScript()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetCreateIfNotExistsScript);
				}

				var builder = new IndentedStringBuilder();
				builder.AppendLine("BEGIN");
				builder.AppendLine(GetCreateScript());
				builder.AppendLine("EXCEPTION");
				builder.AppendLine("WHEN OTHERS THEN");
				builder.AppendLine("    IF(SQLCODE != -942)THEN");
				builder.AppendLine("        RAISE;");
				builder.AppendLine("    END IF;");
				builder.Append("END;");
				return builder.ToString();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetCreateIfNotExistsScript, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetCreateIfNotExistsScript);
				}
			}
		}

		/// <summary>
		/// 如果脚本不存在，则获得Begin脚本
		/// </summary>
		/// <param name="migrationId">迁移ID</param>
		/// <returns></returns>
		public override string GetBeginIfNotExistsScript(string migrationId)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfNotExistsScript);
				}

				Check.NotEmpty(migrationId, nameof(migrationId));
				RelationalTypeMapping mapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
				return new StringBuilder()
					.AppendLine("DECLARE")
					.AppendLine("    v_Count INTEGER;")
					.AppendLine("BEGIN")
					.Append("SELECT COUNT(*) INTO v_Count FROM ")
					.Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
					.Append(" WHERE ")
					.Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
					.Append(" = ")
					.AppendLine(mapping.GenerateSqlLiteral(migrationId))
					.AppendLine("IF v_Count = 0 THEN")
					.ToString();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfNotExistsScript, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfNotExistsScript);
				}
			}
		}

		/// <summary>
		/// 如果脚本不存在，则获得Begin脚本
		/// </summary>
		/// <param name="migrationId">迁移ID</param>
		/// <returns></returns>
		public override string GetBeginIfExistsScript(string migrationId)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfExistsScript);
				}

				Check.NotEmpty(migrationId, nameof(migrationId));
				RelationalTypeMapping mapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
				return new StringBuilder()
					.AppendLine("DECLARE")
					.AppendLine("    v_Count INTEGER;")
					.AppendLine("BEGIN")
					.Append("SELECT COUNT(*) INTO v_Count FROM ")
					.Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
					.Append(" WHERE ")
					.Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
					.Append(" = ")
					.AppendLine(mapping.GenerateSqlLiteral(migrationId))
					.AppendLine("IF v_Count = 1 THEN")
					.ToString();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfExistsScript, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetBeginIfExistsScript);
				}
			}
		}

		/// <summary>
		/// 如果脚本不存在，则获得End脚本
		/// </summary>
		/// <returns></returns>
		public override string GetEndIfScript()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetEndIfScript);
				}

				return new StringBuilder()
					.AppendLine(" END IF")
					.AppendLine("END")
					.ToString();
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleHistoryRepository, OracleTraceFuncName.GetEndIfScript);
				}
			}
		}
	}
}
