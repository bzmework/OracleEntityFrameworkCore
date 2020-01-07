using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// 元数据扩展
	/// </summary>
	public static class OracleMetadataExtensions
	{
        /// <summary>
        ///  获取属性的Oracle特定元数据。
        /// </summary>
        /// <param name="property"> 要为其获取元数据的属性。 </param>
        /// <returns> 属性的Oracle特定元数据。 </returns>
		public static OraclePropertyAnnotations Oracle([NotNull] this IMutableProperty property)
		{
			Check.NotNull(property, nameof(property));
			return (OraclePropertyAnnotations)((IProperty)property).Oracle();
		}

        /// <summary>
        /// 获取属性的Oracle特定元数据。
        /// </summary>
        /// <param name="property"> 要为其获取元数据的属性 </param>
        /// <returns> 属性的Oracle特定元数据 </returns>
		public static IOraclePropertyAnnotations Oracle([NotNull] this IProperty property)
		{
			Check.NotNull(property, nameof(property));
			return new OraclePropertyAnnotations(property);
		}

        /// <summary>
        /// 获取模型的Oracle特定元数据。
        /// </summary>
        /// <param name="model"> 获取元数据的模型。 </param>
        /// <returns> 模型的Oracle特定元数据 </returns>
		public static OracleModelAnnotations Oracle([NotNull] this IMutableModel model)
		{
			Check.NotNull(model, nameof(model));
			return (OracleModelAnnotations)((IModel)model).Oracle();
		}

        /// <summary>
        /// 获取模型的Oracle特定元数据
        /// </summary>
        /// <param name="model"> 获取元数据的模型 </param>
        /// <returns> 模型的Oracle特定元数据 </returns>
		public static IOracleModelAnnotations Oracle([NotNull] this IModel model)
		{
			Check.NotNull(model, nameof(model));
			return new OracleModelAnnotations(model);
		}
	}
}
