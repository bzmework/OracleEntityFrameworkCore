using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Migrations.Operations;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Migrations
{
	/// <summary>
	/// 迁移SQL生成器
	/// </summary>
	public class OracleMigrationsSqlGenerator : MigrationsSqlGenerator
	{
		internal static int s_seqCount = 0;

		internal bool m_bCreatingPlSqlBlock;

		internal string _oracleSQLCompatibility = "12";

		internal IDiagnosticsLogger<DbLoggerCategory.Migrations> m_oracleLogger;

		internal static int MaxIdentifierLengthBytes;

		internal static string SequencePrefix = "SQ";

		internal static string TriggerPrefix = "TR";

		internal static string NameSeparator = "_";

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">迁移SQL生成器依赖</param>
		/// <param name="options">选项</param>
		/// <param name="logger">日志</param>
		public OracleMigrationsSqlGenerator(
			[NotNull] MigrationsSqlGeneratorDependencies dependencies, 
			[NotNull] IOracleOptions options, 
			IDiagnosticsLogger<DbLoggerCategory.Migrations> logger = null)
			: base(dependencies)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ctor);
				}

				if (options != null && options.OracleSQLCompatibility != null)
				{
					_oracleSQLCompatibility = options.OracleSQLCompatibility;
				}
				m_oracleLogger = logger;
				if (_oracleSQLCompatibility == "11")
				{
					MaxIdentifierLengthBytes = 30;
				}
				else
				{
					MaxIdentifierLengthBytes = 128;
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ctor, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ctor);
				}
			}
		}

		/// <summary>
		/// 列定义
		/// </summary>
		/// <param name="operation">增加列操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void ColumnDefinition(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "AddColumnOperation");
				}

				ColumnDefinition(
					operation.Schema, 
					operation.Table, 
					operation.Name, 
					operation.ClrType,
					operation.ColumnType,
					operation.IsUnicode, 
					operation.MaxLength,
					operation.IsFixedLength, 
					operation.IsRowVersion, 
					operation.IsNullable, 
					operation.DefaultValue, 
					operation.DefaultValueSql, 
					operation.ComputedColumnSql, 
					operation, 
					model, 
					builder);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "AddColumnOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">迁移操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "MigrationOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				OracleCreateUserOperation createUserOperation;
				OracleDropUserOperation dropUserOperation;
				if ((createUserOperation = (operation as OracleCreateUserOperation)) != null)
				{
					Generate(createUserOperation, model, builder);
				}
				else if ((dropUserOperation = (operation as OracleDropUserOperation)) != null)
				{
					Generate(dropUserOperation, model, builder);
				}
				else
				{
					base.Generate(operation, model, builder);
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "MigrationOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operations">迁移操作列表</param>
		/// <param name="model">模型</param>
		/// <returns></returns>
		public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "IReadOnlyList<MigrationOperation>");
				}

				Check.NotNull(operations, nameof(operations));
				OracleMigrationCommandListBuilder oracleMigrationCommandListBuilder = new OracleMigrationCommandListBuilder(Dependencies.CommandBuilderFactory);
				foreach (MigrationOperation operation in operations)
				{
					Generate(operation, model, oracleMigrationCommandListBuilder);
				}
				return oracleMigrationCommandListBuilder.GetCommandList();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "IReadOnlyList<MigrationOperation>");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">修改列操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "AlterColumnOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				
				var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);

				if (operation.ComputedColumnSql != null)
				{
					DropColumnOperation dropColumnOperation = new DropColumnOperation
					{
						Schema = operation.Schema,
						Table = operation.Table,
						Name = operation.Name
					};
					AddColumnOperation addColumnOperation = new AddColumnOperation
					{
						Schema = operation.Schema,
						Table = operation.Table,
						Name = operation.Name,
						ClrType = operation.ClrType,
						ColumnType = operation.ColumnType,
						IsUnicode = operation.IsUnicode,
						MaxLength = operation.MaxLength,
						IsRowVersion = operation.IsRowVersion,
						IsNullable = operation.IsNullable,
						DefaultValue = operation.DefaultValue,
						DefaultValueSql = operation.DefaultValueSql,
						ComputedColumnSql = operation.ComputedColumnSql,
						IsFixedLength = operation.IsFixedLength
					};
					addColumnOperation.AddAnnotations(operation.GetAnnotations());
					Generate(dropColumnOperation, model, builder);
					Generate(addColumnOperation, model, builder);
				}
				else
				{
					var valueGenerationStrategy = operation[OracleAnnotationNames.ValueGenerationStrategy] as OracleValueGenerationStrategy?;
					var identity = valueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn;

					if (IsOldColumnSupported(model))
					{
						var oldValueGenerationStrategy = operation.OldColumn[OracleAnnotationNames.ValueGenerationStrategy] as OracleValueGenerationStrategy?;
						var oldIdentity = oldValueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn;

						if (oldIdentity && !identity)
						{
							if (_oracleSQLCompatibility == "11")
							{
								DropIdentityForDB11(operation, builder);
							}
							else
							{
								DropIdentity(operation, builder);
							}
						}
						if (operation.OldColumn.DefaultValue != null 
							|| (operation.OldColumn.DefaultValueSql != null 
							&& (operation.DefaultValue == null || operation.DefaultValueSql == null)))
						{
							DropDefaultConstraint(operation.Schema, operation.Table, operation.Name, builder);
						}
					}
					else
					{
						if (!identity)
						{
							if (_oracleSQLCompatibility == "11")
							{
								DropIdentityForDB11(operation, builder);
							}
							else
							{
								DropIdentity(operation, builder);
							}
						}
						if (operation.DefaultValue == null && operation.DefaultValueSql == null)
						{
							DropDefaultConstraint(operation.Schema, operation.Table, operation.Name, builder);
						}
					}

					builder
						.Append("ALTER TABLE ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
						.Append(" MODIFY ");

					ColumnDefinition(
						operation.Schema,
						operation.Table, 
						operation.Name, 
						operation.ClrType, 
						operation.ColumnType, 
						operation.IsUnicode, 
						operation.MaxLength, 
						operation.IsFixedLength,
						operation.IsRowVersion,
						operation.IsNullable,
						operation.DefaultValue, 
						operation.DefaultValueSql,
						operation.ComputedColumnSql,
						identity, 
						operation,
						model, 
						builder);

					builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
					builder.EndCommand();
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "AlterColumnOperation");
				}
			}
		}

		private static void DropIdentityForDB11(
			[NotNull] AlterColumnOperation operation,
			[NotNull] MigrationCommandListBuilder builder)
		{
			Check.NotNull(operation, nameof(operation));
			Check.NotNull(builder, nameof(builder));

			string text = SequencePrefix + NameSeparator + operation.Name;
			if (Encoding.UTF8.GetByteCount(text) > MaxIdentifierLengthBytes)
			{
				text = DeriveObjectName(null, text);
			}
			string schema = operation.Schema;
			string text2 = "";
			text2 = ((schema != null)  
				? ("\nbegin\nexecute immediate\n'drop sequence " + schema + "." + text + "';\nexception\nwhen others then\nif sqlcode <> -2289 then\nraise;\nend if;\nend;") 
				: ("\nbegin\nexecute immediate\n'drop sequence " + text + "';\nexception\nwhen others then\nif sqlcode <> -2289 then\nraise;\nend if;\nend;"));
			builder.AppendLine(text2).EndCommand();
			
			string text3 = TriggerPrefix + NameSeparator + operation.Name;
			if (Encoding.UTF8.GetByteCount(text3) > MaxIdentifierLengthBytes)
			{
				text3 = DeriveObjectName(null, text3);
			}
			string text4 = "";
			text4 = ((schema != null) 
				? ("\nbegin\nexecute immediate\n'drop trigger " + schema + "." + text3 + "';\nexception\nwhen others then\nif sqlcode <> -4080 then\nraise;\nend if;\nend;") 
				: ("\nbegin\nexecute immediate\n'drop trigger " + text3 + "';\nexception\nwhen others then\nif sqlcode <> -4080 then\nraise;\nend if;\nend;"));
			builder.AppendLine(text4).EndCommand();
		}

		private static void DropIdentity(
			[NotNull] AlterColumnOperation operation,
			[NotNull] MigrationCommandListBuilder builder)
		{
			Check.NotNull(operation, nameof(operation));
			Check.NotNull(builder, nameof(builder));

			// var commandText = "\nDECLARE\n   v_Count INTEGER;\nBEGIN\n  SELECT COUNT(*) INTO v_Count\n  FROM ALL_TAB_IDENTITY_COLS T\n  WHERE T.TABLE_NAME = N'" + operation.Table + "'\n  AND T.COLUMN_NAME = '" + operation.Name + "';\n  IF v_Count > 0 THEN\n    EXECUTE IMMEDIATE 'ALTER  TABLE \"" + operation.Table + "\" MODIFY \"" + operation.Name + "\" DROP IDENTITY';\n  END IF;\nEND;";
			var commandText = $@"
                DECLARE
                   v_Count INTEGER;
                BEGIN
                  SELECT COUNT(*) INTO v_Count
                  FROM ALL_TAB_IDENTITY_COLS T
                  WHERE T.TABLE_NAME = N'{operation.Table}'
                  AND T.COLUMN_NAME = '{operation.Name}';
                  IF v_Count > 0 THEN
                    EXECUTE IMMEDIATE 'ALTER  TABLE ""{operation.Table}"" MODIFY ""{operation.Name}"" DROP IDENTITY';
                  END IF;
                END;";

			builder
				.AppendLine(commandText)
				.EndCommand();
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">重命名索引操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(
			[NotNull] RenameIndexOperation operation,
			[NotNull] IModel model, 
			MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameIndexOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				if (operation.NewName != null)
				{
					builder
						.Append("ALTER INDEX ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
						.Append(" RENAME TO ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName));
				}
				builder.EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameIndexOperation");
				}
			}
		}

		/// <summary>
		/// 序列选项
		/// </summary>
		/// <param name="schema">方案</param>
		/// <param name="name">序列名称</param>
		/// <param name="increment">增量值</param>
		/// <param name="minimumValue">最小值</param>
		/// <param name="maximumValue">最大值</param>
		/// <param name="cycle">周期</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void SequenceOptions(string schema, string name, int increment, long? minimumValue, long? maximumValue, bool cycle, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.SequenceOptions);
				}

				Check.NotEmpty(name, nameof(name));
				Check.NotNull(increment, nameof(increment));
				Check.NotNull(cycle, nameof(cycle));
				Check.NotNull(builder, nameof(builder));

				var intTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(int));
				var longTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(long));

				builder
				.Append(" INCREMENT BY ")
				.Append(intTypeMapping.GenerateSqlLiteral(increment));

				if (minimumValue != null)
				{
					builder
						.Append(" MINVALUE ")
						.Append(longTypeMapping.GenerateSqlLiteral(minimumValue));
				}
				else
				{
					builder.Append(" NOMINVALUE");
				}

				if (maximumValue != null)
				{
					builder
						.Append(" MAXVALUE ")
						.Append(longTypeMapping.GenerateSqlLiteral(maximumValue));
				}
				else
				{
					builder.Append(" NOMAXVALUE");
				}
				builder.Append(cycle ? " CYCLE" : " NOCYCLE");
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.SequenceOptions, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.SequenceOptions);
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">重命名序列操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(
			[NotNull] RenameSequenceOperation operation,
			[NotNull] IModel model, 
			MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameSequenceOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				if (operation.NewName != null && operation.NewName != operation.Name)
				{
					builder
						.Append("RENAME ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
						.Append(" TO ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
						.EndCommand();
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameSequenceOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">重命名表操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(
			[NotNull] RenameTableOperation operation,
			[NotNull] IModel model, 
			MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameTableOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				if (operation.NewName != null && operation.NewName != operation.Name)
				{
					builder
						.Append("ALTER TABLE ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
						.Append(" RENAME TO ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
						.EndCommand();
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameTableOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">删除主键操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(
			[NotNull] DropPrimaryKeyOperation operation,
			[NotNull] IModel model,
			MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropPrimaryKeyOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				base.Generate(operation, model, builder, terminate: false);
				builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropPrimaryKeyOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">确保方案操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected override void Generate(
			[NotNull] EnsureSchemaOperation operation,
			[NotNull] IModel model,
			MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "EnsureSchemaOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				builder
					.Append("select count(*) from all_users where username=")
					.Append("'")
					.Append(operation.Name)
					.Append("'")
					.EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "EnsureSchemaOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">创建用户操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected virtual void Generate(
			[NotNull] OracleCreateUserOperation operation, 
			[CanBeNull] IModel model, 
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "OracleCreateUserOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));

				builder.Append("BEGIN\n                             EXECUTE IMMEDIATE 'CREATE USER " + operation.UserName + " IDENTIFIED BY " + operation.UserName + "';\n                             EXECUTE IMMEDIATE 'GRANT DBA TO " + operation.UserName + "';\n                           END;").EndCommand(suppressTransaction: true);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "OracleCreateUserOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">删除用户操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected virtual void Generate([NotNull] OracleDropUserOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "OracleDropUserOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));

				builder.Append("BEGIN\n                         FOR v_cur IN (SELECT sid, serial# FROM v$session WHERE username = '" + operation.UserName.ToUpperInvariant() + "') LOOP\n                            EXECUTE IMMEDIATE ('ALTER SYSTEM KILL SESSION ''' || v_cur.sid || ',' || v_cur.serial# || ''' IMMEDIATE');\n                         END LOOP;\n                         EXECUTE IMMEDIATE 'DROP USER " + operation.UserName + " CASCADE';\n                       END;").EndCommand(suppressTransaction: true);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "OracleDropUserOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">删除索引操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void Generate(DropIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropIndexOperation 1");
				}

				Generate(operation, model, builder, terminate: true);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropIndexOperation 1");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">删除索引操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		/// <param name="terminate"></param>
		protected virtual void Generate([NotNull] DropIndexOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder, bool terminate)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropIndexOperation 2");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				builder.Append("DROP INDEX ").Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));
				if (terminate)
				{
					builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropIndexOperation 2");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">重命名列操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void Generate(RenameColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameColumnOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				StringBuilder stringBuilder = new StringBuilder();
				if (operation.Schema != null)
				{
					stringBuilder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Schema)).Append(".");
				}
				stringBuilder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table));
				builder
					.Append("ALTER TABLE ")
					.Append(stringBuilder)
					.Append(" RENAME COLUMN ")
					.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
					.Append(" TO ")
					.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName));
				builder.EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "RenameColumnOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">插入数据操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void Generate(InsertDataOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "InsertDataOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ModificationCommand item in operation.GenerateModificationCommands(model))
				{
					SqlGenerator.AppendInsertOperation(stringBuilder, item, 0);
				}
				builder
					.AppendLine("BEGIN")
					.Append(stringBuilder)
					.Append("END")
					.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
				builder.EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "InsertDataOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">创建序列操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void Generate([NotNull] CreateSequenceOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateSequenceOperation");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));

				builder.Append("CREATE SEQUENCE ").Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));
				RelationalTypeMapping mapping = Dependencies.TypeMappingSource.GetMapping(operation.ClrType);
				builder.Append(" START WITH ").Append(mapping.GenerateSqlLiteral(operation.StartValue));
				SequenceOptions(operation, model, builder);
				builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
				EndStatement(builder);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateSequenceOperation");
				}
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">创建表操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 1");
				}

				base.Generate(operation, model, builder);

				var rowVersionColumns = operation.Columns.Where(c => c.IsRowVersion).ToArray();

				if (rowVersionColumns.Length > 0)
				{
					builder
						.Append("CREATE OR REPLACE TRIGGER ")
						.AppendLine(Dependencies.SqlGenerationHelper.DelimitIdentifier("rowversion_" + operation.Name, operation.Schema))
						.Append("BEFORE INSERT OR UPDATE ON ")
						.AppendLine(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
						.AppendLine("FOR EACH ROW")
						.AppendLine("BEGIN");

					foreach (var rowVersionColumn in rowVersionColumns)
					{
						builder
							.Append("  :NEW.")
							.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(rowVersionColumn.Name))
							.Append(" := UTL_RAW.CAST_FROM_BINARY_INTEGER(UTL_RAW.CAST_TO_BINARY_INTEGER(NVL(:OLD.")
							.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(rowVersionColumn.Name))
							.Append(", '00000000')) + 1)")
							.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
					}

					builder
						.Append("END")
						.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
				}

				EndStatement(builder);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 1");
				}
			}
		}

		/// <summary>
		/// 列定义
		/// </summary>
		/// <param name="schema">方案</param>
		/// <param name="table">表</param>
		/// <param name="name">名称</param>
		/// <param name="clrType"></param>
		/// <param name="type">类型</param>
		/// <param name="unicode"></param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="rowVersion">行版本</param>
		/// <param name="nullable">是否可空</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="defaultValueSql">默认值SQL</param>
		/// <param name="computedColumnSql">计算列SQL</param>
		/// <param name="annotatable"></param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void ColumnDefinition(string schema, string table, string name, Type clrType, string type, bool? unicode, int? maxLength, bool? fixedLength, bool rowVersion, bool nullable, object defaultValue, string defaultValueSql, string computedColumnSql, IAnnotatable annotatable, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "ColumnDefinition 3");
				}

				var valueGenerationStrategy = annotatable[OracleAnnotationNames.ValueGenerationStrategy] as OracleValueGenerationStrategy?;

				ColumnDefinition(
					schema, 
					table, 
					name, 
					clrType, 
					type, 
					unicode, 
					maxLength, 
					fixedLength, 
					rowVersion,
					nullable,
					defaultValue,
					defaultValueSql,
					computedColumnSql,
					valueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn,
					annotatable, 
					model, 
					builder);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "ColumnDefinition 3");
				}
			}
		}

		/// <summary>
		/// 创建触发器
		/// </summary>
		/// <param name="SchemaName">方案名</param>
		/// <param name="TableName">表名</param>
		/// <param name="ColumnName">列名</param>
		/// <param name="Operation">操作</param>
		/// <param name="SequencName">序列名</param>
		/// <returns></returns>
		internal string CreateTrigger(string SchemaName, string TableName, string ColumnName, string Operation, string SequencName)
		{
			string text = TriggerPrefix + NameSeparator + TableName;
			if (Encoding.UTF8.GetByteCount(text) > MaxIdentifierLengthBytes)
			{
				text = DeriveObjectName(null, text);
			}
			StringBuilder builder = new StringBuilder();
			builder.Append("create or replace trigger ");
			builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(text, SchemaName));
			builder.AppendLine();
			builder.Append("before ");
			builder.Append(Operation);
			builder.Append(" on ");
			builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(TableName, SchemaName));
			builder.Append(" for each row ");
			builder.AppendLine();
			builder.Append("begin ");
			builder.AppendLine();
			if (Operation == "insert")
			{
				builder.Append("  if :new.");
				builder.Append("\"");
				builder.Append(ColumnName);
				builder.Append("\"");
				builder.Append(" is NULL then ");
				builder.AppendLine();
			}
			builder.Append("    select ");
			builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(SequencName, SchemaName));
			builder.Append(".nextval ");
			builder.Append("into ");
			builder.Append(":new.");
			builder.Append("\"");
			builder.Append(ColumnName);
			builder.Append("\"");
			builder.Append(" from dual;  ");
			builder.AppendLine();
			if (Operation == "insert")
			{
				builder.Append("  end if; ");
				builder.AppendLine();
			}
			builder.Append("end;");
			return builder.ToString();
		}

		/// <summary>
		/// 生成(12cDB)
		/// </summary>
		/// <param name="operation">创建表操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		/// <param name="terminate"></param>
		private void GenerateFor12cDB(
			[NotNull] CreateTableOperation operation, 
			[CanBeNull] IModel model, 
			[NotNull] MigrationCommandListBuilder builder, 
			bool terminate)
		{
			Check.NotNull(operation, nameof(operation));
			Check.NotNull(builder, nameof(builder));

			builder
				.AppendLine("EXECUTE IMMEDIATE 'CREATE TABLE ")
				.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
				.AppendLine(" (");

			using (builder.Indent())
			{
				for (int i = 0; i < operation.Columns.Count; i++)
				{
					AddColumnOperation columnOperation = operation.Columns[i];
					ColumnDefinition(columnOperation, model, builder);
					if (i != operation.Columns.Count - 1)
					{
						builder.AppendLine(",");
					}
				}
				if (operation.PrimaryKey != null)
				{
					builder.AppendLine(",");
					PrimaryKeyConstraint(operation.PrimaryKey, model, builder);
				}
				foreach (AddUniqueConstraintOperation uniqueConstraint in operation.UniqueConstraints)
				{
					builder.AppendLine(",");
					UniqueConstraint(uniqueConstraint, model, builder);
				}
				foreach (AddForeignKeyOperation foreignKey in operation.ForeignKeys)
				{
					builder.AppendLine(",");
					ForeignKeyConstraint(foreignKey, model, builder);
				}
				builder.AppendLine();
			}
			builder.Append(")';");
			builder.AppendLine();
			if (terminate)
			{
				builder.AppendLine("END;");
				EndStatement(builder);
			}
		}

		/// <summary>
		/// 生成(低于12cDB)
		/// </summary>
		/// <param name="operation">创建表操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		/// <param name="terminate"></param>
		private void GenerateForLoweThan12cDB([NotNull] CreateTableOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder, bool terminate)
		{
			Check.NotNull(operation, nameof(operation));
			Check.NotNull(builder, nameof(builder));

			builder.AppendLine("");
			List<string> list = new List<string>();
			for (int i = 0; i < operation.Columns.Count; i++)
			{
				if (OracleValueGenerationStrategy.IdentityColumn == operation.Columns[i][OracleAnnotationNames.ValueGenerationStrategy] as OracleValueGenerationStrategy?)
				{
					string tableName = operation.Name;
					string columnName = operation.Columns[i].Name;
					string schema = operation.Schema;
					string text = SequencePrefix + NameSeparator + operation.Name;
					if (Encoding.UTF8.GetByteCount(text) > MaxIdentifierLengthBytes)
					{
						text = DeriveObjectName(null, text);
					}
					builder.Append("execute immediate 'CREATE SEQUENCE ").Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(text, operation.Schema)).AppendLine(" start with 1';");
					builder.AppendLine();
					string trigger = CreateTrigger(schema, tableName, columnName, "insert", text);
					list.Add(trigger);
				}
			}

			builder
				.Append("execute immediate 'CREATE TABLE ")
				.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
				.AppendLine(" (");

			using (builder.Indent())
			{
				for (int i = 0; i < operation.Columns.Count; i++)
				{
					AddColumnOperation operation2 = operation.Columns[i];
					ColumnDefinition(operation2, model, builder);
					if (i != operation.Columns.Count - 1)
					{
						builder.AppendLine(",");
					}
				}
				if (operation.PrimaryKey != null)
				{
					builder.AppendLine(",");
					PrimaryKeyConstraint(operation.PrimaryKey, model, builder);
				}
				foreach (AddUniqueConstraintOperation uniqueConstraint in operation.UniqueConstraints)
				{
					builder.AppendLine(",");
					UniqueConstraint(uniqueConstraint, model, builder);
				}
				foreach (AddForeignKeyOperation foreignKey in operation.ForeignKeys)
				{
					builder.AppendLine(",");
					ForeignKeyConstraint(foreignKey, model, builder);
				}
				builder.AppendLine();
			}
			builder.Append(")';");
			if (terminate)
			{
				builder.AppendLine();
				foreach (string item in list)
				{
					builder.Append("execute immediate '");
					builder.Append(item);
					builder.Append("';");
					builder.AppendLine("");
				}
				builder.AppendLine("END;");
				EndStatement(builder);
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="operation">创建表操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		/// <param name="terminate"></param>
		protected override void Generate([NotNull] CreateTableOperation operation, [CanBeNull] IModel model, [NotNull] MigrationCommandListBuilder builder, bool terminate)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 2");
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));

				try
				{
					m_bCreatingPlSqlBlock = true;
					builder.AppendLine("BEGIN ");
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 2: OracleSQLCompatibility: " + _oracleSQLCompatibility);
					}
					if (string.Compare(_oracleSQLCompatibility, "12") == 0)
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 2: GenerateFor12cDB called");
						}
						GenerateFor12cDB(operation, model, builder, terminate);
					}
					else
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 2: GenerateForLoweThan12cDB called");
						}
						GenerateForLoweThan12cDB(operation, model, builder, terminate);
					}
				}
				finally
				{
					m_bCreatingPlSqlBlock = false;
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "CreateTableOperation 2");
				}
			}
		}

		/// <summary>
		/// 列定义
		/// </summary>
		/// <param name="schema">方案</param>
		/// <param name="table">表</param>
		/// <param name="name">名称</param>
		/// <param name="clrType"></param>
		/// <param name="type">类型</param>
		/// <param name="unicode"></param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="fixedLength">固定长度</param>
		/// <param name="rowVersion">行版本</param>
		/// <param name="nullable">是否可空</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="defaultValueSql">默认值SQL</param>
		/// <param name="computedColumnSql">计算列SQL</param>
		/// <param name="identity"></param>
		/// <param name="annotatable"></param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected virtual void ColumnDefinition(
			[CanBeNull] string schema, 
			[NotNull] string table,
			[NotNull] string name, 
			[NotNull] Type clrType,
			[CanBeNull] string type, 
			[CanBeNull] bool? unicode,
			[CanBeNull] int? maxLength,
			[CanBeNull] bool? fixedLength,
			bool rowVersion, 
			bool nullable,
			[CanBeNull] object defaultValue,
			[CanBeNull] string defaultValueSql,
			[CanBeNull] string computedColumnSql,
			bool identity, 
			[NotNull] IAnnotatable annotatable, 
			[CanBeNull] IModel model,
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "ColumnDefinition 2");
				}

				Check.NotEmpty(name, nameof(name));
				Check.NotNull(clrType, nameof(clrType));
				Check.NotNull(annotatable, nameof(annotatable));
				Check.NotNull(builder, nameof(builder));

				if (computedColumnSql != null)
				{
					builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name)).Append(" ");
					string columnType = GetColumnType(schema, table, name, clrType, unicode, maxLength, fixedLength, rowVersion, model);
					
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, $"Column '{name}' mapped to {columnType}");
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, $"Detailed column info: schema:{schema}, table:{table}, name:{name}, clrType:{clrType}, unicode:{unicode}, maxLength:{maxLength}, fixedLength:{fixedLength}, rowVersion:{rowVersion}");
					}

					builder.Append(type ?? columnType).Append(" AS (");
					if (m_bCreatingPlSqlBlock)
					{
						builder.Append(computedColumnSql.Replace("'", "''"));
					}
					else
					{
						builder.Append(computedColumnSql);
					}
					builder.Append(")");
				}
				else
				{
					builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name)).Append(" ");
					string columnType = GetColumnType(schema, table, name, clrType, unicode, maxLength, fixedLength, rowVersion, model);
					
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, $"Column '{name}' mapped to {columnType}");
						Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, $"Detailed column info: schema:{schema}, table:{table}, name:{name}, clrType:{clrType}, unicode:{unicode}, maxLength:{maxLength}, fixedLength:{fixedLength}, rowVersion:{rowVersion}");
					}

					builder.Append(type ?? columnType);
					if (identity)
					{
						if (string.Compare(_oracleSQLCompatibility, "12") == 0)
						{
							builder.Append(" GENERATED BY DEFAULT ON NULL AS IDENTITY");
						}
					}
					else
					{
						DefaultValue(defaultValue, defaultValueSql, builder);
					}
					if (!nullable)
					{
						builder.Append(" NOT NULL");
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ColumnDefinition, "ColumnDefinition 2");
				}
			}
		}

		/// <summary>
		/// 默认值
		/// </summary>
		/// <param name="defaultValue">默认值</param>
		/// <param name="defaultValueSql">默认值SQL</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected new virtual void DefaultValue(
			[CanBeNull] object defaultValue, 
			[CanBeNull] string defaultValueSql, 
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.DefaultValue);
				}

				Check.NotNull(builder, nameof(builder));
				if (defaultValueSql != null)
				{
					builder.Append(" DEFAULT (");
					if (m_bCreatingPlSqlBlock)
					{
						builder.Append(defaultValueSql.Replace("'", "''"));
					}
					else
					{
						builder.Append(defaultValueSql);
					}
					builder.Append(")");
				}
				else if (defaultValue != null)
				{
					object obj = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue).GenerateSqlLiteral(defaultValue);
					builder.Append(" DEFAULT ");
					if (m_bCreatingPlSqlBlock)
					{
						builder.Append(obj.ToString().Replace("'", "''"));
					}
					else
					{
						builder.Append(obj.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.DefaultValue, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.DefaultValue);
				}
			}
		}

		/// <summary>
		/// 外键约束
		/// </summary>
		/// <param name="operation">增加外键操作</param>
		/// <param name="model">模型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected override void ForeignKeyConstraint(AddForeignKeyOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ForeignKeyConstraint);
				}

				Check.NotNull(operation, nameof(operation));
				Check.NotNull(builder, nameof(builder));
				
				if (operation.Name != null)
				{
					builder
						.Append("CONSTRAINT ")
						.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
						.Append(" ");
				}
				builder
					.Append("FOREIGN KEY (")
					.Append(ColumnList(operation.Columns))
					.Append(") REFERENCES ")
					.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.PrincipalTable, operation.PrincipalSchema));
				
				if (operation.PrincipalColumns != null)
				{
					builder
						.Append(" (")
						.Append(ColumnList(operation.PrincipalColumns))
						.Append(")");
				}

				if (operation.OnUpdate != 0)
				{
					builder.Append(" ON UPDATE ");
					ForeignKeyAction(operation.OnUpdate, builder);
				}
				if (operation.OnDelete != 0 && operation.OnDelete != ReferentialAction.Restrict)
				{
					builder.Append(" ON DELETE ");
					ForeignKeyAction(operation.OnDelete, builder);
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ForeignKeyConstraint, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.ForeignKeyConstraint);
				}
			}
		}

		/// <summary>
		/// 重命名
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="newName">新的名称</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected virtual void Rename(
			[NotNull] string name, 
			[NotNull] string newName, 
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename);
				}

				Rename(name, newName, null, builder);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename);
				}
			}
		}

		/// <summary>
		/// 重命名
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="newName">新的名称</param>
		/// <param name="type">类型</param>
		/// <param name="builder">迁移命令列表创建者</param>
		protected virtual void Rename(
			[NotNull] string name, 
			[NotNull] string newName, 
			[CanBeNull] string type, 
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename);
				}

				Check.NotEmpty(name, nameof(name));
				Check.NotEmpty(newName, nameof(newName));
				Check.NotNull(builder, nameof(builder));

				var mapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
				builder
					.Append("EXEC sp_rename ")
					.Append(mapping.GenerateSqlLiteral(name))
					.Append(", ")
					.Append(mapping.GenerateSqlLiteral(newName));
				if (type != null)
				{
					builder
						.Append(", ")
						.Append(mapping.GenerateSqlLiteral(type));
				}

				builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Rename);
				}
			}
		}

		/// <summary>
		/// 删除默认约束
		/// </summary>
		/// <param name="schema">方案</param>
		/// <param name="tableName">表名</param>
		/// <param name="columnName">列名</param>
		/// <param name="builder">迁移命令列表创建器</param>
		protected virtual void DropDefaultConstraint(
			[CanBeNull] string schema, 
			[NotNull] string tableName, 
			[NotNull] string columnName, 
			[NotNull] MigrationCommandListBuilder builder)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropDefaultConstraint");
				}

				Check.NotEmpty(tableName, nameof(tableName));
				Check.NotEmpty(columnName, nameof(columnName));
				Check.NotNull(builder, nameof(builder));

				builder
					.Append("ALTER TABLE ")
					.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableName, schema))
					.Append(" MODIFY ")
					.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(columnName))
					.Append(" DEFAULT NULL")
					.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
					.EndCommand();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Migrations>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationsSqlGenerator, OracleTraceFuncName.Generate, "DropDefaultConstraint");
				}
			}
		}

		internal static string DeriveObjectName(string Prefix, string BaseName, int MaxLengthBytes = 30)
		{
			int num = 0;
			char[] array = null;
			if (Prefix != null)
			{
				num = Encoding.UTF8.GetByteCount(Prefix);
				array = Prefix.ToCharArray();
			}
			char[] array2 = null;
			string text = null;
			int num2 = 0;
			char[] array3 = null;
			if (BaseName != null)
			{
				BaseName = BaseName.Replace("\"", string.Empty).Replace('.', '_');
				Encoding.UTF8.GetByteCount(BaseName);
				array2 = BaseName.ToCharArray();
				text = Math.Abs(BaseName.GetHashCode()).ToString();
				num2 = Encoding.UTF8.GetByteCount(text);
				array3 = text.ToCharArray();
			}
			int byteCount = Encoding.UTF8.GetByteCount(NameSeparator);
			char[] array4 = NameSeparator.ToCharArray();
			int num3 = MaxLengthBytes - num - num2 - ((num > 0) ? (byteCount * 2) : byteCount);
			if (num3 < 0)
			{
				num3 = 0;
			}
			int num4 = 0;
			int num5 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			if (num > 0)
			{
				char[] array5 = array;
				for (int i = 0; i < array5.Length; i++)
				{
					char value = array5[i];
					num5 = Encoding.UTF8.GetByteCount(value.ToString());
					if (num4 + num5 > MaxLengthBytes)
					{
						break;
					}
					stringBuilder.Append(value);
					num4 += num5;
				}
				if (num4 < MaxLengthBytes)
				{
					array5 = array4;
					for (int i = 0; i < array5.Length; i++)
					{
						char value2 = array5[i];
						num5 = Encoding.UTF8.GetByteCount(value2.ToString());
						if (num4 + num5 > MaxLengthBytes)
						{
							break;
						}
						stringBuilder.Append(value2);
						num4 += num5;
					}
				}
			}
			if (num3 > 0 && num4 < MaxLengthBytes)
			{
				int num6 = 0;
				char[] array5 = array2;
				for (int i = 0; i < array5.Length; i++)
				{
					char value3 = array5[i];
					num5 = Encoding.UTF8.GetByteCount(value3.ToString());
					if (num6 + num5 > num3)
					{
						break;
					}
					stringBuilder.Append(value3);
					num4 += num5;
					num6 += num5;
				}
				if (num4 < MaxLengthBytes)
				{
					array5 = array4;
					for (int i = 0; i < array5.Length; i++)
					{
						char value4 = array5[i];
						num5 = Encoding.UTF8.GetByteCount(value4.ToString());
						if (num4 + num5 > MaxLengthBytes)
						{
							break;
						}
						stringBuilder.Append(value4);
						num4 += num5;
					}
				}
			}
			if (num2 > 0 && num4 < MaxLengthBytes)
			{
				char[] array5 = array3;
				for (int i = 0; i < array5.Length; i++)
				{
					char value5 = array5[i];
					num5 = Encoding.UTF8.GetByteCount(value5.ToString());
					if (num4 + num5 > MaxLengthBytes)
					{
						break;
					}
					stringBuilder.Append(value5);
					num4 += num5;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
