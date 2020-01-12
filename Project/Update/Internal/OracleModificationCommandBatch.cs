using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Update.Internal
{
	/// <summary>
	/// �޸����������� 
	/// </summary>
	public class OracleModificationCommandBatch : ReaderModificationCommandBatch
	{
		private const int MaxParameterCount = 1000;

		private const int MaxRowCount = 200;

		private int _countParameter = 1;

		private int _cursorPosition = 1;

		private readonly int _maxBatchSize;

		private readonly Dictionary<ModificationCommand, int> _batchInsertCommands;

		private readonly List<int> _cursorPositionList;

		private readonly Dictionary<string, string> _variablesInsert;

		private readonly StringBuilder _variablesCommand;

		private readonly IRelationalCommandBuilderFactory _commandBuilderFactory;

		private IDiagnosticsLogger<DbLoggerCategory.Update> m_oracleLogger;

		/// <summary>
		/// ����SQL������
		/// </summary>
		protected new virtual IOracleUpdateSqlGenerator UpdateSqlGenerator
		{
			get { return (IOracleUpdateSqlGenerator)base.UpdateSqlGenerator; }
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="commandBuilderFactory">�����������</param>
		/// <param name="sqlGenerationHelper">SQL���ɰ�����</param>
		/// <param name="updateSqlGenerator">UodateSql������</param>
		/// <param name="valueBufferFactoryFactory">ֵ���湤��</param>
		/// <param name="maxBatchSize">����������С</param>
		/// <param name="logger"></param>
		public OracleModificationCommandBatch(
			[NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
			[NotNull] ISqlGenerationHelper sqlGenerationHelper, 
			[NotNull] IUpdateSqlGenerator updateSqlGenerator,
			[NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory, 
			int? maxBatchSize, IDiagnosticsLogger<DbLoggerCategory.Update> logger = null)
			: base(commandBuilderFactory, sqlGenerationHelper, updateSqlGenerator, valueBufferFactoryFactory)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ctor);
			}

			_commandBuilderFactory = commandBuilderFactory;
			_batchInsertCommands = new Dictionary<ModificationCommand, int>();
			_cursorPositionList = new List<int>();
			_variablesInsert = new Dictionary<string, string>();
			_variablesCommand = new StringBuilder();
            _maxBatchSize = Math.Min(maxBatchSize ?? int.MaxValue, MaxRowCount);
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// �Ƿ�����������
		/// </summary>
		/// <param name="modificationCommand">�޸�����</param>
		/// <returns></returns>
		protected override bool CanAddCommand(ModificationCommand modificationCommand)
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.CanAddCommand);
			}

			if (ModificationCommands.Count >= _maxBatchSize)
			{
				return false;
			}
            var parameterCount = CountParameters(modificationCommand);
            if (_countParameter + parameterCount >= MaxParameterCount)
			{
				return false;
			}
			_countParameter += parameterCount;

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.CanAddCommand);
			}
			return true;
		}

		/// <summary>
		/// ���������ı�
		/// </summary>
		protected override void ResetCommandText()
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ResetCommandText);
			}

			base.ResetCommandText();
			_batchInsertCommands.Clear();
			_cursorPosition = 1;

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ResetCommandText);
			}
		}

		/// <summary>
		/// �����洢��������
		/// </summary>
		/// <returns></returns>
		protected override RawSqlCommand CreateStoreCommand()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.CreateStoreCommand);
				}

				var commandText = GetCommandText();
				var ommandBuilder = _commandBuilderFactory.Create().Append(commandText);
				var parameterValues = new Dictionary<string, object>(GetParameterCount());

				foreach (ColumnModification item in ModificationCommands.SelectMany((ModificationCommand t) => t.ColumnModifications))
				{
					if (item.UseCurrentValueParameter)
					{
						ommandBuilder.AddParameter(item.ParameterName, SqlGenerationHelper.GenerateParameterName(item.ParameterName), item.Property);
						parameterValues.Add(item.ParameterName, item.Value);
					}
					if (item.UseOriginalValueParameter)
					{
						ommandBuilder.AddParameter(item.OriginalParameterName, SqlGenerationHelper.GenerateParameterName(item.OriginalParameterName), item.Property);
						parameterValues.Add(item.OriginalParameterName, item.OriginalValue);
					}
				}
				for (int i = 1; i < _cursorPosition; i++)
				{
					var parameterName = $"cur{i}";
					ommandBuilder.AddRawParameter(parameterName, (DbParameter)new OracleParameter(parameterName, OracleDbType.RefCursor, (object)DBNull.Value, ParameterDirection.Output));
				}
				return new RawSqlCommand(ommandBuilder.Build(), parameterValues);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.CreateStoreCommand, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.CreateStoreCommand);
				}
			}
		}

		/// <summary>
		/// ��������ı�
		/// </summary>
		/// <returns></returns>
		protected override string GetCommandText()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.GetCommandText);
				}
				
				_variablesInsert.Clear();
				_variablesCommand.Clear();

				StringBuilder builder = new StringBuilder();
				builder.AppendLine("BEGIN");
				builder.AppendLine(base.GetCommandText());
				builder.Append(GetBatchInsertCommandText(ModificationCommands.Count));
				if (_cursorPosition > 1)
				{
					var declare = new StringBuilder();
					declare
						.AppendLine("DECLARE")
						.AppendJoin(_variablesInsert.Select(v => v.Value), delegate(StringBuilder sb, string cm)
						{
							sb.Append(cm);
						}, Environment.NewLine)
						.Append((object)_variablesCommand)
						.AppendLine("v_RowCount INTEGER;");
					builder.Insert(0, declare);
				}
				builder.AppendLine("END;");

				return builder.ToString();
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.GetCommandText, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.GetCommandText);
				}
			}
		}

		private string GetBatchInsertCommandText(int lastIndex)
		{
			if (_batchInsertCommands.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			ResultSetMapping resultSetMapping = UpdateSqlGenerator.AppendBatchInsertOperation(stringBuilder, _variablesInsert, _batchInsertCommands, lastIndex - _batchInsertCommands.Count, ref _cursorPosition, _cursorPositionList);
			for (int i = lastIndex - _batchInsertCommands.Count; i < lastIndex; i++)
			{
				CommandResultSet[i] = resultSetMapping;
			}
			if (resultSetMapping != 0)
			{
				CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
			}
			return stringBuilder.ToString();
		}

		private string GetBatchUpdateCommandText(int lastIndex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ResultSetMapping resultSetMapping = UpdateSqlGenerator.AppendBatchUpdateOperation(stringBuilder, _variablesCommand, ModificationCommands, lastIndex, ref _cursorPosition);
			CommandResultSet[lastIndex] = resultSetMapping;
			return stringBuilder.ToString();
		}

		private string GetBatchDeleteCommandText(int lastIndex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ResultSetMapping value = UpdateSqlGenerator.AppendBatchDeleteOperation(stringBuilder, _variablesCommand, ModificationCommands, lastIndex, ref _cursorPosition);
			CommandResultSet[lastIndex] = value;
			return stringBuilder.ToString();
		}

		/// <summary>
		/// ���»��������ı�
		/// </summary>
		/// <param name="commandPosition"></param>
		protected override void UpdateCachedCommandText(int commandPosition)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.UpdateCachedCommandText);
				}

				new StringBuilder();

				// The list of conceptual insert/update/delete 
				// Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ModificationCommandss
				// in the batch.
				// ȡ�ô�����Ĳ���/����/ɾ���еĸ������������ζ����Ӧ�ö���ʵ��ִ�������Լ�������װ
				ModificationCommand modificationCommand = ModificationCommands[commandPosition];

				// �����������װ
				if (modificationCommand.EntityState == EntityState.Added)
				{
					// Add����
					if (_batchInsertCommands.Count > 0 && !CanBeInserted(_batchInsertCommands.First().Key, modificationCommand))
					{
						CachedCommandText.Append(GetBatchInsertCommandText(commandPosition));
						_batchInsertCommands.Clear();
					}

					if (ModificationCommands[commandPosition].ColumnModifications.Where((ColumnModification o) => o.IsRead).ToArray().Length != 0)
					{
						_batchInsertCommands.Add(modificationCommand, _cursorPosition);
						_cursorPosition++;
					}
					else
					{
						_batchInsertCommands.Add(modificationCommand, 0);
					}
					LastCachedCommandIndex = commandPosition;
				}
				else if (modificationCommand.EntityState == EntityState.Deleted)
				{
					// Delete����
					CachedCommandText.Append(GetBatchDeleteCommandText(commandPosition));
					LastCachedCommandIndex = commandPosition;
				}
				else
				{
					// Update����
					CachedCommandText.Append(GetBatchUpdateCommandText(commandPosition));
					LastCachedCommandIndex = commandPosition;
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.UpdateCachedCommandText, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.UpdateCachedCommandText);
				}
			}
		}

		/// <summary>
		/// CommandText�Ƿ���Ч 
		/// </summary>
		/// <returns></returns>
		protected override bool IsCommandTextValid()
		{
			return true;
		}

		/// <summary>
		/// ��ò�������
		/// </summary>
		/// <returns></returns>
		protected override int GetParameterCount()
		{
			return _countParameter;
		}

		private static int CountParameters(ModificationCommand modificationCommand)
		{
            var parameterCount = 0;
			foreach (ColumnModification columnModification in modificationCommand.ColumnModifications)
			{
				if (columnModification.UseCurrentValueParameter)
				{
					parameterCount++;
				}
				if (columnModification.UseOriginalValueParameter)
				{
					parameterCount++;
				}
			}
			return parameterCount;
		}

		private static bool CanBeInserted(ModificationCommand first, ModificationCommand second)
		{
			if (string.Equals(first.TableName, second.TableName, StringComparison.Ordinal) && string.Equals(first.Schema, second.Schema, StringComparison.Ordinal) && (from o in first.ColumnModifications
				where o.IsWrite
				select o.ColumnName).SequenceEqual(from o in second.ColumnModifications
				where o.IsWrite
				select o.ColumnName))
			{
				return (from o in first.ColumnModifications
					where o.IsRead
					select o.ColumnName).SequenceEqual(from o in second.ColumnModifications
					where o.IsRead
					select o.ColumnName);
			}
			return false;
		}

		/// <summary>
		/// �ָ�
		/// </summary>
		/// <param name="relationalReader">��ϵDataReader</param>
		protected override void Consume(RelationalDataReader relationalReader)
		{
			int commandPosition = 0;
			int rowsAffected = 0;

			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.Consume);
				}

				while (true)
				{
					if (commandPosition < CommandResultSet.Count && CommandResultSet[commandPosition] == ResultSetMapping.NoResultSet)
					{
						commandPosition++;
					}
					else
					{
						if (commandPosition < CommandResultSet.Count)
						{
							if (ModificationCommands[commandPosition].RequiresResultPropagation)
							{
								rowsAffected = 0;
								do
								{
									ModificationCommand modificationCommand = ModificationCommands[commandPosition];
									if (!relationalReader.Read())
									{
										throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(ModificationCommands.Count((ModificationCommand m) => m.RequiresResultPropagation), rowsAffected), ModificationCommands[commandPosition].Entries);
									}
									IRelationalValueBufferFactory relationalValueBufferFactory = CreateValueBufferFactory(modificationCommand.ColumnModifications);
									modificationCommand.PropagateResults(relationalValueBufferFactory.Create(relationalReader.DbDataReader));
									rowsAffected++;
									foreach (ColumnModification columnModification in modificationCommand.ColumnModifications)
									{
										if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
										{
											Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.Consume, $"Column Name: {columnModification.ColumnName}");
										}
									}
								}
								while (++commandPosition < CommandResultSet.Count && CommandResultSet[commandPosition - 1] == ResultSetMapping.NotLastInResultSet);
							}
							else
							{
								int expectedRowsAffected = 1;
								while (++commandPosition < CommandResultSet.Count && CommandResultSet[commandPosition - 1] == ResultSetMapping.NotLastInResultSet)
								{
									expectedRowsAffected++;
								}
								if (!relationalReader.Read())
								{
									break;
								}
                                rowsAffected = relationalReader.DbDataReader.GetInt32(0);
								if (rowsAffected != expectedRowsAffected)
								{
									throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(expectedRowsAffected, rowsAffected), ModificationCommands[commandPosition - 1].Entries);
								}
							}
						}
						if (commandPosition >= CommandResultSet.Count || !relationalReader.DbDataReader.NextResult())
						{
							return;
						}
					}
				}
				throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(1, 0), ModificationCommands[commandPosition - 1].Entries);
			}
			catch (DbUpdateException dbEx)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.Consume, dbEx.ToString());
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.Consume, ex.ToString());
				}
				throw new DbUpdateException(RelationalStrings.UpdateStoreException, ex, ModificationCommands[commandPosition - 1].Entries);
			}
		}

		/// <summary>
		/// �첽�ָ�
		/// </summary>
		/// <param name="relationalReader">��ϵDataReader</param>
		/// <param name="cancellationToken">ȡ������</param>
		/// <returns></returns>
		protected override async Task ConsumeAsync(RelationalDataReader relationalReader, CancellationToken cancellationToken = default(CancellationToken))
		{
			int commandPosition = 0;
			
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ConsumeAsync);
				}

				while (true)
				{
					if (commandPosition < CommandResultSet.Count && CommandResultSet[commandPosition] == ResultSetMapping.NoResultSet)
					{
						commandPosition++;
					}
					else
					{
						if (commandPosition < CommandResultSet.Count)
						{
							if (ModificationCommands[commandPosition].RequiresResultPropagation)
							{
								int rowsAffected = 0;
								int num = 0;
								do
								{
									ModificationCommand tableModification = ModificationCommands[commandPosition];
									if (!(await relationalReader.ReadAsync(cancellationToken)))
									{
										throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(ModificationCommands.Count((ModificationCommand m) => m.RequiresResultPropagation), rowsAffected), ModificationCommands[commandPosition].Entries);
									}
									IRelationalValueBufferFactory relationalValueBufferFactory = CreateValueBufferFactory(tableModification.ColumnModifications);
									tableModification.PropagateResults(relationalValueBufferFactory.Create(relationalReader.DbDataReader));
									rowsAffected++;
									num = commandPosition + 1;
									commandPosition = num;
								}
								while (num < CommandResultSet.Count && CommandResultSet[commandPosition - 1] == ResultSetMapping.NotLastInResultSet);
							}
							else
							{
								int expectedRowsAffected = 1;
								while (true)
								{
									int num = commandPosition + 1;
									commandPosition = num;
									if (num >= CommandResultSet.Count || CommandResultSet[commandPosition - 1] != ResultSetMapping.NotLastInResultSet)
									{
										break;
									}
									expectedRowsAffected++;
								}
								if (!relationalReader.Read())
								{
									break;
								}
								int rowsAffected = relationalReader.DbDataReader.GetInt32(0);
								if (rowsAffected != expectedRowsAffected)
								{
									throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(expectedRowsAffected, rowsAffected), ModificationCommands[commandPosition - 1].Entries);
								}
							}
						}
						bool flag = commandPosition < CommandResultSet.Count;
						if (flag)
						{
							flag = await relationalReader.DbDataReader.NextResultAsync(cancellationToken);
						}
						if (!flag)
						{
							return;
						}
					}
				}
				throw new DbUpdateConcurrencyException(RelationalStrings.UpdateConcurrencyException(1, 0), ModificationCommands[commandPosition - 1].Entries);
			}
			catch (DbUpdateException dbEx)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ConsumeAsync, dbEx.ToString());
				}
				throw;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Update>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleModificationCommandBatch, OracleTraceFuncName.ConsumeAsync, ex.ToString());
				}
				throw new DbUpdateException(RelationalStrings.UpdateStoreException, ex, ModificationCommands[commandPosition].Entries);
			}
		}
	}
}
