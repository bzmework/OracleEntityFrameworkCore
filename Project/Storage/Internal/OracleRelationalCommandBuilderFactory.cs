using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// RelationalCommand创建器工厂，用于决定创建哪种RelationalCommand
	/// </summary>
	public class OracleRelationalCommandBuilderFactory : RelationalCommandBuilderFactory
	{
		private sealed class OracleRelationalCommandBuilder : RelationalCommandBuilder
		{
			private sealed class OracleRelationalCommand : RelationalCommand
			{
				/// <summary>
				/// 实例化
				/// </summary>
				/// <param name="logger"></param>
				/// <param name="commandText"></param>
				/// <param name="parameters"></param>
				public OracleRelationalCommand(
					IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
					string commandText, 
					IReadOnlyList<IRelationalParameter> parameters)
					: base(logger, AdjustCommandText(commandText), parameters)
				{
					if (Check.IsTraceEnabled(logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.ctor);
					}
					if (Check.IsTraceEnabled(logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.ctor);
					}
				}

				private static new string AdjustCommandText(string commandText)
				{
					commandText = commandText.Trim();
					if (commandText.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase) || commandText.StartsWith("DECLARE", StringComparison.OrdinalIgnoreCase) || commandText.StartsWith("CREATE OR REPLACE", StringComparison.OrdinalIgnoreCase) || !commandText.EndsWith(";", StringComparison.Ordinal))
					{
						return commandText;
					}
					return commandText.Substring(0, commandText.Length - 1);
				}

				/// <summary>
				/// 执行 denglf
				/// </summary>
				/// <param name="connection"></param>
				/// <param name="executeMethod"></param>
				/// <param name="parameterValues"></param>
				/// <returns></returns>
				protected override object Execute(IRelationalConnection connection, DbCommandMethod executeMethod, IReadOnlyDictionary<string, object> parameterValues)
				{
					try
					{
						if (Check.IsTraceEnabled(base.Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.Execute);
						}
						Check.NotNull(connection, nameof(connection));
						
						// 创建命令并打开连接
						DbCommand dbCommand = CreateCommand(connection, parameterValues);
						connection.Open();
						
						Guid commandId = Guid.NewGuid();
						DateTimeOffset utcNow = DateTimeOffset.UtcNow;
						Stopwatch stopwatch = Stopwatch.StartNew();

						if (Check.IsTraceEnabled(base.Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.Execute, dbCommand.CommandText);
						}

						// 执行命令
						Logger.CommandExecuting(dbCommand, executeMethod, commandId, connection.ConnectionId, async: false, utcNow);
						
						// 读取数据
						bool readerOpen = false;
						object value;
						try
						{
							switch (executeMethod)
							{
							case DbCommandMethod.ExecuteNonQuery:
								value = dbCommand.ExecuteNonQuery();
								break;
							case DbCommandMethod.ExecuteScalar:
								value = dbCommand.ExecuteScalar();
								break;
							case DbCommandMethod.ExecuteReader:
								value = new RelationalDataReader(connection, dbCommand, dbCommand.ExecuteReader(), commandId, Logger);
								readerOpen = true;
								break;
							default:
								throw new NotSupportedException();
							}
							Logger.CommandExecuted(dbCommand, executeMethod, commandId, connection.ConnectionId, value, async: false, utcNow, stopwatch.Elapsed);
						}
						catch (Exception ex)
						{
							if (Check.IsErrorEnabled(Logger?.Logger))
							{
								Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.Execute, ex.ToString());
							}
							Logger.CommandError(dbCommand, executeMethod, commandId, connection.ConnectionId, ex, async: false, utcNow, stopwatch.Elapsed);
							throw;
						}
						finally
						{
							if (!readerOpen)
							{
								dbCommand.Dispose();
								connection.Close();
							}
							dbCommand.Parameters.Clear();
							if (Check.IsTraceEnabled(Logger?.Logger))
							{
								Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.Execute);
							}
						}
						return value;
					}
					catch (Exception ex)
					{
						if (Check.IsErrorEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.Execute, ex.ToString());
						}
						throw;
					}
					finally
					{
						if (Check.IsTraceEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.Execute);
						}
					}
				}

				/// <summary>
				/// 异步执行
				/// </summary>
				/// <param name="connection"></param>
				/// <param name="executeMethod"></param>
				/// <param name="parameterValues"></param>
				/// <param name="cancellationToken"></param>
				/// <returns></returns>
				protected override async Task<object> ExecuteAsync(IRelationalConnection connection, DbCommandMethod executeMethod, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken))
				{
					if (Check.IsTraceEnabled(Logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.ExecuteAsync);
					}
					Check.NotNull(connection, nameof(connection));

					// 创建命令并打开连接
					DbCommand dbCommand = CreateCommand(connection, parameterValues);
					await connection.OpenAsync(cancellationToken);

					Guid commandId = Guid.NewGuid();
					DateTimeOffset startTime = DateTimeOffset.UtcNow;
					Stopwatch stopwatch = Stopwatch.StartNew();

					// 执行命令
					Logger.CommandExecuting(dbCommand, executeMethod, commandId, connection.ConnectionId, async: true, startTime);
					
					// 读取数据
					bool readerOpen = false;
					try
					{
						object obj;
						switch (executeMethod)
						{
						case DbCommandMethod.ExecuteNonQuery:
							obj = await dbCommand.ExecuteNonQueryAsync(cancellationToken);
							break;
						case DbCommandMethod.ExecuteScalar:
							obj = await dbCommand.ExecuteScalarAsync(cancellationToken);
							break;
						case DbCommandMethod.ExecuteReader:
						{
							DbCommand command = dbCommand;
							obj = new RelationalDataReader(connection, command, await dbCommand.ExecuteReaderAsync(cancellationToken), commandId, Logger);
							readerOpen = true;
							break;
						}
						default:
							throw new NotSupportedException();
						}
						Logger.CommandExecuted(dbCommand, executeMethod, commandId, connection.ConnectionId, obj, async: true, startTime, stopwatch.Elapsed);
						return obj;
					}
					catch (Exception ex)
					{
						if (Check.IsErrorEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.ExecuteAsync, ex.ToString());
						}
						Logger.CommandError(dbCommand, executeMethod, commandId, connection.ConnectionId, ex, async: true, startTime, stopwatch.Elapsed);
						throw;
					}
					finally
					{
						if (!readerOpen)
						{
							dbCommand.Dispose();
							connection.Close();
						}
						dbCommand.Parameters.Clear();
						if (Check.IsTraceEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.ExecuteAsync);
						}
					}
				}

				/// <summary>
				/// 创建命令
				/// </summary>
				/// <param name="connection">连接对象</param>
				/// <param name="parameterValues">参数</param>
				/// <returns></returns>
				private new DbCommand CreateCommand(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues)
				{
					if (Check.IsTraceEnabled(Logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.CreateCommand);
					}

					try
					{
						DbCommand dbCommand = connection.DbConnection.CreateCommand();
						((OracleCommand)dbCommand).BindByName = true;
						dbCommand.CommandText = CommandText;

						if (connection.CurrentTransaction != null)
						{
							dbCommand.Transaction = connection.CurrentTransaction.GetDbTransaction();
						}

						if (connection.CommandTimeout.HasValue)
						{
							dbCommand.CommandTimeout = connection.CommandTimeout.Value;
						}

						if (Parameters.Count > 0)
						{
							if (parameterValues == null)
							{
								throw new InvalidOperationException(RelationalStrings.MissingParameterValue(Parameters[0].InvariantName));
							}
							foreach (IRelationalParameter parameter in Parameters)
							{
								parameter.AddDbParameter(dbCommand, parameterValues);
							}
						}

						return dbCommand;
					}
					catch (Exception ex)
					{
						if (Check.IsErrorEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.CreateCommand, ex.ToString());
						}
						throw;
					}
					finally
					{
						if (Check.IsTraceEnabled(Logger?.Logger))
						{
							Trace<DbLoggerCategory.Database.Command>.Write(Logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommand, OracleTraceFuncName.CreateCommand);
						}
					}
				}
			}

			/// <summary>
			/// 实例化RelationalCommand创建器
			/// </summary>
			/// <param name="logger">日志</param>
			/// <param name="typeMappingSource">映射源</param>
			public OracleRelationalCommandBuilder(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, IRelationalTypeMappingSource typeMappingSource)
				: base(logger, typeMappingSource)
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommandBuilder, OracleTraceFuncName.ctor);
				}
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommandBuilder, OracleTraceFuncName.ctor);
				}
			}

			/// <summary>
			/// 创建核心
			/// </summary>
			/// <param name="logger">日志</param>
			/// <param name="commandText">命令</param>
			/// <param name="parameters">参数</param>
			/// <returns></returns>
			protected override IRelationalCommand BuildCore(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, string commandText, IReadOnlyList<IRelationalParameter> parameters)
			{
				try
				{
					if (Check.IsTraceEnabled(logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommandBuilder, OracleTraceFuncName.BuildCore);
					}

					return new OracleRelationalCommand(logger, commandText, parameters);
				}
				catch (Exception ex)
				{
					if (Check.IsErrorEnabled(logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommandBuilder, OracleTraceFuncName.BuildCore, ex.ToString());
					}
					throw;
				}
				finally
				{
					if (Check.IsTraceEnabled(logger?.Logger))
					{
						Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommandBuilder, OracleTraceFuncName.BuildCore);
					}
				}
			}
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="logger">日期</param>
		/// <param name="typeMappingSource">映射源</param>
		public OracleRelationalCommandBuilderFactory([NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, [NotNull] IRelationalTypeMappingSource typeMappingSource)
			: base(logger, typeMappingSource)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.ctor);
			}

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 创建RelationalCommand
		/// </summary>
		/// <param name="logger">日志</param>
		/// <param name="relationalTypeMappingSource">映射源</param>
		/// <returns></returns>
		protected override IRelationalCommandBuilder CreateCore(
			IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, 
			IRelationalTypeMappingSource relationalTypeMappingSource)
		{
			try
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.CreateCore);
				}

				return new OracleRelationalCommandBuilder(logger, relationalTypeMappingSource);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.CreateCore, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(logger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalCommandBuilderFactory, OracleTraceFuncName.CreateCore);
				}
			}
		}
	}
}
