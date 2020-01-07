using System.Reflection;
using System.Resources;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Diagnostics;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// Oracle�ַ���
	/// </summary>
	public static class OracleStrings
	{
		private static readonly ResourceManager _resourceManager = new ResourceManager("Oracle.EntityFrameworkCore.Properties.OracleStrings", typeof(OracleStrings).GetTypeInfo().Assembly);

		/// <summary>
		/// Ĭ����ֵ������
		/// </summary>
		public static readonly EventDefinition<string, string> LogDefaultDecimalTypeColumn = new EventDefinition<string, string>(OracleEventId.DecimalTypeDefaultWarning, LogLevel.Warning, "OracleEventId.DecimalTypeDefaultWarning", LoggerMessage.Define<string, string>(LogLevel.Warning, OracleEventId.DecimalTypeDefaultWarning, _resourceManager.GetString("LogDefaultDecimalTypeColumn")));

		/// <summary>
		/// �ֽڱ�ʶ��
		/// </summary>
		public static readonly EventDefinition<string, string> LogByteIdentityColumn = new EventDefinition<string, string>(OracleEventId.ByteIdentityColumnWarning, LogLevel.Warning, "OracleEventId.ByteIdentityColumnWarning", LoggerMessage.Define<string, string>(LogLevel.Warning, OracleEventId.ByteIdentityColumnWarning, _resourceManager.GetString("LogByteIdentityColumn")));

		/// <summary>
		/// �ҵ�Ĭ�Ϸ���
		/// </summary>
		public static readonly EventDefinition<string> LogFoundDefaultSchema = new EventDefinition<string>(OracleEventId.DefaultSchemaFound, LogLevel.Debug, "OracleEventId.DefaultSchemaFound", LoggerMessage.Define<string>(LogLevel.Debug, OracleEventId.DefaultSchemaFound, _resourceManager.GetString("LogFoundDefaultSchema")));

		/// <summary>
		/// �ҵ���
		/// </summary>
		public static readonly FallbackEventDefinition LogFoundColumn = new FallbackEventDefinition(OracleEventId.ColumnFound, LogLevel.Debug, "OracleEventId.ColumnFound", _resourceManager.GetString("LogFoundColumn"));

		/// <summary>
		/// �ҵ����
		/// </summary>
		public static readonly EventDefinition<string, string, string, string> LogFoundForeignKey = new EventDefinition<string, string, string, string>(OracleEventId.ForeignKeyFound, LogLevel.Debug, "OracleEventId.ForeignKeyFound", LoggerMessage.Define<string, string, string, string>(LogLevel.Debug, OracleEventId.ForeignKeyFound, _resourceManager.GetString("LogFoundForeignKey")));

		/// <summary>
		/// ������ѡ����
		/// </summary>
		public static readonly EventDefinition<string, string, string> LogPrincipalTableNotInSelectionSet = new EventDefinition<string, string, string>(OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning, LogLevel.Warning, "OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning", LoggerMessage.Define<string, string, string>(LogLevel.Warning, OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning, _resourceManager.GetString("LogPrincipalTableNotInSelectionSet")));

		/// <summary>
		/// ȱ�ٷ���
		/// </summary>
		public static readonly EventDefinition<string> LogMissingSchema = new EventDefinition<string>(OracleEventId.MissingSchemaWarning, LogLevel.Warning, "OracleEventId.MissingSchemaWarning", LoggerMessage.Define<string>(LogLevel.Warning, OracleEventId.MissingSchemaWarning, _resourceManager.GetString("LogMissingSchema")));

		/// <summary>
		/// ȱ�ٱ�
		/// </summary>
		public static readonly EventDefinition<string> LogMissingTable = new EventDefinition<string>(OracleEventId.MissingTableWarning, LogLevel.Warning, "OracleEventId.MissingTableWarning", LoggerMessage.Define<string>(LogLevel.Warning, OracleEventId.MissingTableWarning, _resourceManager.GetString("LogMissingTable")));

		/// <summary>
		/// �ҵ���
		/// </summary>
		public static readonly EventDefinition<string> LogFoundTable = new EventDefinition<string>(OracleEventId.TableFound, LogLevel.Debug, "OracleEventId.TableFound", LoggerMessage.Define<string>(LogLevel.Debug, OracleEventId.TableFound, _resourceManager.GetString("LogFoundTable")));

		/// <summary>
		/// �ҵ�����
		/// </summary>
		public static readonly EventDefinition<string, string, bool> LogFoundIndex = new EventDefinition<string, string, bool>(OracleEventId.IndexFound, LogLevel.Debug, "OracleEventId.IndexFound", LoggerMessage.Define<string, string, bool>(LogLevel.Debug, OracleEventId.IndexFound, _resourceManager.GetString("LogFoundIndex")));

		/// <summary>
		/// �ҵ�����
		/// </summary>
		public static readonly EventDefinition<string, string> LogFoundPrimaryKey = new EventDefinition<string, string>(OracleEventId.PrimaryKeyFound, LogLevel.Debug, "OracleEventId.PrimaryKeyFound", LoggerMessage.Define<string, string>(LogLevel.Debug, OracleEventId.PrimaryKeyFound, _resourceManager.GetString("LogFoundPrimaryKey")));

		/// <summary>
		/// �ҵ�ΨһԼ��
		/// </summary>
		public static readonly EventDefinition<string, string> LogFoundUniqueConstraint = new EventDefinition<string, string>(OracleEventId.UniqueConstraintFound, LogLevel.Debug, "OracleEventId.UniqueConstraintFound", LoggerMessage.Define<string, string>(LogLevel.Debug, OracleEventId.UniqueConstraintFound, _resourceManager.GetString("LogFoundUniqueConstraint")));

		/// <summary>
		/// δ�ҵ�������
		/// </summary>
		public static readonly EventDefinition<string, string, string, string> LogPrincipalColumnNotFound = new EventDefinition<string, string, string, string>(OracleEventId.ForeignKeyPrincipalColumnMissingWarning, LogLevel.Warning, "OracleEventId.ForeignKeyPrincipalColumnMissingWarning", LoggerMessage.Define<string, string, string, string>(LogLevel.Warning, OracleEventId.ForeignKeyPrincipalColumnMissingWarning, _resourceManager.GetString("LogPrincipalColumnNotFound")));

		/// <summary>
		/// ��Ҫ������
		/// </summary>
		public static string IndexTableRequired => GetString("IndexTableRequired");

		/// <summary>
		/// ���ı�ʶ��
		/// </summary>
		public static string AlterIdentityColumn => GetString("AlterIdentityColumn");

		/// <summary>
		/// ��⵽˲ʱ�쳣
		/// </summary>
		public static string TransientExceptionDetected => GetString("TransientExceptionDetected");

		/// <summary>
		/// û���û�ID
		/// </summary>
		public static string NoUserId => GetString("NoUserId");

		/// <summary>
		/// ��ʶ��������
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="entityType">ʵ������</param>
		/// <param name="propertyType">��������</param>
		/// <returns></returns>
		public static string IdentityBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
		{
			return string.Format(GetString("IdentityBadType", "property", "entityType", "propertyType"), property, entityType, propertyType);
		}

		/// <summary>
		/// ���ϸ���������
		/// </summary>
		/// <param name="dataType">��������</param>
		/// <returns></returns>
		public static string UnqualifiedDataType([CanBeNull] object dataType)
		{
			return string.Format(GetString("UnqualifiedDataType", "dataType"), dataType);
		}

		/// <summary>
		/// ���д�������
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="entityType">ʵ������</param>
		/// <param name="propertyType">��������</param>
		/// <returns></returns>
		public static string SequenceBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
		{
			return string.Format(GetString("SequenceBadType", "property", "entityType", "propertyType"), property, entityType, propertyType);
		}

		/// <summary>
		/// �ǹؼ�ֵ����
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="entityType">ʵ������</param>
		/// <returns></returns>
		public static string NonKeyValueGeneration([CanBeNull] object property, [CanBeNull] object entityType)
		{
			return string.Format(GetString("NonKeyValueGeneration", "property", "entityType"), property, entityType);
		}

		/// <summary>
		/// ���ʵ����
		/// </summary>
		/// <param name="properties">����</param>
		/// <param name="table">��</param>
		/// <returns></returns>
		public static string MultipleIdentityColumns([CanBeNull] object properties, [CanBeNull] object table)
		{
			return string.Format(GetString("MultipleIdentityColumns", "properties", "table"), properties, table);
		}

		/// <summary>
		/// ���ּ��а����ı���Ч
		/// </summary>
		/// <param name="table">��</param>
		/// <returns></returns>
		public static string InvalidTableToIncludeInScaffolding([CanBeNull] object table)
		{
			return string.Format(GetString("InvalidTableToIncludeInScaffolding", "table"), table);
		}

		private static string GetString(string name, params string[] formatterNames)
		{
			string text = _resourceManager.GetString(name);
			for (int i = 0; i < formatterNames.Length; i++)
			{
				text = text.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
			}
			return text;
		}
	}
}
