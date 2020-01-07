using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 索引创建器注解
	/// </summary>
	public class OracleIndexBuilderAnnotations : RelationalIndexAnnotations
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="internalBuilder">内部索引创建器</param>
		/// <param name="configurationSource">配置源</param>
		public OracleIndexBuilderAnnotations(
			[NotNull] InternalIndexBuilder internalBuilder,
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
