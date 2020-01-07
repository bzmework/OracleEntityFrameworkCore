using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Oracle.EntityFrameworkCore.Migrations.Internal
{
	/// <summary>
	/// Ǩ������ִ����
	/// </summary>
	public class OracleMigrationCommandExecutor : MigrationCommandExecutor
	{
		private IDiagnosticsLogger<DbLoggerCategory.Database> m_oracleLogger;

		/// <summary>
		/// ִ�в�����һ��һ�е�����
		/// </summary>
		/// <param name="migrationCommands">Ǩ������</param>
		/// <param name="connection">���Ӷ���</param>
		/// <param name="logger">��־</param>
		/// <returns></returns>
		public object ExecuteScalar(
			IEnumerable<MigrationCommand> migrationCommands, 
			IRelationalConnection connection, 
			IDiagnosticsLogger<DbLoggerCategory.Database> logger = null)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteScalar);
				}

				m_oracleLogger = logger;
				object result = new object();
				Check.NotNull(migrationCommands, nameof(migrationCommands));
				Check.NotNull(connection, nameof(connection));
				using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
				{
					connection.Open();
					try
					{
						IDbContextTransaction dbContextTransaction = null;
						try
						{
							foreach (MigrationCommand migrationCommand in migrationCommands)
							{
								if (dbContextTransaction == null && !migrationCommand.TransactionSuppressed)
								{
									dbContextTransaction = connection.BeginTransaction();
								}
								if (dbContextTransaction != null && migrationCommand.TransactionSuppressed)
								{
									dbContextTransaction.Commit();
									dbContextTransaction.Dispose();
									dbContextTransaction = null;
								}
								result = ((OracleMigrationCommand)migrationCommand).ExecuteScalar(connection);
							}
							dbContextTransaction?.Commit();
						}
						finally
						{
							dbContextTransaction?.Dispose();
						}
					}
					finally
					{
						connection.Close();
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteScalar, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteScalar);
				}
			}
		}

		/// <summary>
		/// ִ��
		/// </summary>
		/// <param name="migrationCommands">Ǩ������</param>
		/// <param name="connection">���Ӷ���</param>
		/// <param name="logger">��־</param>
		public void ExecuteNonQuery(
			IEnumerable<MigrationCommand> migrationCommands,
			IRelationalConnection connection, 
			IDiagnosticsLogger<DbLoggerCategory.Database> logger = null)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteNonQuery);
				}

				m_oracleLogger = logger;
				Check.NotNull(migrationCommands, nameof(migrationCommands));
				Check.NotNull(connection, nameof(connection));
				using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
				{
					connection.Open();
					try
					{
						IDbContextTransaction dbContextTransaction = null;
						try
						{
							foreach (MigrationCommand migrationCommand in migrationCommands)
							{
								if (dbContextTransaction == null && !migrationCommand.TransactionSuppressed)
								{
									dbContextTransaction = connection.BeginTransaction();
								}
								if (dbContextTransaction != null && migrationCommand.TransactionSuppressed)
								{
									dbContextTransaction.Commit();
									dbContextTransaction.Dispose();
									dbContextTransaction = null;
								}
								try
								{
									migrationCommand.ExecuteNonQuery(connection);
								}
								catch (Exception ex)
								{
									if (!migrationCommand.CommandText.StartsWith("CREATE UNIQUE INDEX") || (!ex.Message.Contains("ORA-01408") && !ex.Message.Contains("ORA-00955")))
									{
										throw;
									}
								}
							}
							dbContextTransaction?.Commit();
						}
						finally
						{
							dbContextTransaction?.Dispose();
						}
					}
					finally
					{
						connection.Close();
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteNonQuery, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleMigrationCommandExecutor, OracleTraceFuncName.ExecuteNonQuery);
				}
			}
		}
	}
}
