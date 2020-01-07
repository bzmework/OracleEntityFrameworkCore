using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// ģ�ʹ�����ע��
	/// </summary>
	public class OracleModelBuilderAnnotations : OracleModelAnnotations
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="internalBuilder">�ڲ�ģ�ʹ�����</param>
		/// <param name="configurationSource">����Դ</param>
		public OracleModelBuilderAnnotations(
			[NotNull] InternalModelBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// HiLo������
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		public new virtual bool HiLoSequenceName([CanBeNull] string value)
		{
			return SetHiLoSequenceName(value);
		}

		/// <summary>
		/// ֵ���ɲ���
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <returns></returns>
		public new virtual bool ValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
			return SetValueGenerationStrategy(value);
		}
	}
}
