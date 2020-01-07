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
	/// 属性创建器扩展
	/// </summary>
	public static class OraclePropertyBuilderExtensions
	{
		/// <summary>
        /// 将密钥属性配置为在以Oracle为目标时使用基于序列的hi-lo模式为新实体生成值。此方法将属性设置为 <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder">正在配置的属性的创建器</param>
        /// <param name="name">序列的名称</param>
        /// <param name="schema">序列的方案</param>
        /// <returns> 同一个创建器实例，以便可以链接多个调用 </returns>
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
		/// 将密钥属性配置为在以Oracle为目标时使用基于序列的hi-lo模式为新实体生成值。此方法将属性设置为 <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <typeparam name="TProperty">正在配置的属性的类型</typeparam>
		/// <param name="propertyBuilder">正在配置的属性的创建器</param>
		/// <param name="name">序列的名称</param>
		/// <param name="schema">序列的架构</param>
		/// <returns>同一个创建器实例，以便可以链接多个调用</returns>
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
		/// 将密钥属性配置为在以Oracle为目标时使用Oracle标识列为新实体生成值。此方法将属性设置为 <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <param name="propertyBuilder">正在配置的属性的创建器</param>
		/// <returns> 同一个创建器实例，以便可以链接多个调用 </returns>
		public static PropertyBuilder UseOracleIdentityColumn([NotNull] this PropertyBuilder propertyBuilder)
		{
			Check.NotNull(propertyBuilder, nameof(propertyBuilder));
			GetOracleInternalBuilder(propertyBuilder).ValueGenerationStrategy(OracleValueGenerationStrategy.IdentityColumn);
			return propertyBuilder;
		}

		/// <summary>
		/// 将密钥属性配置为在以Oracle为目标时使用Oracle标识列为新实体生成值。此方法将属性设置为 <see cref="ValueGenerated.OnAdd" />.
		/// </summary>
		/// <typeparam name="TProperty"> 正在配置的属性的类型</typeparam>
		/// <param name="propertyBuilder"> 正在配置的属性的创建器 </param>
		/// <returns> 同一个创建器实例，以便可以链接多个调用 </returns>
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
