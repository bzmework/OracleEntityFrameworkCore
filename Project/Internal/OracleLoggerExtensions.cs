using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// 日志扩展
	/// </summary>
	public static class OracleLoggerExtensions
	{
		/// <summary>
		/// 数值类型默认警告
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="property">属性</param>
		public static void DecimalTypeDefaultWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics, 
			[NotNull] IProperty property)
		{
			Check.NotNull(diagnostics, nameof(diagnostics));
			Check.NotNull(property, nameof(property));

			EventDefinition<string, string> logDefaultDecimalTypeColumn = OracleStrings.LogDefaultDecimalTypeColumn;
			WarningBehavior logBehavior = logDefaultDecimalTypeColumn.GetLogBehavior(diagnostics);
			if (logDefaultDecimalTypeColumn.GetLogBehavior(diagnostics) != WarningBehavior.Ignore)
			{
				logDefaultDecimalTypeColumn.Log(diagnostics, logBehavior, property.Name, property.DeclaringEntityType.DisplayName());
			}
			if (diagnostics.DiagnosticSource.IsEnabled(logDefaultDecimalTypeColumn.EventId.Name))
			{
				diagnostics.DiagnosticSource.Write(logDefaultDecimalTypeColumn.EventId.Name, new PropertyEventData(logDefaultDecimalTypeColumn, DecimalTypeDefaultWarning, property));
			}
		}

		private static string DecimalTypeDefaultWarning(EventDefinitionBase definition, EventData payload)
		{
			EventDefinition<string, string> obj = (EventDefinition<string, string>)definition;
			PropertyEventData propertyEventData = (PropertyEventData)payload;
			return obj.GenerateMessage(propertyEventData.Property.Name, propertyEventData.Property.DeclaringEntityType.DisplayName());
		}

		/// <summary>
		/// 字节标识列警告
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="property">属性</param>
		public static void ByteIdentityColumnWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics, 
			[NotNull] IProperty property)
		{
			Check.NotNull(diagnostics, nameof(diagnostics));
			Check.NotNull(property, nameof(property));

			EventDefinition<string, string> logByteIdentityColumn = OracleStrings.LogByteIdentityColumn;
			WarningBehavior logBehavior = logByteIdentityColumn.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logByteIdentityColumn.Log(diagnostics, logBehavior, property.Name, property.DeclaringEntityType.DisplayName());
			}
			if (diagnostics.DiagnosticSource.IsEnabled(logByteIdentityColumn.EventId.Name))
			{
				diagnostics.DiagnosticSource.Write(logByteIdentityColumn.EventId.Name, new PropertyEventData(logByteIdentityColumn, ByteIdentityColumnWarning, property));
			}
		}

		private static string ByteIdentityColumnWarning(EventDefinitionBase definition, EventData payload)
		{
			EventDefinition<string, string> obj = (EventDefinition<string, string>)definition;
			PropertyEventData propertyEventData = (PropertyEventData)payload;
			return obj.GenerateMessage(propertyEventData.Property.Name, propertyEventData.Property.DeclaringEntityType.DisplayName());
		}

		/// <summary>
		/// 列找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="tableName">表名</param>
		/// <param name="columnName">列名</param>
		/// <param name="ordinal">顺序</param>
		/// <param name="dataTypeName">数据类型名</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="precision">精度</param>
		/// <param name="scale">范围</param>
		/// <param name="nullable">是否可空</param>
		/// <param name="identity">标识符</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="computedValue">计算值</param>
		public static void ColumnFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string tableName, 
			[NotNull] string columnName, 
			int ordinal, 
			[NotNull] string dataTypeName, 
			int maxLength, 
			int precision, 
			int scale, 
			bool nullable, 
			bool identity, 
			[CanBeNull] string defaultValue, 
			[CanBeNull] string computedValue)
		{
			FallbackEventDefinition definition = OracleStrings.LogFoundColumn;
			WarningBehavior logBehavior = definition.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				definition.Log(diagnostics, logBehavior, delegate (ILogger l)
				{
					l.LogDebug(definition.EventId, null, definition.MessageFormat, tableName, columnName, ordinal, dataTypeName, maxLength, precision, scale, nullable, identity, defaultValue, computedValue);
				});
			}
		}

		/// <summary>
		/// 外键找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="foreignKeyName">外键名</param>
		/// <param name="tableName">表名</param>
		/// <param name="principalTableName">关联的主表名</param>
		/// <param name="onDeleteAction">删除操作</param>
		public static void ForeignKeyFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string foreignKeyName, 
			[NotNull] string tableName, 
			[NotNull] string principalTableName, 
			[NotNull] string onDeleteAction)
		{
			EventDefinition<string, string, string, string> logFoundForeignKey = OracleStrings.LogFoundForeignKey;
			WarningBehavior logBehavior = logFoundForeignKey.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundForeignKey.Log(diagnostics, logBehavior, foreignKeyName, tableName, principalTableName, onDeleteAction);
			}
		}

		/// <summary>
		/// 默认方案找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="schemaName">方案名</param>
		public static void DefaultSchemaFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string schemaName)
		{
			EventDefinition<string> logFoundDefaultSchema = OracleStrings.LogFoundDefaultSchema;
			WarningBehavior logBehavior = logFoundDefaultSchema.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundDefaultSchema.Log(diagnostics, logBehavior, schemaName);
			}
		}

		/// <summary>
		/// 主键找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="primaryKeyName">主键名</param>
		/// <param name="tableName">表名</param>
		public static void PrimaryKeyFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string primaryKeyName, 
			[NotNull] string tableName)
		{
			EventDefinition<string, string> logFoundPrimaryKey = OracleStrings.LogFoundPrimaryKey;
			WarningBehavior logBehavior = logFoundPrimaryKey.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundPrimaryKey.Log(diagnostics, logBehavior, primaryKeyName, tableName);
			}
		}

		/// <summary>
		/// 唯一约束找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="uniqueConstraintName">唯一约束名</param>
		/// <param name="tableName">表名</param>
		public static void UniqueConstraintFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
			[NotNull] string uniqueConstraintName, 
			[NotNull] string tableName)
		{
			EventDefinition<string, string> logFoundUniqueConstraint = OracleStrings.LogFoundUniqueConstraint;
			WarningBehavior logBehavior = logFoundUniqueConstraint.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundUniqueConstraint.Log(diagnostics, logBehavior, uniqueConstraintName, tableName);
			}
		}

		/// <summary>
		/// 索引找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="indexName">索引名称</param>
		/// <param name="tableName">表名称</param>
		/// <param name="unique">是否唯一</param>
		public static void IndexFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string indexName, 
			[NotNull] string tableName, 
			bool unique)
		{
			EventDefinition<string, string, bool> logFoundIndex = OracleStrings.LogFoundIndex;
			WarningBehavior logBehavior = logFoundIndex.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundIndex.Log(diagnostics, logBehavior, indexName, tableName, unique);
			}
		}

		/// <summary>
		/// 外键找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="foreignKeyName">外键名</param>
		/// <param name="tableName">表名</param>
		/// <param name="principalTableName">关联的表名</param>
		public static void ForeignKeyReferencesMissingPrincipalTableWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[CanBeNull] string foreignKeyName, 
			[CanBeNull] string tableName, 
			[CanBeNull] string principalTableName)
		{
			EventDefinition<string, string, string> logPrincipalTableNotInSelectionSet = OracleStrings.LogPrincipalTableNotInSelectionSet;
			WarningBehavior logBehavior = logPrincipalTableNotInSelectionSet.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logPrincipalTableNotInSelectionSet.Log(diagnostics, logBehavior, foreignKeyName, tableName, principalTableName);
			}
		}

		/// <summary>
		/// 外键主体列丢失警告
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="foreignKeyName">外键名</param>
		/// <param name="tableName">表名</param>
		/// <param name="principalColumnName">关联的表列名</param>
		/// <param name="principalTableName">关联的表名</param>
		public static void ForeignKeyPrincipalColumnMissingWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string foreignKeyName,
			[NotNull] string tableName, 
			[NotNull] string principalColumnName,
			[NotNull] string principalTableName)
		{
			EventDefinition<string, string, string, string> logPrincipalColumnNotFound = OracleStrings.LogPrincipalColumnNotFound;
			WarningBehavior logBehavior = logPrincipalColumnNotFound.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logPrincipalColumnNotFound.Log(diagnostics, logBehavior, foreignKeyName, tableName, principalColumnName, principalTableName);
			}
		}

		/// <summary>
		/// 缺少方案警告
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="schemaName">方案名</param>
		public static void MissingSchemaWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[CanBeNull] string schemaName)
		{
			EventDefinition<string> logMissingSchema = OracleStrings.LogMissingSchema;
			WarningBehavior logBehavior = logMissingSchema.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logMissingSchema.Log(diagnostics, logBehavior, schemaName);
			}
		}

		/// <summary>
		/// 缺少表警告
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="tableName">表名</param>
		public static void MissingTableWarning(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[CanBeNull] string tableName)
		{
			EventDefinition<string> logMissingTable = OracleStrings.LogMissingTable;
			WarningBehavior logBehavior = logMissingTable.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logMissingTable.Log(diagnostics, logBehavior, tableName);
			}
		}

		/// <summary>
		/// 表找到
		/// </summary>
		/// <param name="diagnostics">诊断日志</param>
		/// <param name="tableName">表名</param>
		public static void TableFound(
			[NotNull] this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics, 
			[NotNull] string tableName)
		{
			EventDefinition<string> logFoundTable = OracleStrings.LogFoundTable;
			WarningBehavior logBehavior = logFoundTable.GetLogBehavior(diagnostics);
			if (logBehavior != WarningBehavior.Ignore)
			{
				logFoundTable.Log(diagnostics, logBehavior, tableName);
			}
		}
	}
}
