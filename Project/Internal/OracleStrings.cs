using System.Reflection;
using System.Resources;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Diagnostics;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// Oracle字符串
	/// </summary>
	public static class OracleStrings
	{
		private static readonly ResourceManager _resourceManager = new ResourceManager("Oracle.EntityFrameworkCore.Properties.OracleStrings", typeof(OracleStrings).GetTypeInfo().Assembly);

		/// <summary>
		/// 默认数值类型列
		/// </summary>
		public static readonly EventDefinition<string, string> LogDefaultDecimalTypeColumn = new EventDefinition<string, string>(OracleEventId.DecimalTypeDefaultWarning, LogLevel.Warning, "OracleEventId.DecimalTypeDefaultWarning", LoggerMessage.Define<string, string>(LogLevel.Warning, OracleEventId.DecimalTypeDefaultWarning, _resourceManager.GetString("LogDefaultDecimalTypeColumn")));

		/// <summary>
		/// 字节标识列
		/// </summary>
		public static readonly EventDefinition<string, string> LogByteIdentityColumn = new EventDefinition<string, string>(OracleEventId.ByteIdentityColumnWarning, LogLevel.Warning, "OracleEventId.ByteIdentityColumnWarning", LoggerMessage.Define<string, string>(LogLevel.Warning, OracleEventId.ByteIdentityColumnWarning, _resourceManager.GetString("LogByteIdentityColumn")));

		/// <summary>
		/// 找到默认方案
		/// </summary>
		public static readonly EventDefinition<string> LogFoundDefaultSchema = new EventDefinition<string>(OracleEventId.DefaultSchemaFound, LogLevel.Debug, "OracleEventId.DefaultSchemaFound", LoggerMessage.Define<string>(LogLevel.Debug, OracleEventId.DefaultSchemaFound, _resourceManager.GetString("LogFoundDefaultSchema")));

		/// <summary>
		/// 找到列
		/// </summary>
		public static readonly FallbackEventDefinition LogFoundColumn = new FallbackEventDefinition(OracleEventId.ColumnFound, LogLevel.Debug, "OracleEventId.ColumnFound", _resourceManager.GetString("LogFoundColumn"));

		/// <summary>
		/// 找到外键
		/// </summary>
		public static readonly EventDefinition<string, string, string, string> LogFoundForeignKey = new EventDefinition<string, string, string, string>(OracleEventId.ForeignKeyFound, LogLevel.Debug, "OracleEventId.ForeignKeyFound", LoggerMessage.Define<string, string, string, string>(LogLevel.Debug, OracleEventId.ForeignKeyFound, _resourceManager.GetString("LogFoundForeignKey")));

		/// <summary>
		/// 主表不在选择集中
		/// </summary>
		public static readonly EventDefinition<string, string, string> LogPrincipalTableNotInSelectionSet = new EventDefinition<string, string, string>(OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning, LogLevel.Warning, "OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning", LoggerMessage.Define<string, string, string>(LogLevel.Warning, OracleEventId.ForeignKeyReferencesMissingPrincipalTableWarning, _resourceManager.GetString("LogPrincipalTableNotInSelectionSet")));

		/// <summary>
		/// 缺少方案
		/// </summary>
		public static readonly EventDefinition<string> LogMissingSchema = new EventDefinition<string>(OracleEventId.MissingSchemaWarning, LogLevel.Warning, "OracleEventId.MissingSchemaWarning", LoggerMessage.Define<string>(LogLevel.Warning, OracleEventId.MissingSchemaWarning, _resourceManager.GetString("LogMissingSchema")));

		/// <summary>
		/// 缺少表
		/// </summary>
		public static readonly EventDefinition<string> LogMissingTable = new EventDefinition<string>(OracleEventId.MissingTableWarning, LogLevel.Warning, "OracleEventId.MissingTableWarning", LoggerMessage.Define<string>(LogLevel.Warning, OracleEventId.MissingTableWarning, _resourceManager.GetString("LogMissingTable")));

		/// <summary>
		/// 找到表
		/// </summary>
		public static readonly EventDefinition<string> LogFoundTable = new EventDefinition<string>(OracleEventId.TableFound, LogLevel.Debug, "OracleEventId.TableFound", LoggerMessage.Define<string>(LogLevel.Debug, OracleEventId.TableFound, _resourceManager.GetString("LogFoundTable")));

		/// <summary>
		/// 找到索引
		/// </summary>
		public static readonly EventDefinition<string, string, bool> LogFoundIndex = new EventDefinition<string, string, bool>(OracleEventId.IndexFound, LogLevel.Debug, "OracleEventId.IndexFound", LoggerMessage.Define<string, string, bool>(LogLevel.Debug, OracleEventId.IndexFound, _resourceManager.GetString("LogFoundIndex")));

		/// <summary>
		/// 找到主键
		/// </summary>
		public static readonly EventDefinition<string, string> LogFoundPrimaryKey = new EventDefinition<string, string>(OracleEventId.PrimaryKeyFound, LogLevel.Debug, "OracleEventId.PrimaryKeyFound", LoggerMessage.Define<string, string>(LogLevel.Debug, OracleEventId.PrimaryKeyFound, _resourceManager.GetString("LogFoundPrimaryKey")));

		/// <summary>
		/// 找到唯一约束
		/// </summary>
		public static readonly EventDefinition<string, string> LogFoundUniqueConstraint = new EventDefinition<string, string>(OracleEventId.UniqueConstraintFound, LogLevel.Debug, "OracleEventId.UniqueConstraintFound", LoggerMessage.Define<string, string>(LogLevel.Debug, OracleEventId.UniqueConstraintFound, _resourceManager.GetString("LogFoundUniqueConstraint")));

		/// <summary>
		/// 未找到主体列
		/// </summary>
		public static readonly EventDefinition<string, string, string, string> LogPrincipalColumnNotFound = new EventDefinition<string, string, string, string>(OracleEventId.ForeignKeyPrincipalColumnMissingWarning, LogLevel.Warning, "OracleEventId.ForeignKeyPrincipalColumnMissingWarning", LoggerMessage.Define<string, string, string, string>(LogLevel.Warning, OracleEventId.ForeignKeyPrincipalColumnMissingWarning, _resourceManager.GetString("LogPrincipalColumnNotFound")));

		/// <summary>
		/// 需要索引表
		/// </summary>
		public static string IndexTableRequired => GetString("IndexTableRequired");

		/// <summary>
		/// 更改标识列
		/// </summary>
		public static string AlterIdentityColumn => GetString("AlterIdentityColumn");

		/// <summary>
		/// 检测到瞬时异常
		/// </summary>
		public static string TransientExceptionDetected => GetString("TransientExceptionDetected");

		/// <summary>
		/// 没有用户ID
		/// </summary>
		public static string NoUserId => GetString("NoUserId");

		/// <summary>
		/// 标识错误类型
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="entityType">实体类型</param>
		/// <param name="propertyType">属性类型</param>
		/// <returns></returns>
		public static string IdentityBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
		{
			return string.Format(GetString("IdentityBadType", "property", "entityType", "propertyType"), property, entityType, propertyType);
		}

		/// <summary>
		/// 不合格数据类型
		/// </summary>
		/// <param name="dataType">数据类型</param>
		/// <returns></returns>
		public static string UnqualifiedDataType([CanBeNull] object dataType)
		{
			return string.Format(GetString("UnqualifiedDataType", "dataType"), dataType);
		}

		/// <summary>
		/// 序列错误类型
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="entityType">实体类型</param>
		/// <param name="propertyType">属性类型</param>
		/// <returns></returns>
		public static string SequenceBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
		{
			return string.Format(GetString("SequenceBadType", "property", "entityType", "propertyType"), property, entityType, propertyType);
		}

		/// <summary>
		/// 非关键值生成
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="entityType">实体类型</param>
		/// <returns></returns>
		public static string NonKeyValueGeneration([CanBeNull] object property, [CanBeNull] object entityType)
		{
			return string.Format(GetString("NonKeyValueGeneration", "property", "entityType"), property, entityType);
		}

		/// <summary>
		/// 多个实体列
		/// </summary>
		/// <param name="properties">属性</param>
		/// <param name="table">表</param>
		/// <returns></returns>
		public static string MultipleIdentityColumns([CanBeNull] object properties, [CanBeNull] object table)
		{
			return string.Format(GetString("MultipleIdentityColumns", "properties", "table"), properties, table);
		}

		/// <summary>
		/// 脚手架中包含的表无效
		/// </summary>
		/// <param name="table">表</param>
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
