using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// 模型验证器
	/// </summary>
	public class OracleModelValidator : RelationalModelValidator
	{
		private IDiagnosticsLogger<DbLoggerCategory.Model.Validation> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">诊断日志</param>
		/// <param name="relationalDependencies">关系依赖</param>
		/// <param name="logger">日志</param>
		public OracleModelValidator(
			[NotNull] ModelValidatorDependencies dependencies, 
			[NotNull] RelationalModelValidatorDependencies relationalDependencies, 
			IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger = null)
			: base(dependencies, relationalDependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Model.Validation>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Model.Validation>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 验证
		/// </summary>
		/// <param name="model">模型</param>
		public override void Validate(IModel model)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model.Validation>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.Validate);
				}
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model.Validation>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.Validate, model.ToDebugString());
				}

				base.Validate(model);
				ValidateDefaultDecimalMapping(model);
				ValidateByteIdentityMapping(model);
				ValidateNonKeyValueGeneration(model);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model.Validation>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.Validate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Model.Validation>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModelValidator, OracleTraceFuncName.Validate);
				}
			}
		}

		/// <summary>
		/// 验证默认数字映射
		/// </summary>
		/// <param name="model">模型</param>
        protected virtual void ValidateDefaultDecimalMapping([NotNull] IModel model)
        {
			Check.NotNull(model, nameof(model));

			foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(decimal)
                         && p.Oracle().ColumnType == null))
            {
                Dependencies.Logger.DecimalTypeDefaultWarning(property);
            }
        }
		
		/// <summary>
		/// 验证字节标识映射
		/// </summary>
		/// <param name="model">模型</param>
        protected virtual void ValidateByteIdentityMapping([NotNull] IModel model)
        {
			Check.NotNull(model, nameof(model));

			foreach (var property in model.GetEntityTypes()
                .SelectMany(t => t.GetDeclaredProperties())
                .Where(
                    p => p.ClrType.UnwrapNullableType() == typeof(byte)
                         && p.Oracle().ValueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn))
            {
                Dependencies.Logger.ByteIdentityColumnWarning(property);
            }
        }

		/// <summary>
		/// 验证非键值生成
		/// </summary>
		/// <param name="model">模型</param>
        protected virtual void ValidateNonKeyValueGeneration([NotNull] IModel model)
        {
			foreach (var property in model.GetEntityTypes()
				.SelectMany(t => t.GetDeclaredProperties())
				.Where(
					p =>
						((OraclePropertyAnnotations)p.Oracle()).GetOracleValueGenerationStrategy(fallbackToModel: false) == OracleValueGenerationStrategy.SequenceHiLo
						&& !p.IsKey()))
			{
				throw new InvalidOperationException(
					OracleStrings.NonKeyValueGeneration(property.Name, property.DeclaringEntityType.DisplayName()));
			}
		}

		/// <summary>
		/// 验证共享列兼容性
		/// </summary>
		/// <param name="mappedTypes">映射类型</param>
		/// <param name="tableName">表名</param>
		protected override void ValidateSharedColumnsCompatibility(IReadOnlyList<IEntityType> mappedTypes, string tableName)
		{
			base.ValidateSharedColumnsCompatibility(mappedTypes, tableName);

            var identityColumns = mappedTypes.SelectMany(et => et.GetDeclaredProperties())
                .Where(p => p.Oracle().ValueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn)
                .Distinct((p1, p2) => p1.Name == p2.Name)
                .ToList();

            if (identityColumns.Count > 1)
            {
                var sb = new StringBuilder().AppendJoin(identityColumns.Select(p => "'" + p.DeclaringEntityType.DisplayName() + "." + p.Name + "'"));
                throw new InvalidOperationException(OracleStrings.MultipleIdentityColumns(sb, tableName));
			}
		}
	}
}
