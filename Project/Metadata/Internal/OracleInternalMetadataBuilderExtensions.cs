using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 内部元数据创建器扩展 
	/// </summary>
	public static class OracleInternalMetadataBuilderExtensions
	{
		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static OracleModelBuilderAnnotations Oracle(
			[NotNull] this InternalModelBuilder builder, 
			ConfigurationSource configurationSource)
		{
			return new OracleModelBuilderAnnotations(builder, configurationSource);
		}

		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static OraclePropertyBuilderAnnotations Oracle(
			[NotNull] this InternalPropertyBuilder builder, 
			ConfigurationSource configurationSource)
		{
			return new OraclePropertyBuilderAnnotations(builder, configurationSource);
		}

		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static OracleEntityTypeBuilderAnnotations Oracle(
			[NotNull] this InternalEntityTypeBuilder builder, 
			ConfigurationSource configurationSource)
		{
			return new OracleEntityTypeBuilderAnnotations(builder, configurationSource);
		}

		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static OracleKeyBuilderAnnotations Oracle(
			[NotNull] this InternalKeyBuilder builder, 
			ConfigurationSource configurationSource)
		{
			return new OracleKeyBuilderAnnotations(builder, configurationSource);
		}

		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static OracleIndexBuilderAnnotations Oracle(
			[NotNull] this InternalIndexBuilder builder, 
			ConfigurationSource configurationSource)
		{
			return new OracleIndexBuilderAnnotations(builder, configurationSource);
		}

		/// <summary>
		/// Oracle
		/// </summary>
		/// <param name="builder">内部模型创建器</param>
		/// <param name="configurationSource">配置源</param>
		/// <returns></returns>
		public static RelationalForeignKeyBuilderAnnotations Oracle(
			[NotNull] this InternalRelationshipBuilder builder,
			ConfigurationSource configurationSource)
		{
			return new RelationalForeignKeyBuilderAnnotations(builder, configurationSource);
		}
	}
}
