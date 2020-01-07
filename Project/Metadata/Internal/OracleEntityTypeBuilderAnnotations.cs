using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 实体类型创建器注解
	/// </summary>
	public class OracleEntityTypeBuilderAnnotations : RelationalEntityTypeAnnotations
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="internalBuilder">内部实体类型创建器</param>
		/// <param name="configurationSource">配置源</param>
		public OracleEntityTypeBuilderAnnotations(
			[NotNull] InternalEntityTypeBuilder internalBuilder, 
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// 转换成表
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns></returns>
		public virtual bool ToTable([CanBeNull] string name)
		{
			return SetTableName(Check.NullButNotEmpty(name, nameof(name)));
		}
	}
}
