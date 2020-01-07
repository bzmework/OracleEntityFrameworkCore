using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 模型创建器注解
	/// </summary>
	public class OracleModelBuilderAnnotations : OracleModelAnnotations
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="internalBuilder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		public OracleModelBuilderAnnotations(
			[NotNull] InternalModelBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// HiLo序列名
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		public new virtual bool HiLoSequenceName([CanBeNull] string value)
		{
			return SetHiLoSequenceName(value);
		}

		/// <summary>
		/// 值生成策略
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		public new virtual bool ValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
			return SetValueGenerationStrategy(value);
		}
	}
}
