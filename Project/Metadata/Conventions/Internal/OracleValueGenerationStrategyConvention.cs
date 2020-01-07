using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Conventions.Internal
{
	/// <summary>
	/// ֵ���ɲ���Լ��
	/// </summary>
	public class OracleValueGenerationStrategyConvention : IModelInitializedConvention
	{
		/// <summary>
		/// Ӧ��
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
