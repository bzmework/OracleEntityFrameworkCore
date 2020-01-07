using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// ģ�ʹ�������չ
	/// </summary>
	public static class OracleModelBuilderExtensions
	{
		/// <summary>
        /// ��ģ������Ϊ����OracleΪĿ��ʱʹ�û������е�hi-loģʽΪ���Ϊ<see cref="ValueGenerated.OnAdd" />�ļ���������ֵ��
		/// </summary>
		/// <param name="modelBuilder">ģ�ʹ�����</param>
        /// <param name="name"> ���е����� </param>
        /// <returns> ͬһ��������ʵ�����Ա�������Ӷ������ </returns>
		public static ModelBuilder ForOracleUseSequenceHiLo(
			[NotNull] this ModelBuilder modelBuilder, 
			[CanBeNull] string name = null)
		{
			Check.NotNull(modelBuilder, nameof(modelBuilder));
			Check.NullButNotEmpty(name, nameof(name));

            var model = modelBuilder.Model;

            name = name ?? OracleModelAnnotations.DefaultHiLoSequenceName;

			if (model.Oracle().FindSequence(name) == null)
			{
				modelBuilder.HasSequence(name).IncrementsBy(10);
			}
			model.Oracle().ValueGenerationStrategy = OracleValueGenerationStrategy.SequenceHiLo;
			model.Oracle().HiLoSequenceName = name;

			return modelBuilder;
		}

		/// <summary>
        /// ��ģ������Ϊ����OracleΪĿ��ʱʹ��Oracle��ʶ����Ϊ���Ϊ<see cref="ValueGenerated.OnAdd" />�ļ���������ֵ���������Oracleʱ��Ĭ����Ϊ��
		/// </summary>
		/// <param name="modelBuilder">ģ�ʹ�����</param>
        /// <returns> ͬһ��������ʵ�����Ա�������Ӷ������ </returns>
		public static ModelBuilder ForOracleUseIdentityColumns([NotNull] this ModelBuilder modelBuilder)
		{
			Check.NotNull(modelBuilder, nameof(modelBuilder));

			IMutableModel model = modelBuilder.Model;
			model.Oracle().ValueGenerationStrategy = OracleValueGenerationStrategy.IdentityColumn;
			model.Oracle().HiLoSequenceName = null;

			return modelBuilder;
		}
	}
}
