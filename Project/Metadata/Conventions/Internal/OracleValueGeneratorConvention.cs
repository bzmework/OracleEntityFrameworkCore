using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Conventions.Internal
{
	/// <summary>
	/// 值生成器约定
	/// </summary>
	public class OracleValueGeneratorConvention : RelationalValueGeneratorConvention
	{
		/// <summary>
		/// 应用
		/// </summary>
		/// <param name="propertyBuilder">属性创建器</param>
		/// <param name="name">名称</param>
		/// <param name="annotation">注解</param>
		/// <param name="oldAnnotation">原来的注解</param>
		/// <returns></returns>
		public override Annotation Apply(InternalPropertyBuilder propertyBuilder, string name, Annotation annotation, Annotation oldAnnotation)
		{
            if (name == OracleAnnotationNames.ValueGenerationStrategy)
			{
				propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata), ConfigurationSource.Convention);
				return annotation;
			}
			return base.Apply(propertyBuilder, name, annotation, oldAnnotation);
		}

		/// <summary>
		/// 获得生成的值
		/// </summary>
		/// <param name="property">属性</param>
		/// <returns></returns>
		public override ValueGenerated? GetValueGenerated(Property property)
		{
			ValueGenerated? valueGenerated = base.GetValueGenerated(property);
			if (!valueGenerated.HasValue)
			{
				if (!property.Oracle().GetOracleValueGenerationStrategy(fallbackToModel: false).HasValue)
				{
					return null;
				}
				return ValueGenerated.OnAdd;
			}
			return valueGenerated;
		}
	}
}
