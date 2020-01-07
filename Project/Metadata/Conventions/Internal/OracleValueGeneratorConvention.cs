using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Conventions.Internal
{
	/// <summary>
	/// ֵ������Լ��
	/// </summary>
	public class OracleValueGeneratorConvention : RelationalValueGeneratorConvention
	{
		/// <summary>
		/// Ӧ��
		/// </summary>
		/// <param name="propertyBuilder">���Դ�����</param>
		/// <param name="name">����</param>
		/// <param name="annotation">ע��</param>
		/// <param name="oldAnnotation">ԭ����ע��</param>
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
		/// ������ɵ�ֵ
		/// </summary>
		/// <param name="property">����</param>
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
