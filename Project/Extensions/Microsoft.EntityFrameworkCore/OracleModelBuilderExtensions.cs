using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// 模型创建器扩展
	/// </summary>
	public static class OracleModelBuilderExtensions
	{
		/// <summary>
        /// 将模型配置为在以Oracle为目标时使用基于序列的hi-lo模式为标记为<see cref="ValueGenerated.OnAdd" />的键属性生成值。
		/// </summary>
		/// <param name="modelBuilder">模型创建器</param>
        /// <param name="name"> 序列的名称 </param>
        /// <returns> 同一个创建器实例，以便可以链接多个调用 </returns>
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
        /// 将模型配置为在以Oracle为目标时使用Oracle标识功能为标记为<see cref="ValueGenerated.OnAdd" />的键属性生成值。这是针对Oracle时的默认行为。
		/// </summary>
		/// <param name="modelBuilder">模型创建器</param>
        /// <returns> 同一个创建器实例，以便可以链接多个调用 </returns>
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
