using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// ����������ע��
	/// </summary>
	public class OracleIndexBuilderAnnotations : RelationalIndexAnnotations
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="internalBuilder">�ڲ�����������</param>
		/// <param name="configurationSource">����Դ</param>
		public OracleIndexBuilderAnnotations(
			[NotNull] InternalIndexBuilder internalBuilder,
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
