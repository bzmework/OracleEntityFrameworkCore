using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Update.Internal
{
	/// <summary>
	/// �޸������������������ھ���ѡ����һ��Oracle�޸�����������
	/// </summary>
	public class OracleModificationCommandBatchFactory : IModificationCommandBatchFactory
	{
		private readonly IRelationalCommandBuilderFactory _commandBuilderFactory;

		private readonly ISqlGenerationHelper _sqlGenerationHelper;

		private readonly IUpdateSqlGenerator _updateSqlGenerator;

		private readonly IRelationalValueBufferFactoryFactory _valueBufferFactoryFactory;

		private readonly IDbContextOptions _options;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="commandBuilderFactory">�����������</param>
		/// <param name="sqlGenerationHelper">SQL���ɰ�����</param>
		/// <param name="updateSqlGenerator">UpdateSql������</param>
		/// <param name="valueBufferFactoryFactory">ֵ���湤��</param>
		/// <param name="options">���ݿ�������ѡ��</param>
		/// <param name="logger">��־</param>
		public OracleModificationCommandBatchFactory(
			[NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
			[NotNull] ISqlGenerationHelper sqlGenerationHelper,
			[NotNull] IUpdateSqlGenerator updateSqlGenerator,
			[NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory, 
			[NotNull] IDbContextOptions options, 
			IDiagnosticsLogger<DbLoggerCategory.Update> logger = null)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatchFactory, OracleTraceFuncName.ctor);
			}

			Check.NotNull(commandBuilderFactory, nameof(commandBuilderFactory));
			Check.NotNull(sqlGenerationHelper, nameof(sqlGenerationHelper));
			Check.NotNull(updateSqlGenerator, nameof(updateSqlGenerator));
			Check.NotNull(valueBufferFactoryFactory, nameof(valueBufferFactoryFactory));
			Check.NotNull(options, nameof(options));
			_commandBuilderFactory = commandBuilderFactory;
			_sqlGenerationHelper = sqlGenerationHelper;
			_updateSqlGenerator = updateSqlGenerator;
			_valueBufferFactoryFactory = valueBufferFactoryFactory;
			_options = options;
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatchFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		/// <returns></returns>
		public virtual ModificationCommandBatch Create()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatchFactory, OracleTraceFuncName.Create);
				}

				var optionsExtension = _options.FindExtension<OracleOptionsExtension>();

				return new OracleModificationCommandBatch(
					_commandBuilderFactory, 
					_sqlGenerationHelper, 
					_updateSqlGenerator, 
					_valueBufferFactoryFactory,
					optionsExtension?.MaxBatchSize, 
					m_oracleLogger);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatchFactory, OracleTraceFuncName.Create, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatchFactory, OracleTraceFuncName.Create);
				}
			}
		}
	}
}
