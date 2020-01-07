using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.Sql.Internal
{
	/// <summary>
	/// ��ѯSQL���������������ھ���ѡ���ĸ���ѯSQL������
	/// </summary>
	public class OracleQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
	{
		private readonly IOracleOptions _oracleOptions;

		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		/// <summary>
		/// ��ʼ��
		/// </summary>
		/// <param name="dependencies">��ѯSQL����������</param>
		/// <param name="oracleOptions">ѡ��</param>
		/// <param name="logger">��־</param>
		public OracleQuerySqlGeneratorFactory(
			[NotNull] QuerySqlGeneratorDependencies dependencies,
			[NotNull] IOracleOptions oracleOptions, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGeneratorFactory, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;
			_oracleOptions = oracleOptions;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGeneratorFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ����Ĭ��
		/// </summary>
		/// <param name="selectExpression">Select���ʽ</param>
		/// <returns></returns>
		public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGeneratorFactory, OracleTraceFuncName.CreateDefault);
				}

				return new OracleQuerySqlGenerator(Dependencies, Check.NotNull(selectExpression, nameof(selectExpression)), _oracleOptions.OracleSQLCompatibility, m_oracleLogger);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGeneratorFactory, OracleTraceFuncName.CreateDefault, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGeneratorFactory, OracleTraceFuncName.CreateDefault);
				}
			}
		}
	}
}
