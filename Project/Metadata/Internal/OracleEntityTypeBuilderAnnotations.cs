using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// ʵ�����ʹ�����ע��
	/// </summary>
	public class OracleEntityTypeBuilderAnnotations : RelationalEntityTypeAnnotations
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="internalBuilder">�ڲ�ʵ�����ʹ�����</param>
		/// <param name="configurationSource">����Դ</param>
		public OracleEntityTypeBuilderAnnotations(
			[NotNull] InternalEntityTypeBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// ת���ɱ�
		/// </summary>
		/// <param name="name">����</param>
		/// <returns></returns>
		public virtual bool ToTable([CanBeNull] string name)
		{
			return SetTableName(Check.NullButNotEmpty(name, nameof(name)));
		}
	}
}
