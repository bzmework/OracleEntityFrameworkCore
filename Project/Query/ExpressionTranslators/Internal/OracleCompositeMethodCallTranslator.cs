using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	///  Oracle���Ϸ�������ת���� 
	/// </summary>
	public class OracleCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
	{
		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		private static readonly IMethodCallTranslator[] _methodCallTranslators = new IMethodCallTranslator[16]
		{
			new OracleContainsOptimizedTranslator(),
			new OracleConvertTranslator(),
			new OracleDateAddTranslator(),
			new OracleEndsWithOptimizedTranslator(),
			new OracleMathTranslator(),
			new OracleNewGuidTranslator(),
			new OracleObjectToStringTranslator(),
			new OracleStartsWithOptimizedTranslator(),
			new OracleStringIsNullOrWhiteSpaceTranslator(),
			new OracleStringReplaceTranslator(),
			new OracleStringSubstringTranslator(),
			new OracleStringToLowerTranslator(),
			new OracleStringToUpperTranslator(),
			new OracleStringTrimEndTranslator(),
			new OracleStringTrimStartTranslator(),
			new OracleStringTrimTranslator()
		};

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">Oracle���Ϸ�������ת��������</param>
		/// <param name="logger">��־</param>
		public OracleCompositeMethodCallTranslator(
			[NotNull] RelationalCompositeMethodCallTranslatorDependencies dependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleCompositeMethodCallTranslator, OracleTraceFuncName.ctor);
				}

				m_oracleLogger = logger;
				AddTranslators(_methodCallTranslators);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompositeMethodCallTranslator, OracleTraceFuncName.ctor, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleCompositeMethodCallTranslator, OracleTraceFuncName.ctor);
				}
			}
		}
	}
}
