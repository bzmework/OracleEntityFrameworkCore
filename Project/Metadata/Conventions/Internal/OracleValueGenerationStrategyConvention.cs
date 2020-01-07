using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Conventions.Internal
{
	/// <summary>
	/// 值生成策略约定
	/// </summary>
	public class OracleValueGenerationStrategyConvention : IModelInitializedConvention
	{
		/// <summary>
		/// 应用
		/// </summary>
		/// <param name="modelBuilder"></param>
		/// <returns></returns>
		public virtual InternalModelBuilder Apply(InternalModelBuilder modelBuilder)
		{
			modelBuilder.Oracle(ConfigurationSource.Convention).ValueGenerationStrategy(OracleValueGenerationStrategy.IdentityColumn);
			return modelBuilder;
		}
	}
}
