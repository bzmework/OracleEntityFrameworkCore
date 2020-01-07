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
	/// ģ��ע��
	/// </summary>
	public class OracleModelAnnotations : RelationalModelAnnotations, IOracleModelAnnotations, IRelationalModelAnnotations
	{
		/// <summary>
		/// Ĭ�ϵ�HiLo������
		/// </summary>
		public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";

		private IDiagnosticsLogger<DbLoggerCategory.Model> m_oracleLogger;

		/// <summary>
		/// HiLo������
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
		/// Oracleֵ���ɲ���
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
		/// ʵ����
		/// </summary>
		/// <param name="model">ģ��</param>
		/// <param name="logger">��־</param>
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
		/// ʵ����
		/// </summary>
		/// <param name="annotations">ע��</param>
		/// <param name="oracleLogger">��־</param>
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
		/// ����HiLo������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected virtual bool SetHiLoSequenceName([CanBeNull] string value)
		{
			return Annotations.SetAnnotation(OracleAnnotationNames.HiLoSequenceName, Check.NullButNotEmpty(value, nameof(value)));
		}

		/// <summary>
		/// ����ֵ���ɲ���
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		protected virtual bool SetValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
            return Annotations.SetAnnotation(OracleAnnotationNames.ValueGenerationStrategy, value);
		}
	}
}
