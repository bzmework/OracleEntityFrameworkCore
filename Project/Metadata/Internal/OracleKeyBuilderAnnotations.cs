using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// ��������ע��
	/// </summary>
	public class OracleKeyBuilderAnnotations : RelationalKeyAnnotations
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="internalBuilder">�ڲ�ģ�ʹ�����</param>
		/// <param name="configurationSource">����Դ</param>
		public OracleKeyBuilderAnnotations(
			[NotNull] InternalKeyBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		public new virtual bool Name([CanBeNull] string value)
		{
			return SetName(value);
		}
	}
}
