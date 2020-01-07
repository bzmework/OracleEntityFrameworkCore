using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Sql生成帮助器。
	/// 提供有助于生成SQL命令的服务。
	/// 此类型通常由数据库提供程序（和其他扩展程序）使用。
	/// 它通常不用于应用程序代码中。
	/// </summary>
	public class OracleSqlGenerationHelper : RelationalSqlGenerationHelper
	{
		private IDiagnosticsLogger<DbLoggerCategory.Database.Command> m_oracleLogger;

		/// <summary>
		/// 实例化Sql生成帮助器
		/// </summary>
		/// <param name="dependencies">Sql生成帮助器依赖</param>
		/// <param name="logger">日志</param>
		public OracleSqlGenerationHelper(
			[NotNull] RelationalSqlGenerationHelperDependencies dependencies,
			IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 用于批处理SQL语句的终止符 
		/// </summary>
		public override string BatchTerminator
		{
			get { return "/" + Environment.NewLine + Environment.NewLine; }
		}

		/// <summary>
		/// 为给定的候选名称生成有效的参数名。 
		/// </summary>
		/// <param name="name">参数的候选名称</param>
		/// <returns> 基于候选名称的有效名称</returns>
		public override string GenerateParameterName(string name)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}

				while (name.StartsWith("_", StringComparison.Ordinal) || Regex.IsMatch(name, "^\\d"))
				{
					name = name.Substring(1);
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}
			}
			if (Encoding.UTF8.GetByteCount(name) > 30)
			{
				name = OracleMigrationsSqlGenerator.DeriveObjectName(null, name);
			}
			return ":" + name;
		}

		/// <summary>
		/// 为给定的候选名称写入有效的参数名
		/// </summary>
		/// <param name="builder">要将生成的字符串写入的字符串创建器</param>
		/// <param name="name">参数的候选名称</param>
		public override void GenerateParameterName(StringBuilder builder, string name)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}

				builder.Append(GenerateParameterName(name));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}
			}
		}

		/// <summary>
		/// 生成标识符（列名、表名等）的分隔SQL表示形式
		/// </summary>
		/// <param name="identifier">要分隔的标识符</param>
		/// <returns>生成的字符串</returns>
		public override string DelimitIdentifier(string identifier)
		{
			// 说明：
			// Oracle的对象(表，序列等等)都是大写的(关联的需要)，不区分大小写，但加上双引号以后就大小写敏感，例如：
			// DEPARTMENT表和"Department"表被认为是两个不同的表。
			// 在进行SQL转换时加上引号无非是防止表名等对象中包含空格，这实在是没有必要，
			// 虽然对象中包含空格是数据库支持的特性，但在一个成熟的系统中都必须避免并杜绝对象中包含引号，因为这给使用和管理带来极大的不便。
			// 因此，不应该用引号将对象引起来，并且为了生成SQL的可读性，也不应该将对象转换成大写。
			//string id = Check.NotEmpty(identifier, nameof(identifier));
			//return "\"" + base.EscapeIdentifier(id.ToUpper()) + "\"";

			return base.EscapeIdentifier(identifier);
		}


		/// <summary>
		/// 写入标识符（列名、表名等）的分隔SQL表示形式。
		/// </summary>
		/// <param name="builder">要将生成的字符串写入的字符串创建器</param>
		/// <param name="identifier">要分隔的标识符</param>
		public override void DelimitIdentifier(StringBuilder builder, string identifier)
		{
			// 说明：
			// Oracle的对象(表，序列等等)都是大写的(关联的需要)，不区分大小写，但加上双引号以后就大小写敏感，例如：
			// DEPARTMENT表和"Department"表被认为是两个不同的表。
			// 在进行SQL转换时加上引号无非是防止表名等对象中包含空格，这实在是没有必要，
			// 虽然对象中包含空格是数据库支持的特性，但在一个成熟的系统中都必须避免并杜绝对象中包含引号，因为这给使用和管理带来极大的不便。
			// 因此，不应该用引号将对象引起来，并且为了生成SQL的可读性，也不应该将对象转换成大写。

			//try
			//{
			//	if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier);
			//	}

			//	Check.NotEmpty(identifier, nameof(identifier));
			//	builder.Append('"');
			//	base.EscapeIdentifier(builder, identifier.ToUpper());
			//	builder.Append('"');
			//}
			//catch (Exception ex)
			//{
			//	if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier, ex.ToString());
			//	}
			//	throw;
			//}
			//finally
			//{
			//	if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier);
			//	}
			//}

			base.EscapeIdentifier(builder, identifier);
		}
	}
}
