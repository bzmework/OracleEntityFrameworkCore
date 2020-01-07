using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Migrations.Internal;
using Oracle.EntityFrameworkCore.Migrations.Operations;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 数据库创建器
	/// </summary>
	public class OracleDatabaseCreator : RelationalDatabaseCreator
	{
		internal static string cleanSchemaPLSQL1 =  "BEGIN\n            FOR cur_rec IN(SELECT object_name, object_type\n              FROM user_objects\n              WHERE object_type IN\n              ('TABLE',\n                'VIEW',\n                'PACKAGE',\n                'PROCEDURE',\n                'FUNCTION',\n                'SYNONYM',\n                'SEQUENCE'\n              ) ";

		internal static string cleanSchemaPLSQL2 = ")\n            LOOP\n              BEGIN\n                IF cur_rec.object_type = 'TABLE'\n                  THEN\n                    EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' \"' || SYS_CONTEXT('userenv','current_schema') || '\".\"' || cur_rec.object_name || '\" CASCADE CONSTRAINTS';\n                  ELSE\n                    EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' \"' || SYS_CONTEXT('userenv','current_schema') || '\".\"' || cur_rec.object_name || '\"';\n                END IF;\n                EXCEPTION\n                  WHEN OTHERS\n                  THEN\n                  DBMS_OUTPUT.put_line('FAILED: DROP ' || cur_rec.object_type || ' \"' ||SYS_CONTEXT('userenv','current_schema') || '\".\"' || cur_rec.object_name || '\"');\n              END;\n            END LOOP;\n          END;";

		internal static string cleanSchemaPLSQL3 = "BEGIN\n            FOR cur_rec IN(SELECT object_name, object_type, owner \n              FROM all_objects\n              WHERE object_type IN\n              ('TABLE',\n                'VIEW',\n                'PACKAGE',\n                'PROCEDURE',\n                'FUNCTION',\n                'SYNONYM',\n                'SEQUENCE'\n              ) ";

		internal static string notOracleMaintained = " oracle_maintained='N' ";

		internal static string cleanSchemaPLSQL4 = ")\n            LOOP\n              BEGIN\n                IF cur_rec.object_type = 'TABLE'\n                  THEN\n                    EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' \"' || cur_rec.owner || '\".\"' || cur_rec.object_name || '\" CASCADE CONSTRAINTS';\n                  ELSE\n                    EXECUTE IMMEDIATE 'DROP ' || cur_rec.object_type || ' \"' || cur_rec.owner || '\".\"' || cur_rec.object_name || '\"';\n                END IF;\n                EXCEPTION\n                  WHEN OTHERS\n                  THEN\n                  DBMS_OUTPUT.put_line('FAILED: DROP ' || cur_rec.object_type || ' \"' || cur_rec.owner || '\".\"' || cur_rec.object_name || '\"');\n              END;\n            END LOOP;\n          END;";

		internal static string and = " AND ";

		internal static string schemaFilterSQLUser = " owner IN ({0}) ";

		internal static string[] builtInSchemas = new string[42]
		{
			"ANONYMOUS",
			"APEX_050000",
			"APEX_030200",
			"SYSMAN",
			"EXFSYS",
			"APEX_PUBLIC_USER",
			"APPQOSSYS",
			"AUDSYS",
			"CTXSYS",
			"DBSFWUSER",
			"DBSNMP",
			"DIP",
			"DVSYS",
			"DVF",
			"FLOWS_FILES",
			"GGSYS",
			"GSMADMIN_INTERNAL",
			"GSMCATUSER",
			"GSMUSER",
			"LBACSYS",
			"MDDATA",
			"MDSYS",
			"ORDPLUGINS",
			"ORDSYS",
			"ORDDATA",
			"OUTLN",
			"ORACLE_OCM",
			"REMOTE_SCHEDULER_AGENT",
			"SI_INFORMTN_SCHEMA",
			"SPATIAL_CSW_ADMIN_USR",
			"SYS",
			"SYSTEM",
			"SYSBACKUP",
			"SYSKM",
			"SYSDG",
			"SYSRAC",
			"SYS$UMF",
			"WMSYS",
			"XDB",
			"PUBLIC",
			"OJVMSYS",
			"OLAPSYS"
		};

		internal static string schemaFilterSQLInternal = " owner NOT IN ({0}) ";

		internal string _oracleSQLCompatibility = "12";

		private readonly IOracleConnection _connection;

		private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

		private IDiagnosticsLogger<DbLoggerCategory.Database> m_oracleLogger;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">数据库创建器依赖</param>
		/// <param name="connection">连接</param>
		/// <param name="rawSqlCommandBuilder">命令创建器</param>
		/// <param name="options">选项</param>
		/// <param name="logger">日志</param>
		public OracleDatabaseCreator(
			[NotNull] RelationalDatabaseCreatorDependencies dependencies, 
			[NotNull] IOracleConnection connection,
			[NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder,
			[NotNull] IOracleOptions options, 
			IDiagnosticsLogger<DbLoggerCategory.Database> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.ctor);
			}

			if (options != null && options.OracleSQLCompatibility != null)
			{
				_oracleSQLCompatibility = options.OracleSQLCompatibility;
			}
			_connection = connection;
			_rawSqlCommandBuilder = rawSqlCommandBuilder;
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 创建
		/// </summary>
		public override void Create()
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Create);
			}

			if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Create, "Required user does not exists or invalid username/password provided");
			}

			throw new NotSupportedException("Required user does not exists or invalid username/password provided");
		}

		/// <summary>
		/// 创建表
		/// </summary>
		public override void CreateTables()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.CreateTables);
				}

				IReadOnlyList<MigrationCommand> createTablesCommands = GetCreateTablesCommands();
				List<MigrationCommand> list = new List<MigrationCommand>();
				List<MigrationCommand> list2 = new List<MigrationCommand>();
				foreach (MigrationCommand item in createTablesCommands)
				{
					if (item.CommandText.StartsWith("select count(*) from all_users where username="))
					{
						list.Add(item);
					}
					else
					{
						list2.Add(item);
					}
				}
				if (list.Count != 0 && Convert.ToInt32(((OracleMigrationCommandExecutor)Dependencies.MigrationCommandExecutor).ExecuteScalar(list, Dependencies.Connection, m_oracleLogger)) == 0)
				{
					if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.CreateTables, "Schema does not exist");
					}
					throw new Exception("Schema does not exist");
				}
				((OracleMigrationCommandExecutor)Dependencies.MigrationCommandExecutor).ExecuteNonQuery(list2, Dependencies.Connection);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.CreateTables, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.CreateTables, "CreateTableOperation 2");
				}
			}
		}

		/// <summary>
		/// 检查是否有表
		/// </summary>
		/// <returns></returns>
		protected override bool HasTables()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTables);
				}

				bool flag = Dependencies.ExecutionStrategyFactory.Create().Execute(_connection, (IOracleConnection connection) => Convert.ToInt32(CreateHasTablesCommand().ExecuteScalar(connection)) > 0);

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, $"Tables exist for user {val.UserID} : {flag}");
				}
				return flag;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTables, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTables);
				}
			}
		}

		/// <summary>
		/// 异步检查是否有表
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsync);
				}

				Task<bool> task = Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(_connection, async (IOracleConnection connection, CancellationToken ct) => Convert.ToInt32(await CreateHasTablesCommand().ExecuteScalarAsync(connection, ct)) > 0, cancellationToken);

				if (Check.IsInformationEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Information, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsync, $"Tables exist for user {val.UserID} : {task.Result}");
				}

				return task;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsync, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsync);
				}
			}
		}

		/// <summary>
		/// 通过方案选项异步检查是否有表
		/// </summary>
		/// <param name="schemas"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<bool> HasTablesAsyncWithSchemaOption(string[] schemas, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsyncWithSchemaOption);
				}

				cancellationToken.ThrowIfCancellationRequested();
				return Task.FromResult(HasTablesWithSchemaOption(schemas));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsyncWithSchemaOption, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesAsyncWithSchemaOption);
				}
			}
		}

		private IRelationalCommand CreateHasTablesCommand()
		{
			return _rawSqlCommandBuilder.Build("SELECT COUNT(*) FROM user_tables where sys_context('userenv', 'current_schema') NOT in (" + string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'")) + ")");
		}

		private IRelationalCommand CreateHasAllTablesCommand(string filter)
		{
			return _rawSqlCommandBuilder.Build("SELECT COUNT(*) FROM all_tables WHERE " + string.Format(schemaFilterSQLUser, filter) + and + string.Format(schemaFilterSQLInternal, string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))));
		}

		private IEnumerable<MigrationCommand> CreateCreateOperations()
		{
			OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
			return Dependencies.MigrationsSqlGenerator.Generate(new OracleCreateUserOperation[1]
			{
				new OracleCreateUserOperation
				{
					UserName = val.UserID
				}
			});
		}

		/// <summary>
		/// 通过方案选项删除
		/// </summary>
		/// <param name="schemas"></param>
		public void DeleteWithSchemaOption(string[] schemas)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteWithSchemaOption);
				}

				string schemaFilter = GenerateSchemaFilter(schemas);
				DeleteObjects(schemaFilter);
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteWithSchemaOption, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteWithSchemaOption, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteWithSchemaOption, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteWithSchemaOption);
				}
			}
		}

		/// <summary>
		/// 通过方案选项检查是否有表
		/// </summary>
		/// <param name="schemas"></param>
		/// <returns></returns>
		public bool HasTablesWithSchemaOption(string[] schemas)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption);
				}

				bool flag = false;
				string filter = GenerateSchemaFilter(schemas);
				flag = ((!(filter != "*")) ? HasTables() : Dependencies.ExecutionStrategyFactory.Create().Execute(_connection, (IOracleConnection connection) => Convert.ToInt32(CreateHasAllTablesCommand(filter).ExecuteScalar(connection)) > 0));
				
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption, $"Tables exist for user {val.UserID} : {flag}");
				}

				return flag;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.HasTablesWithSchemaOption);
				}
			}
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		/// <returns></returns>
		public override bool Exists()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists);
				}

				OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, $"user: {val.UserID}, data source: {val.DataSource}");
				}

				_connection.Open(errorsExpected: true);

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, $"user '{val.UserID}'exists");
				}

				_connection.Close();
				return true;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, err.Message);
				}
				if (1017 != err.Number)
				{
					throw;
				}
				return false;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists);
				}
			}
		}

		/// <summary>
		/// 异步检查是否存在
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.ExistsAsync);
				}

				await _connection.OpenAsync(cancellationToken, errorsExpected: true);
				_connection.Close();
				return true;
			}
			catch (OracleException ex)
			{
				OracleException err = (OracleException)(object)ex;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.ExistsAsync, "OracleException.Number: " + err.Number);
				}
				if (1017 == err.Number)
				{
					return false;
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.ExistsAsync);
				}
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		public override void Delete()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete);
				}

				_connection.Open();
				DbCommand dbCommand = _connection.DbConnection.CreateCommand();
				if (_oracleSQLCompatibility == "11")
				{
					dbCommand.CommandText = cleanSchemaPLSQL1 + and + string.Format(" sys_context('userenv', 'current_schema') NOT IN ({0}) ", string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + cleanSchemaPLSQL2;
				}
				else
				{
					dbCommand.CommandText = cleanSchemaPLSQL1 + and + string.Format(" sys_context('userenv', 'current_schema') NOT IN ({0}) ", string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + and + notOracleMaintained + cleanSchemaPLSQL2;
				}

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete, dbCommand.CommandText);
				}

				dbCommand.ExecuteNonQuery();

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Exists, $"all objects deleted for user '{val.UserID}'");
				}

				OracleMigrationsSqlGenerator.s_seqCount = 0;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete, ex.ToString());
				}
				throw;
			}
			finally
			{
				_connection.Close();
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete);
				}
			}
		}

		/// <summary>
		/// 删除对象
		/// </summary>
		/// <param name="schemaFilter"></param>
		public void DeleteObjects(string schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects);
				}

				_connection.Open();
				DbCommand dbCommand = _connection.DbConnection.CreateCommand();
				if (_oracleSQLCompatibility == "11")
				{
					dbCommand.CommandText = cleanSchemaPLSQL3 + and + (schemaFilter.Equals("*") ? string.Empty : string.Format(schemaFilterSQLUser, schemaFilter)) + and + string.Format(schemaFilterSQLInternal, string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + cleanSchemaPLSQL4;
				}
				else
				{
					dbCommand.CommandText = cleanSchemaPLSQL3 + and + (schemaFilter.Equals("*") ? string.Empty : string.Format(schemaFilterSQLUser, schemaFilter)) + and + string.Format(schemaFilterSQLInternal, string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + and + notOracleMaintained + cleanSchemaPLSQL4;
				}

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects, dbCommand.CommandText);
				}

				dbCommand.ExecuteNonQuery();

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects, $"all objects that user has access to deleted for user '{val.UserID}'");
				}

				OracleMigrationsSqlGenerator.s_seqCount = 0;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects, ex.ToString());
				}
				throw;
			}
			finally
			{
				_connection.Close();
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjects);
				}
			}
		}

		/// <summary>
		/// 异步删除对象
		/// </summary>
		/// <param name="schemaFilter"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task DeleteObjectsAsync(string schemaFilter, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync);
				}

				_connection.Open();
				DbCommand dbCommand = _connection.DbConnection.CreateCommand();
				if (_oracleSQLCompatibility == "11")
				{
					dbCommand.CommandText = cleanSchemaPLSQL3 + and + (schemaFilter.Equals("*") ? string.Empty : string.Format(schemaFilterSQLUser, schemaFilter)) + and + string.Format(schemaFilterSQLInternal, string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + cleanSchemaPLSQL4;
				}
				else
				{
					dbCommand.CommandText = cleanSchemaPLSQL3 + and + (schemaFilter.Equals("*") ? string.Empty : string.Format(schemaFilterSQLUser, schemaFilter)) + and + string.Format(schemaFilterSQLInternal, string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + and + notOracleMaintained + cleanSchemaPLSQL4;
				}

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync, dbCommand.CommandText);
				}

				await dbCommand.ExecuteNonQueryAsync(cancellationToken);

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync, $"all objects that user has access to deleted for user '{val.UserID}'");
				}

				OracleMigrationsSqlGenerator.s_seqCount = 0;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync, ex.ToString());
				}
				throw;
			}
			finally
			{
				_connection.Close();
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteObjectsAsync);
				}
			}
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsync);
				}

				_connection.Open();
				DbCommand dbCommand = _connection.DbConnection.CreateCommand();
				if (_oracleSQLCompatibility == "11")
				{
					dbCommand.CommandText = cleanSchemaPLSQL1 + and + string.Format(" sys_context('userenv', 'current_schema') NOT IN ({0}) ", string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + cleanSchemaPLSQL2;
				}
				else
				{
					dbCommand.CommandText = cleanSchemaPLSQL1 + and + string.Format(" sys_context('userenv', 'current_schema') NOT IN ({0}) ", string.Join(", ", builtInSchemas.Select((string x) => "'" + x + "'"))) + and + notOracleMaintained + cleanSchemaPLSQL2;
				}
				await dbCommand.ExecuteNonQueryAsync(cancellationToken);

				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					OracleConnectionStringBuilder val = (OracleConnectionStringBuilder)(object)new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString);
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Connection, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsync, $"all objects deleted for user '{val.UserID}'");
				}

				OracleMigrationsSqlGenerator.s_seqCount = 0;
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.Delete, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsync, err.Message);
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsync, ex.ToString());
				}
				throw;
			}
			finally
			{
				_connection.Close();
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsync);
				}
			}
		}

		/// <summary>
		/// 通过方案选项异步删除
		/// </summary>
		/// <param name="schemas"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task DeleteAsyncWithSchemaOption(string[] schemas, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsyncWithSchemaOption);
				}

				string schemaFilter = GenerateSchemaFilter(schemas);
				await DeleteObjectsAsync(schemaFilter, cancellationToken);
			}
			catch (OracleException oraEx)
			{
				OracleException err = (OracleException)(object)oraEx;
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsyncWithSchemaOption, "OracleException.Number: " + err.Number);
				}
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsyncWithSchemaOption, ((object)err).ToString());
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsyncWithSchemaOption, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseCreator, OracleTraceFuncName.DeleteAsyncWithSchemaOption);
				}
			}
		}

		private string GenerateSchemaFilter(string[] schemas)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < schemas.Length; i++)
			{
				string schema = schemas[i].Trim().Replace("'", "''");
				if (schema.StartsWith("\"") && schema.EndsWith("\""))
				{
					schema = schema.Trim(new char[1]
					{
						'"'
					});
				}
				list.Add(schema);
			}
			string userID = new OracleConnectionStringBuilder(_connection.DbConnection.ConnectionString).UserID;
			userID = userID.Trim();
			userID = ((!userID.StartsWith("\"") || !userID.EndsWith("\"")) ? userID.ToUpper() : userID.Trim(new char[1]
			{
				'"'
			}));
			userID = userID.Replace("'", "''");
			if (!list.Contains(userID))
			{
				list.Add(userID);
			}
			return string.Join(", ", list.Select((string x) => "'" + x + "'"));
		}
	}
}
