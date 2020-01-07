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
	/// ��־��չ
	/// </summary>
	public static class OracleLoggerExtensions
	{
		/// <summary>
		/// ��ֵ����Ĭ�Ͼ���
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="property">����</param>
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
		/// �ֽڱ�ʶ�о���
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="property">����</param>
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
		/// ���ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="tableName">����</param>
		/// <param name="columnName">����</param>
		/// <param name="ordinal">˳��</param>
		/// <param name="dataTypeName">����������</param>
		/// <param name="maxLength">��󳤶�</param>
		/// <param name="precision">����</param>
		/// <param name="scale">��Χ</param>
		/// <param name="nullable">�Ƿ�ɿ�</param>
		/// <param name="identity">��ʶ��</param>
		/// <param name="defaultValue">Ĭ��ֵ</param>
		/// <param name="computedValue">����ֵ</param>
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
		/// ����ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="foreignKeyName">�����</param>
		/// <param name="tableName">����</param>
		/// <param name="principalTableName">������������</param>
		/// <param name="onDeleteAction">ɾ������</param>
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
		/// Ĭ�Ϸ����ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="schemaName">������</param>
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
		/// �����ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="primaryKeyName">������</param>
		/// <param name="tableName">����</param>
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
		/// ΨһԼ���ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="uniqueConstraintName">ΨһԼ����</param>
		/// <param name="tableName">����</param>
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
		/// �����ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="indexName">��������</param>
		/// <param name="tableName">������</param>
		/// <param name="unique">�Ƿ�Ψһ</param>
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
		/// ����ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="foreignKeyName">�����</param>
		/// <param name="tableName">����</param>
		/// <param name="principalTableName">�����ı���</param>
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
		/// ��������ж�ʧ����
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="foreignKeyName">�����</param>
		/// <param name="tableName">����</param>
		/// <param name="principalColumnName">�����ı�����</param>
		/// <param name="principalTableName">�����ı���</param>
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
		/// ȱ�ٷ�������
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="schemaName">������</param>
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
		/// ȱ�ٱ���
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="tableName">����</param>
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
		/// ���ҵ�
		/// </summary>
		/// <param name="diagnostics">�����־</param>
		/// <param name="tableName">����</param>
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
