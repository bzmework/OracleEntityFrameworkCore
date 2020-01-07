using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Design.Internal
{
    /// <summary>
    /// 注解代码生成器
    /// </summary>
    public class OracleAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        private IDiagnosticsLogger<DbLoggerCategory.Model> m_oracleLogger;

        /// <summary>
        /// 实例化注解代码生成器
        /// </summary>
        /// <param name="dependencies">注解代码生成器依赖</param>
        /// <param name="oracleLogger">日志</param>
        public OracleAnnotationCodeGenerator(
            [NotNull] AnnotationCodeGeneratorDependencies dependencies,
            IDiagnosticsLogger<DbLoggerCategory.Model> oracleLogger = null)
            : base(dependencies)
        {
            if (Check.IsTraceEnabled(oracleLogger?.Logger))
            {
                Trace<DbLoggerCategory.Model>.Write(oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleAnnotationCodeGenerator, OracleTraceFuncName.ctor);
            }

            m_oracleLogger = oracleLogger;

            if (Check.IsTraceEnabled(oracleLogger?.Logger))
            {
                Trace<DbLoggerCategory.Model>.Write(oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleAnnotationCodeGenerator, OracleTraceFuncName.ctor);
            }
        }

        /// <summary>
        /// 是否已经按约定处理
        /// </summary>
        /// <param name="model"></param>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public override bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            try
            {
                if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
                {
                    Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleAnnotationCodeGenerator, OracleTraceFuncName.IsHandledByConvention);
                }

                Check.NotNull(model, nameof(model));
                Check.NotNull(annotation, nameof(annotation));
                return annotation.Name == RelationalAnnotationNames.DefaultSchema && string.Equals("SYSTEM", (string)annotation.Value);
            }
            catch (Exception ex)
            {
                if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
                {
                    Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.IsHandledByConvention, ex.ToString());
                }
                throw;
            }
            finally
            {
                if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
                {
                    Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleAnnotationCodeGenerator, OracleTraceFuncName.IsHandledByConvention);
                }
            }
        }
    }
}
