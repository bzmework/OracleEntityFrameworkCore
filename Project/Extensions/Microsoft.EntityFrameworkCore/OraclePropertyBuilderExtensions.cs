using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// ���Դ�������չ
	/// </summary>
	public static class OraclePropertyBuilderExtensions
	{
		/// <summary>
        /// ����Կ��������Ϊ����OracleΪĿ��ʱʹ�û������е�hi-loģʽΪ��ʵ������ֵ���˷�������������Ϊ <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder">�������õ����ԵĴ�����</param>
        /// <param name="name">���е�����</param>
        /// <param name="schema">���еķ���</param>
        /// <returns> ͬһ��������ʵ�����Ա�������Ӷ������ </returns>
		public static PropertyBuilder ForOracleUseSequenceHiLo(
			[NotNull] this PropertyBuilder propertyBuilder, 
			[CanBeNull] string name = null, 
			[CanBeNull] string schema = null)
		{
			Check.NotNull(propertyBuilder, nameof(propertyBuilder));
			Check.NullButNotEmpty(name, nameof(name));
			Check.NullButNotEmpty(schema, nameof(schema));

			IMutableProperty metadata = propertyBuilder.Metadata;
            name = name ?? OracleModelAnnotations.DefaultHiLoSequenceName;
			IMutableModel model = metadata.DeclaringEntityType.Model;
			if (model.Oracle().FindSequence(name, schema) == null)
			{
				model.Oracle().GetOrAddSequence(name, schema).IncrementBy = 10;
			}
			GetOracleInternalBuilder(propertyBuilder).ValueGenerationStrategy(OracleValueGenerationStrategy.SequenceHiLo);
			metadata.Oracle().HiLoSequenceName = name;

			return propertyBuilder;
		}

		/// <summary>
		/// ����Կ��������Ϊ����OracleΪĿ��ʱʹ�û������е�hi-loģʽΪ��ʵ������ֵ���˷�������������Ϊ <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <typeparam name="TProperty">�������õ����Ե�����</typeparam>
		/// <param name="propertyBuilder">�������õ����ԵĴ�����</param>
		/// <param name="name">���е�����</param>
		/// <param name="schema">���еļܹ�</param>
		/// <returns>ͬһ��������ʵ�����Ա�������Ӷ������</returns>
		public static PropertyBuilder<TProperty> ForOracleUseSequenceHiLo<TProperty>(
			[NotNull] this PropertyBuilder<TProperty> propertyBuilder,
			[CanBeNull] string name = null,
			[CanBeNull] string schema = null)
		{
			Check.NotNull(propertyBuilder, nameof(propertyBuilder));
			Check.NullButNotEmpty(name, nameof(name));
			Check.NullButNotEmpty(schema, nameof(schema));

			return (PropertyBuilder<TProperty>)((PropertyBuilder)propertyBuilder).ForOracleUseSequenceHiLo(name, schema);
		}

		/// <summary>
		/// ����Կ��������Ϊ����OracleΪĿ��ʱʹ��Oracle��ʶ��Ϊ��ʵ������ֵ���˷�������������Ϊ <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <param name="propertyBuilder">�������õ����ԵĴ�����</param>
		/// <returns> ͬһ��������ʵ�����Ա�������Ӷ������ </returns>
		public static PropertyBuilder UseOracleIdentityColumn([NotNull] this PropertyBuilder propertyBuilder)
		{
			Check.NotNull(propertyBuilder, nameof(propertyBuilder));
			GetOracleInternalBuilder(propertyBuilder).ValueGenerationStrategy(OracleValueGenerationStrategy.IdentityColumn);
			return propertyBuilder;
		}

		/// <summary>
		/// ����Կ��������Ϊ����OracleΪĿ��ʱʹ��Oracle��ʶ��Ϊ��ʵ������ֵ���˷�������������Ϊ <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <typeparam name="TProperty"> �������õ����Ե�����</typeparam>
		/// <param name="propertyBuilder"> �������õ����ԵĴ����� </param>
		/// <returns> ͬһ��������ʵ�����Ա�������Ӷ������ </returns>
		public static PropertyBuilder<TProperty> UseOracleIdentityColumn<TProperty>([NotNull] this PropertyBuilder<TProperty> propertyBuilder)
		{
			Check.NotNull(propertyBuilder, nameof(propertyBuilder));
			return (PropertyBuilder<TProperty>)((PropertyBuilder)propertyBuilder).UseOracleIdentityColumn();
		}

		private static OraclePropertyBuilderAnnotations GetOracleInternalBuilder(PropertyBuilder propertyBuilder)
		{
			return propertyBuilder.GetInfrastructure<InternalPropertyBuilder>().Oracle(ConfigurationSource.Explicit);
		}
	}
}
