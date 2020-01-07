using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// ���ϳ�Աת���� 
	/// </summary>
	public class OracleCompositeMemberTranslator : RelationalCompositeMemberTranslator
	{
		private IDiagnosticsLogger<DbLoggerCategory.Database> m_oracleLogger;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">Oracle���ϳ�Աת��������</param>
		/// <param name="logger">��־</param>
		public OracleCompositeMemberTranslator(
			[NotNull] RelationalCompositeMemberTranslatorDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Database> logger = null)
			: base(dependencies)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.ctor);
				}

				m_oracleLogger = logger;
				List<IMemberTranslator> translators = new List<IMemberTranslator>
				{
					new OracleStringLengthTranslator(),
					new OracleDateTimeMemberTranslator()
				};
				AddTranslators(translators);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.ctor, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.ctor);
				}
			}
		}
	}
}
