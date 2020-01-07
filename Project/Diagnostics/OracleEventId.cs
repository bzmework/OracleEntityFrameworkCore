using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Oracle.EntityFrameworkCore.Diagnostics
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public static class OracleEventId
    {
        // Warning: These values must not change between releases.
        // Only add new values to the end of sections, never in the middle.
        // Try to use <Noun><Verb> naming and be consistent with existing names.
        private enum Id
        {
            // Model validation events
            DecimalTypeDefaultWarning = CoreEventId.ProviderBaseId,
            ByteIdentityColumnWarning,

            // Scaffolding events
            ColumnFound = CoreEventId.ProviderDesignBaseId,
            DefaultSchemaFound,
            ForeignKeyReferencesMissingPrincipalTableWarning,
            MissingSchemaWarning,
            MissingTableWarning,
            TableFound,
            PrimaryKeyFound,
            UniqueConstraintFound,
            IndexFound,
            ForeignKeyFound,
            ForeignKeyPrincipalColumnMissingWarning
        }

        private static readonly string _validationPrefix = LoggerCategory<DbLoggerCategory.Model.Validation>.Name + ".";

        /// <summary>
        /// 数字类型默认警告
        /// </summary>
        public static readonly EventId DecimalTypeDefaultWarning = MakeValidationId(Id.DecimalTypeDefaultWarning);

        /// <summary>
        /// 字节标识列警告
        /// </summary>
        public static readonly EventId ByteIdentityColumnWarning = MakeValidationId(Id.ByteIdentityColumnWarning);
        private static readonly string _scaffoldingPrefix = LoggerCategory<DbLoggerCategory.Scaffolding>.Name + ".";

        /// <summary>
        /// 列找到（column）
        /// </summary>
        public static readonly EventId ColumnFound = MakeScaffoldingId(Id.ColumnFound);

        /// <summary>
        /// 找到默认配置（default schema）
        /// </summary>
        public static readonly EventId DefaultSchemaFound = MakeScaffoldingId(Id.DefaultSchemaFound);

        /// <summary>
        /// 缺少方案警告（missing schema）
        /// </summary>
        public static readonly EventId MissingSchemaWarning = MakeScaffoldingId(Id.MissingSchemaWarning);

        /// <summary>
        /// 缺少表警告（missing table）
        /// </summary>
        public static readonly EventId MissingTableWarning = MakeScaffoldingId(Id.MissingTableWarning);

        /// <summary>
        /// 外键引用缺少主体表警告（foreign key references missing）
        /// </summary>
        public static readonly EventId ForeignKeyReferencesMissingPrincipalTableWarning = MakeScaffoldingId(Id.ForeignKeyReferencesMissingPrincipalTableWarning);

        /// <summary>
        /// 表找到（table）
        /// </summary>
        public static readonly EventId TableFound = MakeScaffoldingId(Id.TableFound);

        /// <summary>
        /// 主键找到（primary key）
        /// </summary>
        public static readonly EventId PrimaryKeyFound = MakeScaffoldingId(Id.PrimaryKeyFound);

        /// <summary>
        /// 唯一约束找到（unique constraint）
        /// </summary>
        public static readonly EventId UniqueConstraintFound = MakeScaffoldingId(Id.UniqueConstraintFound);

        /// <summary>
        /// 索引找到（index）
        /// </summary>
        public static readonly EventId IndexFound = MakeScaffoldingId(Id.IndexFound);

        /// <summary>
        /// 外键找到（foreign key）
        /// </summary>
        public static readonly EventId ForeignKeyFound = MakeScaffoldingId(Id.ForeignKeyFound);

        /// <summary>
        /// 外键引用的主体列丢失警告（principal column）
        /// </summary>
        public static readonly EventId ForeignKeyPrincipalColumnMissingWarning = MakeScaffoldingId(Id.ForeignKeyPrincipalColumnMissingWarning);

        /// <summary>
        /// 制作验证ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static EventId MakeValidationId(Id id)
        {
            return new EventId((int)id, _validationPrefix + id);
        }

        /// <summary>
        /// 制作脚手架ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static EventId MakeScaffoldingId(Id id)
        {
            return new EventId((int)id, _scaffoldingPrefix + id);
        }
    }
}