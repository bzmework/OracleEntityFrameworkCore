using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// Ԫ������չ
	/// </summary>
	public static class OracleMetadataExtensions
	{
        /// <summary>
        ///  ��ȡ���Ե�Oracle�ض�Ԫ���ݡ�
        /// </summary>
        /// <param name="property"> ҪΪ���ȡԪ���ݵ����ԡ� </param>
        /// <returns> ���Ե�Oracle�ض�Ԫ���ݡ� </returns>
		public static OraclePropertyAnnotations Oracle([NotNull] this IMutableProperty property)
		{
			Check.NotNull(property, nameof(property));
			return (OraclePropertyAnnotations)((IProperty)property).Oracle();
		}

        /// <summary>
        /// ��ȡ���Ե�Oracle�ض�Ԫ���ݡ�
        /// </summary>
        /// <param name="property"> ҪΪ���ȡԪ���ݵ����� </param>
        /// <returns> ���Ե�Oracle�ض�Ԫ���� </returns>
		public static IOraclePropertyAnnotations Oracle([NotNull] this IProperty property)
		{
			Check.NotNull(property, nameof(property));
			return new OraclePropertyAnnotations(property);
		}

        /// <summary>
        /// ��ȡģ�͵�Oracle�ض�Ԫ���ݡ�
        /// </summary>
        /// <param name="model"> ��ȡԪ���ݵ�ģ�͡� </param>
        /// <returns> ģ�͵�Oracle�ض�Ԫ���� </returns>
		public static OracleModelAnnotations Oracle([NotNull] this IMutableModel model)
		{
			Check.NotNull(model, nameof(model));
			return (OracleModelAnnotations)((IModel)model).Oracle();
		}

        /// <summary>
        /// ��ȡģ�͵�Oracle�ض�Ԫ����
        /// </summary>
        /// <param name="model"> ��ȡԪ���ݵ�ģ�� </param>
        /// <returns> ģ�͵�Oracle�ض�Ԫ���� </returns>
		public static IOracleModelAnnotations Oracle([NotNull] this IModel model)
		{
			Check.NotNull(model, nameof(model));
			return new OracleModelAnnotations(model);
		}
	}
}
