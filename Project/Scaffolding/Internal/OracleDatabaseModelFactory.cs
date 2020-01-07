using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Internal;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Scaffolding.Internal
{
	/// <summary>
	/// 数据库模型工厂
	/// </summary>
	public class OracleDatabaseModelFactory : IDatabaseModelFactory
	{
		private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> m_oracleLogger;

		private const string NamePartRegex = "(?:(?:\\[(?<part{0}>(?:(?:\\]\\])|[^\\]])+)\\])|(?<part{0}>[^\\.\\[\\]]+))";

		private static readonly Regex _partExtractor = new Regex(
			string.Format(CultureInfo.InvariantCulture, "^{0}(?:\\.{1})?$", 
				string.Format(CultureInfo.InvariantCulture, NamePartRegex, 1), 
				string.Format(CultureInfo.InvariantCulture, NamePartRegex, 2)), 
				RegexOptions.Compiled, TimeSpan.FromMilliseconds(1000.0));

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="logger">日志</param>
		public OracleDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger = null)
		{
			Check.NotNull(logger, nameof(logger));
			m_oracleLogger = logger;
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <param name="tables">表</param>
		/// <param name="schemas">方案</param>
		/// <returns></returns>
		public virtual DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
		{
			Check.NotEmpty(connectionString, nameof(connectionString));
			Check.NotNull(tables, nameof(tables));
			Check.NotNull(schemas, nameof(schemas));
			OracleConnection val = (OracleConnection)(object)new OracleConnection(connectionString);
			try
			{
				return Create((DbConnection)(object)val, tables, schemas);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="connection">连接对象</param>
		/// <param name="tables">表</param>
		/// <param name="schemas">方案</param>
		/// <returns></returns>
		public virtual DatabaseModel Create(DbConnection connection, IEnumerable<string> tables, IEnumerable<string> schemas)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.Create);
				}

				Check.NotNull(connection, nameof(connection));
				Check.NotNull(tables, nameof(tables));
				Check.NotNull(schemas, nameof(schemas));

				DatabaseModel databaseModel = new DatabaseModel();
				bool open = connection.State == ConnectionState.Open;
				if (!open)
				{
					connection.Open();
					open = true;
				}
				try
				{
					databaseModel.DefaultSchema = GetDefaultSchema(connection);
					var schemaList = schemas.ToList();
					var schemaFilter = GenerateSchemaFilter(schemaList);
					var tableList = tables.ToList();
					
					if (schemaList.Count == 0)
					{
						List<(string, string)> tempTables = new List<(string, string)>();
						for (int i = 0; i < tableList.Count; i++)
						{
							if (i < schemaList.Count)
							{
								if (string.IsNullOrEmpty(schemaList[i]))
								{
									tempTables.Add((databaseModel.DefaultSchema, tableList[i]));
								}
								else
								{
									tempTables.Add((schemaList[i], tableList[i]));
								}
							}
							else
							{
								tempTables.Add((databaseModel.DefaultSchema, tableList[i]));
							}
						}
						var tableFilter = GenerateTableFilter(tempTables, null);

						GetSequencesCombined(connection, databaseModel.Tables, schemaFilter, databaseModel);
						GetTablesCombined(connection, tableFilter, databaseModel, schemaFilter);
					}
					else
					{
						var tableFilter = GenerateTableFilter(tableList.Select(Parse1).ToList(), schemaFilter);
						GetSequencesCombined(connection, databaseModel.Tables, schemaFilter, databaseModel);
						GetTablesCombined(connection, tableFilter, databaseModel, schemaFilter);
					}

					foreach (var schemaName in schemaList.Except(databaseModel.Sequences.Select((DatabaseSequence s) => s.Schema).Concat(databaseModel.Tables.Select((DatabaseTable t) => t.Schema))))
					{
						m_oracleLogger.MissingSchemaWarning(schemaName);
					}

					foreach (var tableName in tableList)
					{
						(string, string) valueTuple = Parse(tableName);
						string Schema = valueTuple.Item1;
						string Table = valueTuple.Item2;
						if (!databaseModel.Tables.Any((DatabaseTable t) => (!string.IsNullOrEmpty(Schema) && t.Schema == Schema) || t.Name == Table))
						{
							m_oracleLogger.MissingTableWarning(tableName);
						}
					}

					return databaseModel;
				}
				finally
				{
					if (open)
					{
						connection.Close();
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.Create, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.Create);
				}
			}
		}

		private void GetSequencesCombined(DbConnection connection, IList<DatabaseTable> tables, Func<string, string> schemaFilter, DatabaseModel databaseModel)
		{
			using (DbCommand dbCommand = connection.CreateCommand())
			{
				if (schemaFilter == null)
				{
					dbCommand.CommandText = "SELECT s.sequence_name name FROM user_sequences s";
				}
				else
				{
					dbCommand.CommandText = "SELECT s.sequence_name name, s.sequence_owner owner FROM all_sequences s ";
					dbCommand.CommandText = dbCommand.CommandText + "WHERE " + schemaFilter("s.sequence_owner");
				}
				using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
				{
					while (dbDataReader.Read())
					{
						string valueOrDefault = dbDataReader.GetValueOrDefault<string>("name");
						string schema = null;
						if (schemaFilter != null)
						{
							schema = dbDataReader.GetValueOrDefault<string>("owner");
						}
						DatabaseSequence databaseSequence = new DatabaseSequence
						{
							Name = valueOrDefault,
							Schema = schema
						};
						databaseSequence.Database = databaseModel;
						databaseModel.Sequences.Add(databaseSequence);

						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetSequencesCombined, $"sequence name: {databaseSequence.Name}");
						}
					}
				}
			}
		}

		private void GetTablesCombined(DbConnection connection, Func<string, string, string> tableFilter, DatabaseModel databaseModel, Func<string, string> schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetTablesCombined);
				}

				using (DbCommand dbCommand = connection.CreateCommand())
				{
					string select = null;
					string where = null;
					if (schemaFilter == null)
					{
						select = "SELECT sys_context('userenv', 'current_schema') AS schema, t.table_name AS name FROM user_tables t ";
						where = "WHERE t.table_name <> '__EFMigrationsHistory' " + ((tableFilter != null) ? (" AND " + tableFilter("sys_context('userenv', 'current_schema')", "t.table_name")) : "");
					}
					else
					{
						select = "SELECT t.table_name name, t.owner schema FROM all_tables t ";
						where = "WHERE t.table_name <> '__EFMigrationsHistory' " + ((tableFilter != null) ? (" AND " + tableFilter("t.owner", "t.table_name")) : ((schemaFilter != null) ? (" AND " + schemaFilter("t.owner")) : ""));
					}
					dbCommand.CommandText = select + where;

					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetTables, dbCommand.CommandText);
					}

					using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
					{
						while (dbDataReader.Read())
						{
							string schema = dbDataReader.GetValueOrDefault<string>("schema");
							string name = dbDataReader.GetValueOrDefault<string>("name");
							m_oracleLogger.TableFound(DisplayName(schema, name));
							DatabaseTable item = new DatabaseTable
							{
								Schema = schema,
								Name = name
							};

							if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
							{
								Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetTables, $"schema: {schema}, table name: {name}");
							}

							databaseModel.Tables.Add(item);
						}
					}

					GetColumnsCombined(connection, where, databaseModel, schemaFilter);
					GetKeysCombined(connection, where, databaseModel, schemaFilter);
					GetIndexesCombined(connection, where, databaseModel, schemaFilter);
					GetForeignKeysCombined(connection, where, databaseModel, schemaFilter);
				}
			}
			catch (Exception ex)
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetTablesCombined, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetTablesCombined);
				}
			}
		}

		private void GetColumnsCombined(DbConnection connection, string tableFilter, DatabaseModel databaseModel, Func<string, string> schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined);
				}

				OracleCommand val = (OracleCommand)(object)(OracleCommand)connection.CreateCommand();
				try
				{
					// 为了提高性能，此SQL应该用sys核心表提换

					val.InitialLONGFetchSize = -1;
					StringBuilder selectBuilder = new StringBuilder();
					selectBuilder.AppendLine("select u.*, v.trigger_name, v.table_name, v.column_name, v.table_owner from (SELECT").AppendLine((schemaFilter == null) ? "   sys_context('userenv', 'current_schema') as schema," : " c.owner schema,").AppendLine("   c.table_name,")
						.AppendLine("   c.column_name,")
						.AppendLine("   c.column_id,")
						.AppendLine("   c.data_type,")
						.AppendLine("   c.char_length,")
						.AppendLine("   c.data_length,")
						.AppendLine("   c.data_precision,")
						.AppendLine("   c.data_scale,")
						.AppendLine("   c.nullable,");

					string str = "   c.identity_column,";
					StringBuilder fromBuilder = new StringBuilder();
					fromBuilder
						.AppendLine("   c.data_default,")
						.AppendLine("   c.virtual_column,")
						.AppendLine("   c.hidden_column ")
						.AppendLine((schemaFilter == null) ? "FROM user_tab_cols c" : "FROM all_tab_cols c")
						.AppendLine((schemaFilter == null) ? "INNER JOIN user_tables t " : "INNER JOIN all_tables t")
						.AppendLine("ON t.table_name=c.table_name")
						.AppendLine((schemaFilter == null) ? "" : " AND t.owner=c.owner")
						.AppendLine(tableFilter)
						.AppendLine((schemaFilter == null) ? ")u left join USER_TRIGGER_COLS v " : ")u left join ALL_TRIGGER_COLS v ")
						.AppendLine("on u.table_name = v.table_name and u.column_name = v.column_name and u.schema = v.table_owner ORDER BY u.column_id");
					
					if (connection.ServerVersion.StartsWith("11.2"))
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, "Server Version: 11.2");
						}
						((DbCommand)(object)val).CommandText = selectBuilder.ToString() + fromBuilder.ToString();
					}
					else
					{
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, "Server Version: NOT 11.2");
						}
						((DbCommand)(object)val).CommandText = selectBuilder.ToString() + str + fromBuilder.ToString();
					}

					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, ((DbCommand)(object)val).CommandText);
					}

					OracleDataReader reader = val.ExecuteReader();
					try
					{
						foreach (IGrouping<(string, string), DbDataRecord> columns in ((IEnumerable)reader).Cast<DbDataRecord>().GroupBy((Func<DbDataRecord, (string, string)>)((DbDataRecord ddr) => (ddr.GetValueOrDefault<string>("schema"), ddr.GetValueOrDefault<string>("table_name")))))
						{
							string tableSchema = columns.Key.Item1;
							string tableName = columns.Key.Item2;
							DatabaseTable databaseTable = databaseModel.Tables.Single((DatabaseTable t) => t.Schema == tableSchema && t.Name == tableName);
							List<string> list = new List<string>();
							foreach (DbDataRecord column in columns)
							{
								string columnName = column.GetValueOrDefault<string>("column_name");
								string triggerName = column.GetValueOrDefault<string>("trigger_name");
								
								bool isGenerated = !string.IsNullOrEmpty(triggerName);
								if (isGenerated && Check.IsTraceEnabled(m_oracleLogger?.Logger))
								{
									Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, $"Trigger/Sequence column. Column Name: {columnName}, Trigger Name: {triggerName}");
								}

								if (list.Contains(columnName))
								{
									if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
									{
										Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, string.Format("trigger column name: " + columnName + ". Multiple triggers on same column."));
									}
								}
								else
								{
									list.Add(columnName);
									var columnID = column.GetValueOrDefault<int>("column_id");
									var dataType = column.GetValueOrDefault<string>("data_type");
									var charLength = column.GetValueOrDefault<int>("char_length");
									var dataLength = column.GetValueOrDefault<int>("data_length");
									var dataPrecision = column.GetValue("data_precision");
									var dataScale = column.GetValue("data_scale");
									var isNullable = column.GetValueOrDefault<string>("nullable").Equals("Y");
									var isIdentity = false;
									if (!connection.ServerVersion.StartsWith("11.2"))
									{
										isIdentity = column.GetValueOrDefault<string>("identity_column").Equals("YES");
										if (isIdentity && Check.IsTraceEnabled(m_oracleLogger?.Logger))
										{
											Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, "Identity column. Name: " + columnName);
										}
									}
									var defaultValue = (!isIdentity) ? column.GetValueOrDefault<string>("data_default") : null;
									var computedValue = column.GetValueOrDefault<string>("virtual_column").Equals("YES") ? defaultValue : null;
									var oracleClrType = GetOracleClrType(dataType, dataPrecision, dataScale, charLength, dataLength);

									if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
									{
										Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, $"storeType: {oracleClrType}, dataTypeName: {dataType}, charLength: {charLength}, dataLength: {dataLength}, precision: {dataPrecision}, scale: {dataScale}, databaseGenerated: {isGenerated}, isIdentity: {isIdentity}, computedValue: {computedValue}, tableSchema: {tableSchema}, tableName: {tableName}, columnName: {columnName}, ordinal: {columnID}, isNullable: {isNullable}, defaultValue: {defaultValue}");
									}

									if (string.IsNullOrWhiteSpace(defaultValue) || !string.IsNullOrWhiteSpace(computedValue))
									{
										defaultValue = null;
									}
									m_oracleLogger.ColumnFound(DisplayName(tableSchema, tableName), columnName, columnID, dataType, charLength, (dataPrecision != null) ? Convert.ToInt32(dataPrecision) : 0, (dataScale != null) ? Convert.ToInt32(dataScale) : 0, isNullable, isIdentity | isGenerated, defaultValue, computedValue);
									DatabaseColumn databaseColumn = new DatabaseColumn
									{
										Table = databaseTable,
										Name = columnName,
										StoreType = oracleClrType,
										IsNullable = isNullable,
										DefaultValueSql = defaultValue,
										ComputedColumnSql = computedValue,
										ValueGenerated = ((isIdentity | isGenerated) ? new ValueGenerated?(ValueGenerated.OnAdd) : null)
									};
									if (string.Compare(column.GetValueOrDefault<string>("hidden_column"), "YES", ignoreCase: true) != 0)
									{
										databaseTable.Columns.Add(databaseColumn);
									}
									else if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
									{
										Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, $"Skipping Hidden Column. Column Name: {databaseColumn.Name}");
									}
								}
							}
						}
					}
					finally
					{
						((IDisposable)reader)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetColumnsCombined);
				}
			}
		}

		private void GetKeysCombined(DbConnection connection, string tableFilter, DatabaseModel databaseModel, Func<string, string> schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetKeysCombined);
				}

				using (DbCommand dbCommand = connection.CreateCommand())
				{
					dbCommand.CommandText = new StringBuilder()
						.AppendLine("SELECT")
						.AppendLine((schemaFilter == null) ? "   sys_context('userenv', 'current_schema') as owner," : " t.owner as owner,")
						.AppendLine("   t.table_name,")
						.AppendLine("   t.column_name,")
						.AppendLine("   c.delete_rule,")
						.AppendLine("   t.constraint_name,")
						.AppendLine("   c.constraint_type")
						.AppendLine((schemaFilter == null) ? "FROM user_cons_columns t" : "FROM all_cons_columns t")
						.AppendLine((schemaFilter == null) ? "JOIN user_constraints c" : "JOIN all_constraints c")
						.AppendLine("   ON t.CONSTRAINT_NAME = c.CONSTRAINT_NAME")
						.AppendLine("INNER JOIN all_tables x")
						.AppendLine("   ON x.table_name = t.table_name ")
						.AppendLine(tableFilter)
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("c.owner")))
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("x.owner")))
						.AppendLine(" AND c.constraint_type IN ('P','U') ")
						.ToString();
					
					using (DbDataReader source = dbCommand.ExecuteReader())
					{
						foreach (IGrouping<(string, string), DbDataRecord> table in source.Cast<DbDataRecord>().GroupBy((Func<DbDataRecord, (string, string)>)((DbDataRecord ddr) => (ddr.GetValueOrDefault<string>("owner"), ddr.GetValueOrDefault<string>("table_name")))))
						{
							var tableSchema = table.Key.Item1;
							var tableName = table.Key.Item2;
							string columnName = "";

							DatabaseTable databaseTable = databaseModel.Tables.Single((DatabaseTable t) => t.Schema == tableSchema && t.Name == tableName);
							
							// 主键
							IGrouping<string, DbDataRecord>[] primaryKeys = 
								(from ddr in table
								where ddr.GetValueOrDefault<string>("constraint_type").Equals("P")
								group ddr by ddr.GetValueOrDefault<string>("constraint_name")).ToArray();
							if (primaryKeys.Length == 1)
							{
								IGrouping<string, DbDataRecord> primaryKey = primaryKeys[0];
								m_oracleLogger.PrimaryKeyFound(primaryKey.Key, DisplayName(tableSchema, tableName));
								DatabasePrimaryKey databasePrimaryKey = new DatabasePrimaryKey
								{
									Table = databaseTable,
									Name = primaryKey.Key
								};
								foreach (DbDataRecord column in primaryKey)
								{
									columnName = column.GetValueOrDefault<string>("column_name");
									DatabaseColumn item = databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name == columnName) ?? databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
									if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
									{
										Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetKeysCombined, $"tableName: {tableName}, tableSchema: {tableSchema}, primaryKeyName: {databasePrimaryKey.Name}, columnName: {columnName}");
									}
									databasePrimaryKey.Columns.Add(item);
								}
								databaseTable.PrimaryKey = databasePrimaryKey;
							}

							// 唯一键
							IGrouping<string, DbDataRecord>[] uniqueKeys = 
								(from ddr in table
								where ddr.GetValueOrDefault<string>("constraint_type").Equals("U")
								group ddr by ddr.GetValueOrDefault<string>("constraint_name")).ToArray();
							foreach (var uniqueKey in uniqueKeys)
							{
								m_oracleLogger.UniqueConstraintFound(uniqueKey.Key, DisplayName(tableSchema, tableName));
								DatabaseUniqueConstraint databaseUniqueConstraint = new DatabaseUniqueConstraint
								{
									Table = databaseTable,
									Name = uniqueKey.Key
								};
								foreach (DbDataRecord item in uniqueKey)
								{
									columnName = item.GetValueOrDefault<string>("column_name");
									DatabaseColumn column = databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name == columnName) ?? databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
									databaseUniqueConstraint.Columns.Add(column);
								}
								databaseTable.UniqueConstraints.Add(databaseUniqueConstraint);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetKeysCombined, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetKeysCombined);
				}
			}
		}

		private void GetForeignKeysCombined(DbConnection connection, string tableFilter, DatabaseModel databaseModel, Func<string, string> schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetForeignKeysCombined);
				}

				using (DbCommand dbCommand = connection.CreateCommand())
				{
					dbCommand.CommandText = new StringBuilder()
						.AppendLine("SELECT DISTINCT r.owner,")
						.AppendLine("     t.table_name,")
						.AppendLine("     t.column_name,")
						.AppendLine("     c.table_name principal_table_name,")
						.AppendLine("     r.delete_rule,")
						.AppendLine("     r.constraint_name,")
						.AppendLine("     c.column_name principal_column_name")
						.AppendLine("FROM all_cons_columns t,")
						.AppendLine("     all_cons_columns c,")
						.AppendLine("     all_constraints r,")
						.AppendLine((schemaFilter == null) ? "     user_tables x" : "     all_tables x")
						.AppendLine(tableFilter)
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("x.owner")))
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("r.owner")))
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("c.owner")))
						.AppendLine("  AND t.constraint_name = r.constraint_name")
						.AppendLine("  AND x.table_name = t.table_name")
						.AppendLine("  AND c.constraint_name = r.r_constraint_name")
						.AppendLine("  AND c.owner = r.r_owner")
						.AppendLine("  AND r.constraint_type = 'R'")
						.AppendLine((schemaFilter == null) ? (" AND r.owner = '" + databaseModel.DefaultSchema + "'") : "")
						.AppendLine((schemaFilter == null) ? " ORDER BY t.table_name, column_name, principal_column_name, r.constraint_name" : " ORDER BY r.owner, t.table_name, column_name, principal_column_name, r.constraint_name")
						.ToString();

					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetForeignKeysCombined, dbCommand.CommandText);
					}

					using (DbDataReader source = dbCommand.ExecuteReader())
					{
						foreach (IGrouping<(string, string), DbDataRecord> table in source.Cast<DbDataRecord>().GroupBy((Func<DbDataRecord, (string, string)>)((DbDataRecord ddr) => (ddr.GetValueOrDefault<string>("owner"), ddr.GetValueOrDefault<string>("table_name")))))
						{
							string tableSchema = table.Key.Item1;
							string tableName = table.Key.Item2;
							DatabaseTable databaseTable = databaseModel.Tables.Single((DatabaseTable t) => t.Schema == tableSchema && t.Name == tableName);
							foreach (IGrouping<(string, string, string, string), DbDataRecord> columns in ((IEnumerable<DbDataRecord>)table).GroupBy((Func<DbDataRecord, (string, string, string, string)>)((DbDataRecord c) => (c.GetValueOrDefault<string>("constraint_name"), c.GetValueOrDefault<string>("owner"), c.GetValueOrDefault<string>("principal_table_name"), c.GetValueOrDefault<string>("delete_rule")))))
							{
								string foreignKeyName = columns.Key.Item1;
								string principalTableSchema = columns.Key.Item2;
								string principalTableName = columns.Key.Item3;
								string action = columns.Key.Item4;
								m_oracleLogger.ForeignKeyFound(foreignKeyName, DisplayName(databaseTable.Schema, databaseTable.Name), DisplayName(principalTableSchema, principalTableName), action);
								DatabaseTable principalTable = databaseModel.Tables.FirstOrDefault((DatabaseTable t) => t.Schema == principalTableSchema && t.Name == principalTableName) ?? databaseModel.Tables.FirstOrDefault((DatabaseTable t) => t.Schema.Equals(principalTableSchema, StringComparison.OrdinalIgnoreCase) && t.Name.Equals(principalTableName, StringComparison.OrdinalIgnoreCase));
								if (principalTable == null)
								{
									m_oracleLogger.ForeignKeyReferencesMissingPrincipalTableWarning(foreignKeyName, DisplayName(databaseTable.Schema, databaseTable.Name), DisplayName(principalTableSchema, principalTableName));
								}
								else
								{
									DatabaseForeignKey databaseForeignKey = new DatabaseForeignKey
									{
										Name = foreignKeyName,
										Table = databaseTable,
										PrincipalTable = principalTable,
										OnDelete = ConvertToReferentialAction(action)
									};
									bool flag = false;
									foreach (DbDataRecord column in columns)
									{
										string columnName = column.GetValueOrDefault<string>("column_name");
										DatabaseColumn databaseColumn = databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name == columnName) ?? databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
										string principalColumnName = column.GetValueOrDefault<string>("principal_column_name");
										DatabaseColumn databaseColumn2 = databaseForeignKey.PrincipalTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name == principalColumnName) ?? databaseForeignKey.PrincipalTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name.Equals(principalColumnName, StringComparison.OrdinalIgnoreCase));
										if (databaseColumn2 == null)
										{
											flag = true;
											m_oracleLogger.ForeignKeyPrincipalColumnMissingWarning(foreignKeyName, DisplayName(databaseTable.Schema, databaseTable.Name), principalColumnName, DisplayName(principalTableSchema, principalTableName));
											break;
										}

										if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
										{
											Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetForeignKeysCombined, $"tableName: {tableName}, tableSchema: {tableSchema}, principalTableName: {principalTableName}, principalTableSchema: {principalTableSchema}, columnName: {columnName}, principalColumnName: {principalColumnName}");
										}

										if (!$"{tableSchema}.{tableName}.{databaseColumn.Name}".Equals($"{principalTableSchema}.{principalTableName}.{principalColumnName}", StringComparison.OrdinalIgnoreCase))
										{
											if (!databaseForeignKey.Columns.Contains(databaseColumn))
											{
												databaseForeignKey.Columns.Add(databaseColumn);
											}
											if (!databaseForeignKey.PrincipalColumns.Contains(databaseColumn2))
											{
												databaseForeignKey.PrincipalColumns.Add(databaseColumn2);
											}
										}
									}
									if (!flag && databaseForeignKey.Columns.Count != 0 && !databaseTable.ForeignKeys.Contains(databaseForeignKey))
									{
										databaseTable.ForeignKeys.Add(databaseForeignKey);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetForeignKeysCombined, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetForeignKeysCombined);
				}
			}
		}

		private void GetIndexesCombined(DbConnection connection, string tableFilter, DatabaseModel databaseModel, Func<string, string> schemaFilter)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetIndexesCombined);
				}

				using (DbCommand dbCommand = connection.CreateCommand())
				{
					// 为了提高性能，此SQL应该用sys核心表提换
					StringBuilder stringBuilder = new StringBuilder()
						.AppendLine("select u.*, c.column_expression from (SELECT")
						.AppendLine((schemaFilter == null) ? "   sys_context('userenv', 'current_schema') as owner," : " t.owner as owner,")
						.AppendLine("   t.uniqueness,")
						.AppendLine("   a.index_name,")
						.AppendLine("   a.table_name,")
						.AppendLine("   a.column_position,")
						.AppendLine("   a.column_name")
						.AppendLine((schemaFilter == null) ? "FROM user_ind_columns a" : "FROM all_ind_columns a")
						.AppendLine((schemaFilter == null) ? "INNER JOIN user_indexes t" : "INNER JOIN all_indexes t")
						.AppendLine("   ON a.index_name = t.index_name")
						.AppendLine((schemaFilter == null) ? "INNER JOIN user_tables x" : "INNER JOIN all_tables x")
						.AppendLine("   ON x.table_name = a.table_name")
						.AppendLine(tableFilter)
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("a.table_owner")))
						.AppendLine((schemaFilter == null) ? "" : (" AND " + schemaFilter("x.owner")))
						.AppendLine(")u left join ALL_IND_EXPRESSIONS c on u.index_name = c.index_name ORDER BY u.table_name, u.index_name, u.column_position");
					dbCommand.CommandText = stringBuilder.ToString();
					((OracleCommand)dbCommand).InitialLONGFetchSize = -1;
					using (DbDataReader source = dbCommand.ExecuteReader())
					{
						foreach (IGrouping<(string, string), DbDataRecord> item in source.Cast<DbDataRecord>().GroupBy((Func<DbDataRecord, (string, string)>)((DbDataRecord ddr) => (ddr.GetValueOrDefault<string>("owner"), ddr.GetValueOrDefault<string>("table_name")))))
						{
							string tableSchema = item.Key.Item1;
							string tableName = item.Key.Item2;
							DatabaseTable databaseTable = databaseModel.Tables.Single((DatabaseTable t) => t.Schema == tableSchema && t.Name == tableName);
							IGrouping<(string, string, bool), DbDataRecord>[] array = ((IEnumerable<DbDataRecord>)item).GroupBy((Func<DbDataRecord, (string, string, bool)>)((DbDataRecord ddr) => (ddr.GetValueOrDefault<string>("index_name"), ddr.GetValueOrDefault<string>("column_expression"), ddr.GetValueOrDefault<string>("uniqueness").Equals("UNIQUE")))).ToArray();
							string columnName;
							foreach (IGrouping<(string, string, bool), DbDataRecord> grouping in array)
							{
								m_oracleLogger.IndexFound(grouping.Key.Item1, DisplayName(tableSchema, tableName), grouping.Key.Item3);
								DatabaseIndex databaseIndex = new DatabaseIndex
								{
									Table = databaseTable,
									Name = grouping.Key.Item1,
									Filter = grouping.Key.Item2,
									IsUnique = grouping.Key.Item3
								};
								foreach (DbDataRecord dataRecord in grouping)
								{
									columnName = dataRecord.GetValueOrDefault<string>("column_name");
									DatabaseColumn databaseColumn = databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name == columnName) ?? databaseTable.Columns.FirstOrDefault((DatabaseColumn c) => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
									if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
									{
										Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetIndexesCombined, $"tableName: {tableName}, tableSchema: {tableSchema}, indexName: {databaseIndex.Name}, isUnique: {databaseIndex.IsUnique}, columnName: {columnName}");
									}
									if (databaseColumn != null)
									{
										databaseIndex.Columns.Add(databaseColumn);
									}
								}
								if (databaseIndex.Columns.Count != 0)
								{
									databaseTable.Indexes.Add(databaseIndex);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetIndexesCombined, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetIndexesCombined);
				}
			}
		}

		private static string DisplayName(string schema, string name)
		{
			return ((!string.IsNullOrEmpty(schema)) ? (schema + ".") : "") + name;
		}

		private string GetDefaultSchema(DbConnection connection)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema);
				}

				using (DbCommand dbCommand = connection.CreateCommand())
				{
					dbCommand.CommandText = "select sys_context('userenv', 'current_schema') from dual";
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema, dbCommand.CommandText);
					}

					if (dbCommand.ExecuteScalar() is string schemaName)
					{
						m_oracleLogger.DefaultSchemaFound(schemaName);
						if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
						{
							Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema, "Schema: " + schemaName);
						}
						return schemaName;
					}

					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema, "NULL schema returned");
					}
					return null;
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Scaffolding>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleDatabaseModelFactory, OracleTraceFuncName.GetDefaultSchema);
				}
			}
		}

		private static string GetOracleClrType(string dataTypeName, object precision, object scale, int charLength, int dataLength)
		{
			string result = dataTypeName;
			switch (dataTypeName.ToUpper())
			{
			case "NUMBER":
			{
				int precisionValue = (precision != null) ? Convert.ToInt32(precision) : 0;
				int scaleValue = (scale != null) ? Convert.ToInt32(scale) : 0;
				if (precision == null && scale != null)
				{
					precisionValue = 38;
				}
				result = ((precisionValue <= 0) ? (dataTypeName ?? "") : ((scaleValue != 0) ? $"{dataTypeName}({precisionValue},{scaleValue})" : $"{dataTypeName}({precisionValue})"));
				break;
			}
			case "VARCHAR2":
				result = ((charLength > 0) ? $"{dataTypeName}({charLength})" : (dataTypeName + "(4000)"));
				break;
			case "NCHAR":
				result = ((charLength > 0) ? $"{dataTypeName}({charLength})" : (dataTypeName + "(1000)"));
				break;
			case "NVARCHAR2":
			case "CHAR":
				result = ((charLength > 0) ? $"{dataTypeName}({charLength})" : (dataTypeName + "(2000)"));
				break;
			case "RAW":
				result = ((dataLength > 0) ? $"{dataTypeName}({dataLength})" : (dataTypeName + "(2000)"));
				break;
			case "NCLOB":
			case "CLOB":
				result = (dataTypeName ?? "");
				break;
			}
			return result;
		}

		private static ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
		{
			if (!(onDeleteAction == "SET NULL"))
			{
				if (!(onDeleteAction == "NO ACTION"))
				{
					if (onDeleteAction == "CASCADE")
					{
						return ReferentialAction.Cascade;
					}
					return null;
				}
				return ReferentialAction.NoAction;
			}
			return ReferentialAction.SetNull;
		}

		private static Func<string, string> GenerateSchemaFilter(IReadOnlyList<string> schemas)
		{
			if (schemas.Count > 0)
			{
				return delegate(string s)
				{
					StringBuilder builder = new StringBuilder();
					builder.Append(s);
					builder.Append(" IN (");
					builder.Append(string.Join(", ", schemas.Select(EscapeLiteral)));
					builder.Append(")");
					return builder.ToString();
				};
			}
			return null;
		}

		private static (string Schema, string Table) Parse(string table)
		{
			return (null, table);
		}

		private static (string Schema, string Table) Parse1(string table)
		{
			Match match = _partExtractor.Match(table.Trim());
			if (!match.Success)
			{
				throw new InvalidOperationException(OracleStrings.InvalidTableToIncludeInScaffolding(table));
			}
			string part1 = match.Groups["part1"].Value.Replace("\"\"", "\"");
			string part2 = match.Groups["part2"].Value.Replace("\"\"", "\"");
			if (!string.IsNullOrEmpty(part2))
			{
				return (part1, part2);
			}
			return (null, part1);
		}

		private static Func<string, string, string> GenerateTableFilter(IReadOnlyList<(string Schema, string Table)> tables, Func<string, string> schemaFilter)
		{
			if (schemaFilter != null || tables.Count > 0)
			{
				return delegate(string s, string t)
				{
                    var tableFilterBuilder = new StringBuilder();
					var openBracket = false;

					if (schemaFilter != null)
					{
						tableFilterBuilder.Append("(").Append(schemaFilter(s));
						openBracket = true;
					}
					if (tables.Count > 0)
					{
						if (openBracket)
						{
							tableFilterBuilder.AppendLine().Append("OR ");
						}
						else
						{
							tableFilterBuilder.Append("(");
							openBracket = true;
						}
						List<(string, string)> list = tables.Where(((string Schema, string Table) e) => string.IsNullOrEmpty(e.Schema)).ToList();
						if (list.Count > 0)
						{
							tableFilterBuilder.Append(t);
							tableFilterBuilder.Append(" IN (");
							tableFilterBuilder.Append(string.Join(", ", ((IEnumerable<(string, string)>)list).Select((Func<(string, string), string>)(((string Schema, string Table) e) => EscapeLiteral(e.Table)))));
							tableFilterBuilder.Append(")");
						}
						List<(string, string)> list2 = tables.Where(((string Schema, string Table) e) => !string.IsNullOrEmpty(e.Schema)).ToList();
                            var tablesWithSchema = tables.Where(e => !string.IsNullOrEmpty(e.Schema)).ToList();
						if (list2.Count > 0)
						{
							if (list.Count > 0)
							{
								tableFilterBuilder.Append(" OR ");
							}
							tableFilterBuilder.Append(t);
							tableFilterBuilder.Append(" IN (");
							tableFilterBuilder.Append(string.Join(", ", ((IEnumerable<(string, string)>)list2).Select((Func<(string, string), string>)(((string Schema, string Table) e) => EscapeLiteral(e.Table)))));
							tableFilterBuilder.Append(") AND CONCAT(");
							tableFilterBuilder.Append(s);
							tableFilterBuilder.Append(", CONCAT(");
							tableFilterBuilder.Append("N'.', ");
							tableFilterBuilder.Append(t);
							tableFilterBuilder.Append(")) IN (");
							tableFilterBuilder.Append(string.Join(", ", ((IEnumerable<(string, string)>)list2).Select((Func<(string, string), string>)(((string Schema, string Table) e) => EscapeLiteral(e.Schema + "." + e.Table)))));
							tableFilterBuilder.Append(")");
						}
					}
					if (openBracket)
					{
						tableFilterBuilder.Append(")");
					}
					return tableFilterBuilder.ToString();
				};
			}
			return null;
		}

		private static string EscapeLiteral(string s)
		{
            return $"N'{s}'";
		}
	}
}
