using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Oracle.EntityFrameworkCore.Diagnostics
{
    /// <summary>
    /// �¼�ID
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
        /// ��������Ĭ�Ͼ���
        /// </summary>
        public static readonly EventId DecimalTypeDefaultWarning = MakeValidationId(Id.DecimalTypeDefaultWarning);

        /// <summary>
        /// �ֽڱ�ʶ�о���
        /// </summary>
        public static readonly EventId ByteIdentityColumnWarning = MakeValidationId(Id.ByteIdentityColumnWarning);
        private static readonly string _scaffoldingPrefix = LoggerCategory<DbLoggerCategory.Scaffolding>.Name + ".";

        /// <summary>
        /// ���ҵ���column��
        /// </summary>
        public static readonly EventId ColumnFound = MakeScaffoldingId(Id.ColumnFound);

        /// <summary>
        /// �ҵ�Ĭ�����ã�default schema��
        /// </summary>
        public static readonly EventId DefaultSchemaFound = MakeScaffoldingId(Id.DefaultSchemaFound);

        /// <summary>
        /// ȱ�ٷ������棨missing schema��
        /// </summary>
        public static readonly EventId MissingSchemaWarning = MakeScaffoldingId(Id.MissingSchemaWarning);

        /// <summary>
        /// ȱ�ٱ��棨missing table��
        /// </summary>
        public static readonly EventId MissingTableWarning = MakeScaffoldingId(Id.MissingTableWarning);

        /// <summary>
        /// �������ȱ��������棨foreign key references missing��
        /// </summary>
        public static readonly EventId ForeignKeyReferencesMissingPrincipalTableWarning = MakeScaffoldingId(Id.ForeignKeyReferencesMissingPrincipalTableWarning);

        /// <summary>
        /// ���ҵ���table��
        /// </summary>
        public static readonly EventId TableFound = MakeScaffoldingId(Id.TableFound);

        /// <summary>
        /// �����ҵ���primary key��
        /// </summary>
        public static readonly EventId PrimaryKeyFound = MakeScaffoldingId(Id.PrimaryKeyFound);

        /// <summary>
        /// ΨһԼ���ҵ���unique constraint��
        /// </summary>
        public static readonly EventId UniqueConstraintFound = MakeScaffoldingId(Id.UniqueConstraintFound);

        /// <summary>
        /// �����ҵ���index��
        /// </summary>
        public static readonly EventId IndexFound = MakeScaffoldingId(Id.IndexFound);

        /// <summary>
        /// ����ҵ���foreign key��
        /// </summary>
        public static readonly EventId ForeignKeyFound = MakeScaffoldingId(Id.ForeignKeyFound);

        /// <summary>
        /// ������õ������ж�ʧ���棨principal column��
        /// </summary>
        public static readonly EventId ForeignKeyPrincipalColumnMissingWarning = MakeScaffoldingId(Id.ForeignKeyPrincipalColumnMissingWarning);

        /// <summary>
        /// ������֤ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static EventId MakeValidationId(Id id)
        {
            return new EventId((int)id, _validationPrefix + id);
        }

        /// <summary>
        /// �������ּ�ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static EventId MakeScaffoldingId(Id id)
        {
            return new EventId((int)id, _scaffoldingPrefix + id);
        }
    }
}