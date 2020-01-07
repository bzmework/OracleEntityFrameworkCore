using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// 模型注解
	/// </summary>
	public class OracleModelAnnotations : RelationalModelAnnotations, IOracleModelAnnotations, IRelationalModelAnnotations
	{
		/// <summary>
		/// 默认的HiLo序列名
		/// </summary>
		public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";

		private IDiagnosticsLogger<DbLoggerCategory.Model> m_oracleLogger;

		/// <summary>
		/// HiLo序列名
		/// </summary>
		public virtual string HiLoSequenceName
		{
			get
			{
				return (string)Annotations.Metadata[OracleAnnotationNames.HiLoSequenceName];
			}
			[param: CanBeNull]
			set
			{
				SetHiLoSequenceName(value);
			}
		}

		/// <summary>
		/// Oracle值生成策略
		/// </summary>
		public virtual OracleValueGenerationStrategy? ValueGenerationStrategy
		{
			get
			{
				return (OracleValueGenerationStrategy?)Annotations.Metadata[OracleAnnotationNames.ValueGenerationStrategy];
			}
			set
			{
				SetValueGenerationStrategy(value);
			}
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="model">模型</param>
		/// <param name="logger">日志</param>
		public OracleModelAnnotations(
			[NotNull] IModel model, 
			IDiagnosticsLogger<DbLoggerCategory.Model> logger = null)
			: base(model)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModelAnnotations, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModelAnnotations, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="annotations">注解</param>
		/// <param name="oracleLogger">日志</param>
		protected OracleModelAnnotations(
			[NotNull] RelationalAnnotations annotations, 
			IDiagnosticsLogger<DbLoggerCategory.Model> oracleLogger = null)
			: base(annotations)
		{
			if (Check.IsTraceEnabled(oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModelAnnotations, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = oracleLogger;

			if (Check.IsTraceEnabled(oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Model>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModelAnnotations, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 设置HiLo序列名
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected virtual bool SetHiLoSequenceName([CanBeNull] string value)
		{
			return Annotations.SetAnnotation(OracleAnnotationNames.HiLoSequenceName, Check.NullButNotEmpty(value, nameof(value)));
		}

		/// <summary>
		/// 设置值生成策略
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected virtual bool SetValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
            return Annotations.SetAnnotation(OracleAnnotationNames.ValueGenerationStrategy, value);
		}
	}
}
