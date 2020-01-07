using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Update.Internal
{
	/// <summary>
	/// UpdateSql������
	/// </summary>
	public class OracleUpdateSqlGenerator : UpdateSqlGenerator, IOracleUpdateSqlGenerator, IUpdateSqlGenerator, ISingletonUpdateSqlGenerator
	{
		private readonly IRelationalTypeMappingSource _typeMappingSource;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">UpdateSql����������</param>
		/// <param name="typeMappingSource">����ӳ��Դ</param>
		/// <param name="logger">��־</param>
		public OracleUpdateSqlGenerator(
			[NotNull] UpdateSqlGeneratorDependencies dependencies,
			[NotNull] IRelationalTypeMappingSource typeMappingSource, 
			IDiagnosticsLogger<DbLoggerCategory.Update> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.ctor);
			}

			_typeMappingSource = typeMappingSource;
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ��������������
		/// </summary>
		/// <param name="commandStringBuilder">�ַ���</param>
		/// <param name="variablesInsert">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <param name="cursorPositionList">�α�λ���б�</param>
		/// <returns></returns>
		public virtual ResultSetMapping AppendBatchInsertOperation(
			StringBuilder commandStringBuilder,
			Dictionary<string, string> variablesInsert,
			IReadOnlyDictionary<ModificationCommand, int> modificationCommands,
			int commandPosition, 
			ref int cursorPosition,
			List<int> cursorPositionList)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchInsertOperation);
				}

				commandStringBuilder.Clear();
				var result = ResultSetMapping.NoResultSet;
				var keyValuePair = modificationCommands.First();
				var tableName = keyValuePair.Key.TableName;
				var schema = keyValuePair.Key.Schema;
				var reads = keyValuePair.Key.ColumnModifications.Where(o => o.IsRead).ToArray();
				var name = tableName;

				int num = 28 - commandPosition.ToString().Length;
				if (Encoding.UTF8.GetByteCount(name) > num)
				{
					name = OracleMigrationsSqlGenerator.DeriveObjectName(null, name, num);
				}
				string nameVariable = $"{name}_{commandPosition}";
				if (reads.Length != 0)
				{
					if (!variablesInsert.Any(p => p.Key == nameVariable))
					{
						StringBuilder stringBuilder = new StringBuilder();

						stringBuilder
							.AppendLine("TYPE r" + nameVariable + " IS RECORD")
							.AppendLine("(");

						stringBuilder
							.AppendJoin(reads, delegate(StringBuilder sb, ColumnModification cm)
							{
								sb.Append("\"" + cm.ColumnName + "\"").Append(" ").AppendLine(GetVariableType(cm));
							}, ",")
							.Append(")")
							.AppendLine(SqlGenerationHelper.StatementTerminator);

						stringBuilder
							.Append("TYPE t" + nameVariable + " IS TABLE OF r" + nameVariable)
							.AppendLine(SqlGenerationHelper.StatementTerminator)
							.Append("l" + nameVariable + " t" + nameVariable)
							.AppendLine(SqlGenerationHelper.StatementTerminator);
						
						variablesInsert.Add(nameVariable, stringBuilder.ToString());
					}

					commandStringBuilder
						.Append("l")
						.Append(nameVariable)
						.Append(" := ")
						.Append("t" + nameVariable)
						.Append("()")
						.AppendLine(SqlGenerationHelper.StatementTerminator);

					commandStringBuilder
						.Append("l" + nameVariable + ".extend(")
						.Append(modificationCommands.Count)
						.Append(")")
						.AppendLine(SqlGenerationHelper.StatementTerminator);
				}

				num = 0;
				foreach (var modificationCommand in modificationCommands)
				{
					var operations = modificationCommand.Key.ColumnModifications;
					var readOperations = operations.Where(o => o.IsRead).ToArray();
					var writeOperations = operations.Where(o => o.IsWrite).ToArray();

					AppendInsertCommand(commandStringBuilder, tableName, schema, writeOperations, (IReadOnlyCollection<ColumnModification>)(object)readOperations);
					AppendReturnInsert(commandStringBuilder, nameVariable, readOperations, num);
					num++;
				}

				num = 0;
				foreach (var modificationCommand in modificationCommands)
				{
					var readOperations = modificationCommand.Key.ColumnModifications.Where(o => o.IsRead).ToArray();

					if (readOperations.Length != 0)
					{
						int value = modificationCommand.Value;
						AppendReturnCursor(commandStringBuilder, nameVariable, readOperations, num, value);
						result = ResultSetMapping.LastInResultSet;
					}
					num++;
				}

				return result;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchInsertOperation, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchInsertOperation);
				}
			}
		}

		/// <summary>
		/// �����һ������ֵ����
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="name">����</param>
		/// <param name="schema">����</param>
		public override void AppendNextSequenceValueOperation(StringBuilder commandStringBuilder, string name, string schema)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendNextSequenceValueOperation);
				}

				commandStringBuilder.Append("SELECT ");
				SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, Check.NotNull(name, nameof(name)), schema);
				commandStringBuilder.Append(".NEXTVAL FROM DUAL");
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendNextSequenceValueOperation, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendNextSequenceValueOperation);
				}
			}
		}

		/// <summary>
		/// ���ֵͷ��
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="operations">���޸Ĳ���</param>
		protected override void AppendValuesHeader(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
		{
			Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
			Check.NotNull(operations, nameof(operations));
			commandStringBuilder.AppendLine();
			commandStringBuilder.Append("VALUES ");
		}

		/// <summary>
		/// ����������²���
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="variablesCommand">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <returns></returns>
		public ResultSetMapping AppendBatchUpdateOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchUpdateOperation);
				}

				string tableName = modificationCommands[commandPosition].TableName;
				string schema = modificationCommands[commandPosition].Schema;
				IReadOnlyList<ColumnModification> columnModifications = modificationCommands[commandPosition].ColumnModifications;
				List<ColumnModification> writeOperations = columnModifications.Where((ColumnModification o) => o.IsWrite).ToList();
				List<ColumnModification> conditionOperations = columnModifications.Where((ColumnModification o) => o.IsCondition).ToList();
				List<ColumnModification> list = columnModifications.Where((ColumnModification o) => o.IsRead).ToList();
				if (list.Count > 0)
				{
					variablesCommand.AppendJoin(list, delegate(StringBuilder sb, ColumnModification cm)
					{
						sb.Append(GetVariableName(cm)).Append(" ").Append(GetVariableType(cm)).Append(";");
					}, Environment.NewLine).AppendLine();
				}
				AppendUpdateCommand(commandStringBuilder, tableName, schema, writeOperations, conditionOperations, list);
				ResultSetMapping result;
				if (list.Count > 0)
				{
					columnModifications.Where((ColumnModification o) => o.IsKey).ToList();
					result = AppendSelectAffectedCommand(commandStringBuilder, list, cursorPosition);
					cursorPosition++;
				}
				else
				{
					result = AppendSelectAffectedCountCommand(commandStringBuilder, cursorPosition);
					cursorPosition++;
				}
				return result;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchUpdateOperation, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchUpdateOperation);
				}
			}
		}

		/// <summary>
		/// �������ɾ������
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="variablesCommand">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <returns></returns>
		public ResultSetMapping AppendBatchDeleteOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchDeleteOperation);
				}

				string tableName = modificationCommands[commandPosition].TableName;
				string schema = modificationCommands[commandPosition].Schema;
				List<ColumnModification> conditionOperations = modificationCommands[commandPosition].ColumnModifications.Where((ColumnModification o) => o.IsCondition).ToList();
				AppendDeleteCommand(commandStringBuilder, tableName, schema, conditionOperations);
				ResultSetMapping result = AppendSelectAffectedCountCommand(commandStringBuilder, cursorPosition);
				cursorPosition++;
				return result;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchDeleteOperation, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleUpdateSqlGenerator, OracleTraceFuncName.AppendBatchDeleteOperation);
				}
			}
		}

		/// <summary>
		/// ���Where������ʶ��
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="columnModification">���޸�</param>
		protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
		{
			SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, columnModification.ColumnName);
			commandStringBuilder.Append(" = ").Append(GetVariableName(columnModification));
		}

		/// <summary>
		/// ���ѡ����Ӱ�������
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="readOperations">���в���</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <returns></returns>
		private ResultSetMapping AppendSelectAffectedCommand(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> readOperations, int cursorPosition)
		{
			commandStringBuilder
				.AppendLine("v_RowCount := SQL%ROWCOUNT;")
				.AppendLine($"OPEN :cur{cursorPosition} FOR")
				.Append("SELECT ")
				.AppendJoin(readOperations, delegate(StringBuilder sb, ColumnModification o)
				{
					sb.Append(GetVariableName(o));
				})
				.AppendLine()
				.AppendLine("FROM DUAL")
				.Append("WHERE ");
			AppendRowsAffectedWhereCondition(commandStringBuilder, 1);
			commandStringBuilder.AppendLine(";");
			return ResultSetMapping.LastInResultSet;
		}

		/// <summary>
		/// �����Where����Ӱ����� 
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="expectedRowsAffected">��Ӱ���Ԥ����</param>
		protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
		{
			commandStringBuilder.Append("v_RowCount = ").Append(expectedRowsAffected.ToString(CultureInfo.InvariantCulture));
		}

		private static ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, int cursorPosition)
		{
			commandStringBuilder.AppendLine("v_RowCount := SQL%ROWCOUNT;").AppendLine($"OPEN :cur{cursorPosition} FOR SELECT v_RowCount FROM DUAL;");
			return ResultSetMapping.LastInResultSet;
		}

		private void AppendReturnInsert(StringBuilder commandStringBuilder, string name, IReadOnlyList<ColumnModification> operations, int commandPosition)
		{
			if (operations.Count > 0)
			{
				commandStringBuilder
					.AppendLine()
					.Append("RETURNING ")
					.AppendJoin(operations, delegate(StringBuilder sb, ColumnModification cm)
					{
						sb.Append(SqlGenerationHelper.DelimitIdentifier(cm.ColumnName));
					})
					.Append(" INTO ")
					.AppendJoin(operations, delegate(StringBuilder sb, ColumnModification cm)
					{
						sb.Append($"l{name}({commandPosition + 1}).{SqlGenerationHelper.DelimitIdentifier(cm.ColumnName)}");
					});
			}
			commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
		}

		private void AppendReturnCursor(StringBuilder commandStringBuilder, string name, IReadOnlyList<ColumnModification> operations, int commandPosition, int cursorPosition)
		{
			if (operations.Count > 0)
			{
				commandStringBuilder
					.Append("OPEN :cur")
					.Append(cursorPosition)
					.Append(" FOR")
					.Append(" SELECT ")
					.AppendJoin(operations, delegate(StringBuilder sb, ColumnModification o)
					{
						sb.Append("l").Append(name).Append("(")
							.Append(commandPosition + 1)
							.Append(").")
							.Append("\"" + o.ColumnName + "\"");
					}, ",")
					.Append(" FROM DUAL")
					.AppendLine(SqlGenerationHelper.StatementTerminator);
			}
		}

		private static string GetVariableName(ColumnModification columnModification)
		{
			string text = "v" + columnModification.ParameterName + "_" + columnModification.ColumnName;
			if (Encoding.UTF8.GetByteCount(text) > 30)
			{
				text = OracleMigrationsSqlGenerator.DeriveObjectName(null, text);
			}
			return text;
		}

		private string GetVariableType(ColumnModification columnModification)
		{
			return _typeMappingSource.FindMapping(columnModification.Property).StoreType;
		}

		private void AppendInsertCommand(StringBuilder commandStringBuilder, string name, string schema, IReadOnlyList<ColumnModification> writeOperations, IReadOnlyCollection<ColumnModification> readOperations)
		{
			AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
			AppendValuesHeader(commandStringBuilder, writeOperations);
			IReadOnlyList<ColumnModification> operations;
			if (writeOperations.Count <= 0)
			{
				IReadOnlyList<ColumnModification> readOnlyList = readOperations.ToArray();
				operations = readOnlyList;
			}
			else
			{
				operations = writeOperations;
			}
			AppendValues(commandStringBuilder, operations);
		}

		private void AppendUpdateCommand(StringBuilder commandStringBuilder, string name, string schema, IReadOnlyList<ColumnModification> writeOperations, IReadOnlyList<ColumnModification> conditionOperations, IReadOnlyCollection<ColumnModification> readOperations)
		{
			AppendUpdateCommandHeader(commandStringBuilder, name, schema, writeOperations);
			AppendWhereClause(commandStringBuilder, conditionOperations);
			if (readOperations.Count > 0)
			{
				commandStringBuilder
					.AppendLine()
					.Append("RETURN ")
					.AppendJoin(readOperations, delegate(StringBuilder sb, ColumnModification cm)
					{
						sb.Append(SqlGenerationHelper.DelimitIdentifier(cm.ColumnName));
					})
					.Append(" INTO ")
					.AppendJoin(readOperations, delegate(StringBuilder sb, ColumnModification cm)
					{
						sb.Append(GetVariableName(cm));
					});
			}
			commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
		}
	}
}
