using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 键创建器注解
	/// </summary>
	public class OracleKeyBuilderAnnotations : RelationalKeyAnnotations
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="internalBuilder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		public OracleKeyBuilderAnnotations(
			[NotNull] InternalKeyBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// 返回名称
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		public new virtual bool Name([CanBeNull] string value)
		{
			return SetName(value);
		}
	}
}
